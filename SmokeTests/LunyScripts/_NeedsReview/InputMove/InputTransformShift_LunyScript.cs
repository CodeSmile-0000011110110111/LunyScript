namespace LunyScript.SmokeTests
{
	/// <summary>
	/// LunyScript implementing a 3-way world-axis-aligned motion
	/// => 11 lines of code, 331 characters (excluding: empty lines, comments, namespace, usings)
	/// </summary>
	public sealed class InputTransformShift_LunyScript : Script
	{
		public override void Build() => On.FrameUpdate(
			Transform.MoveBy(Input.Direction("Move")).Speed(4).InWorldSpace(),
			Transform.MoveUp(Input.Button("Jump").Strength).Speed(4).InWorldSpace(),
			Transform.MoveDown(Input.Button("Crouch").Strength).Speed(4).InWorldSpace()
		);
	}
}
