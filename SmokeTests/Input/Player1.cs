using System;

namespace LunyScript.SmokeTests.Input
{
	public sealed class Player1 : Script
	{
		public static void HandleInputActions(Script s, String playerName)
		{
			var upscale = s.Var.Constant("Upscale", 1.2);

			// input actions filtered by player name
			s.When.Input.Action("Move").For(playerName).Continues(s.Transform.MoveBy(s.Input.Direction("Move")));
			s.When.Input.Action("Look").For(playerName).Continues(s.Transform.SetLocalRotation(s.Input.Rotation("Look")));
			s.When.Input.Action("Jump").For(playerName).Begins(s.Transform.ShiftUp(10));
			s.When.Input.Action("Crouch").For(playerName).Begins(s.Transform.ShiftDown(10));
			s.When.Input.Action("Interact")
				.For(playerName)
				.Begins(s.Transform.SetLocalScale(upscale))
				.Ends(s.Transform.SetLocalScale(1));
			s.When.Input.Action("Attack")
				.For(playerName)
				.Begins(s.Object.Enable("AttackButtonPressed"))
				.Ends(s.Object.Disable("AttackButtonPressed"));

			// Allow joined players to leave a session
			HandlePlayerLeave(s, playerName);
		}

		private static void HandlePlayerLeave(Script s, String playerName)
		{
			// Leaving a session has to be handled by each individual player (script). Here, we use
			// a "Leave" action. But it could also be done via menu or game event (eg "end game session").
			var playerNum = GetLastDigit(playerName);
			var playerJoined = s.GVar[$"Player{playerNum} joined"];
			var playerCount = s.GVar["PlayerCount"];
			s.When.Input.Action("Leave")
				.For(playerName) // only act for the player sending this action
				.Begins(s.If(playerJoined)
					.Then(playerJoined.Toggle(),
						playerCount.Dec(),
						s.Debug.Log($"LEAVE {playerName}"),
						s.Input.Unpair(playerName),
						s.Object.Enable($"JoinP{playerNum}"))
				);
		}

		private static Int32 GetLastDigit(String input)
		{
			if (String.IsNullOrEmpty(input))
				return -1;

			var lastChar = input[input.Length - 1];
			return Char.IsDigit(lastChar) ? lastChar - '0' : -1;
		}

		public override void Build(ScriptContext context)
		{
			// Joining is done via the Host's Action Map since the Host owns all devices by default.
			// The device that sends the "Join" action will then be paired with a specific script/object.

			// FIXME: joining multiple times with the same device is possible
			// Need: Input.Action("Join").Disable(), Input.Action("Leave").Enable()
			var player1Joined = GVar.Define("Player1 joined", false);
			var player2Joined = GVar.Define("Player2 joined", false);
			var player3Joined = GVar.Define("Player3 joined", false);
			var player4Joined = GVar.Define("Player4 joined", false);
			var playerCount = GVar.Define("PlayerCount", 0);
			When.Input.Action("Join")
				.Begins(If(player1Joined == false)
					.Then(player1Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player1)), Object.Disable("JoinP1"))
					.ElseIf(player2Joined == false)
					.Then(player2Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player2)), Object.Disable("JoinP2"))
					.ElseIf(player3Joined == false)
					.Then(player3Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player3)), Object.Disable("JoinP3"))
					.ElseIf(player4Joined == false)
					.Then(player4Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player4)), Object.Disable("JoinP4"))
				);

			HandleInputActions(this, nameof(Player1));
		}
	}
}
