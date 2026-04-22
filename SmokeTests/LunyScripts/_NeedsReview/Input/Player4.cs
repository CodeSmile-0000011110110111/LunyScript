namespace LunyScript.SmokeTests
{
	public sealed class Player4 : Script
	{
		public override void Build() => Player1.HandleInputActions(this, nameof(Player4));
	}
}
