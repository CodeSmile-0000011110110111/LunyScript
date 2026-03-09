namespace LunyScript.SmokeTests.Input
{
	public sealed class Player4 : Script
	{
		public override void Build(ScriptContext context)
		{
			Player1.HandleInputActions(this, nameof(Player4));

			var playerCount = Var["PlayerCount"];

			When.Input.Action("Join")
				.Begins(
					Debug.Log("Join: Player4"),
					If(playerCount == 3).Then(playerCount.Inc(), Input.AssignUser(nameof(Player4)))
				);

			//When.Input.Action("Leave").Begins(Input.UnpairDevice());
		}
	}
}
