namespace LunyScript.SmokeTests
{
	public sealed class Player3 : Script
	{
		public override void Build() => Player1.HandleInputActions(this, nameof(Player3));
	}
}
