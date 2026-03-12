namespace LunyScript.SmokeTests
{
	public sealed class Player2 : Script
	{
		public override void Build(ScriptContext context)
		{
			Player1.HandleInputActions(this, nameof(Player2));
		}
	}
}
