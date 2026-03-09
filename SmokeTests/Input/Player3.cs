namespace LunyScript.SmokeTests.Input
{
	public sealed class Player3 : Script
	{
		public override void Build(ScriptContext context)
		{
			Player1.HandleInputActions(this, nameof(Player3));

			var playerCount = Var["PlayerCount"];

			When.Input.Action("Join")
				.Begins(
					Debug.Log("Join: Player3"),
					If(playerCount == 2).Then(playerCount.Inc(), Input.AssignUser(nameof(Player3)))
				);

			//When.Input.Action("Leave").Begins(Input.UnpairDevice());
		}
	}
}
