namespace LunyScript.SmokeTests.Input
{
	public sealed class Player2 : Script
	{
		public override void Build(ScriptContext context)
		{
			Player1.HandleInputActions(this, nameof(Player2));

			var playerCount = Var["PlayerCount"];

			When.Input.Action("Join")
				.Begins(
					Debug.Log("Join: Player2"),
					If(playerCount == 1).Then(playerCount.Inc(), Input.AssignUser(nameof(Player2)), Object.Disable("JoinP2"))
				);

			//On.Input.Action("Leave").Begins(playerCount.Dec(), Input.UnpairDevice());
		}
	}
}
