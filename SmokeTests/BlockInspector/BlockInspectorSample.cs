namespace LunyScript.SmokeTests.BlockInspector
{
	public class BlockInspectorSample : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			On.Created(Debug.Log($"Hello, {nameof(BlockInspectorSample)}!"));

			// dummy events for the Blocks Inspector
			var number = Var.Define("number", 1234);
			var boolean = Var.Define("boolean", true);
			var text = Var.Define("text", "Hello, Luny!");

			var counter = Var.Define("counter", 0);
			On.Heartbeat(number.Inc(), counter.Inc(),
				If(counter < 50)
					.Then(Debug.Log("less than 50"))
					.ElseIf(counter < 100)
					.Then(Debug.Log("less than 100"))
					.ElseIf(counter < 150)
					.Then(Debug.Log("less than 150"))
					.ElseIf(counter < 200)
					.Then(Debug.Log("less than 200"))
					.Else(counter.Set(0), boolean.Toggle()),
				While(!boolean, number > 1700, text == "Hello, Luny!").Do(text.Set("Why do you say 'Hello'? I say goodbye.")),
				If(boolean)
					.Then(Debug.Log(text))
					.ElseIf(boolean != false)
					.Then(Debug.Log(text))
					.ElseIf(!boolean == false)
					.Then(Debug.Log(text))
					.ElseIf(!boolean != true)
					.Then(Debug.Log(text))
					.Else(Debug.Log(text))
			);

			var permaToggle = Var.Define("toggles continuously", true);
			On.Heartbeat(
				If(permaToggle)
					.Then(permaToggle.Toggle())
					.ElseIf(!permaToggle)
					.Then(permaToggle.Toggle())
					.Else(Debug.Log("unreachable branch")),
				For(3).Do(Debug.Log("thrice")),
				For(9, 3).Do(Debug.Log("thrice too")),
				For(5, -1).Do(Debug.Log("five time, but backwards")),
				For(5, -2).Do(Debug.Log("five time, but backwards in steps 2")),
				For(5, -3).Do(Debug.Log("five time, but backwards in steps 3")),
				For(5, 2).Do(Debug.Log("five time, steps 2")),
				For(5, 3).Do(Debug.Log("five time, steps 3"))
			);

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
		}
	}
}
