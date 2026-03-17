namespace LunyScript.SmokeTests
{
	public sealed class Player2 : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			Player1.HandleInputActions(this, nameof(Player2));
		}
	}
}
