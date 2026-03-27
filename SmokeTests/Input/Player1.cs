using LunyScript.Api;
using System;

namespace LunyScript.SmokeTests
{
	public sealed class Player1 : Script
	{
		public static void HandleInputActions(Script s, String playerName)
		{
			var upscale = s.Var.Constant("Upscale", 1.2);

			// input actions filtered by player name
			s.When.Input.Action("Move").ForUser(playerName).Continuing(s.Transform.MoveBy(s.Input.Direction("Move")));
			s.When.Input.Action("Look").ForUser(playerName).Continuing(s.Transform.SetLocalRotation(s.Input.Rotation("Look")));
			s.When.Input.Action("Jump").ForUser(playerName).Started(s.Transform.ShiftUp(10));
			s.When.Input.Action("Crouch").ForUser(playerName).Started(s.Transform.ShiftDown(10));
			s.When.Input.Action("Interact")
				.ForUser(playerName)
				.Started(s.Transform.SetLocalScale(upscale))
				.Ended(s.Transform.SetLocalScale(1));
			s.When.Input.Action("Attack")
				.ForUser(playerName)
				.Started(s.Object.Enable("AttackButtonPressed"))
				.Ended(s.Object.Disable("AttackButtonPressed"));

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
				.ForUser(playerName) // only act for the player sending this action
				.Started(s.If(playerJoined && s.Input.IsPaired(playerName))
					.Then(playerJoined.Toggle(), playerCount.Dec(),
						s.Input.Unpair(playerName), s.Object.Enable($"JoinP{playerNum}"), s.Debug.Log($"LEFT: {playerName}"))
				);
		}

		private static Int32 GetLastDigit(String input)
		{
			if (String.IsNullOrEmpty(input))
				return -1;

			var lastChar = input[input.Length - 1];
			return Char.IsDigit(lastChar) ? lastChar - '0' : -1;
		}

		public override void Build(ScriptBuildContext context)
		{
			// dummy events for the Blocks Inspector
			var number = Var.Define("number", 1234.56789);
			var boolean = Var.Define("bool", true);
			var text = Var.Define("text", "Hello, Luny!");
			On.Created(Debug.Log(number), Debug.Log(boolean), Debug.Log(text));
			On.Ready(Debug.Log("ready"));
			On.Destroyed(Debug.Log("destroyed"));
			Coroutine("test1").Every(1000).Heartbeats().WhenElapsed(Debug.Log("tic"));
			Coroutine("test2").In(5).Minutes().WhenElapsed(Debug.Log("5 min"));


			// Joining is done via the Host's Action Map since the Host owns all devices by default.
			// The device that sends the "Join" action will then be paired with a specific script/object.
			var p1Joined = GVar.Define("Player1 joined", false);
			var p2Joined = GVar.Define("Player2 joined", false);
			var p3Joined = GVar.Define("Player3 joined", false);
			var p4Joined = GVar.Define("Player4 joined", false);
			var playerCount = GVar.Define("PlayerCount", 0);

			When.Input.Action("Join")
				.Started(If(p1Joined == false && !Input.IsPaired(nameof(Player1)))
					.Then(p1Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player1)), Object.Disable("JoinP1"), Debug.Log("JOIN: P1"))
					.ElseIf(!p2Joined && !Input.IsPaired(nameof(Player2)))
					.Then(p2Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player2)), Object.Disable("JoinP2"), Debug.Log("JOIN: P2"))
					.ElseIf(!p3Joined && !Input.IsPaired(nameof(Player3)))
					.Then(p3Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player3)), Object.Disable("JoinP3"), Debug.Log("JOIN: P3"))
					.ElseIf(!p4Joined && !Input.IsPaired(nameof(Player4)))
					.Then(p4Joined.Toggle(), playerCount.Inc(), Input.Pair(nameof(Player4)), Object.Disable("JoinP4"), Debug.Log("JOIN: P4"))
					.Else(Debug.LogWarning("Max Players reached. This demo supports up to four players but there's no hard limit."))
				);

			HandleInputActions(this, nameof(Player1));
		}
	}
}
