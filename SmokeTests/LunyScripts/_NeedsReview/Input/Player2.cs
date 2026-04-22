namespace LunyScript.SmokeTests
{
	public sealed class Player2 : Script
	{
		public override void Build() => Player1.HandleInputActions(this, nameof(Player2));
	}
}
