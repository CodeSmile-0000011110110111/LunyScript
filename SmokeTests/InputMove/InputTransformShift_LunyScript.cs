namespace LunyScript.SmokeTests
{
	/// <summary>
	/// LunyScript implementing a 3-way world-axis-aligned motion
	/// => 11 lines of code, 331 characters (excluding: empty lines, comments, namespace, usings)
	/// </summary>
	public sealed class InputTransformShift_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(
			Transform.ShiftBy(Input.Direction("Move"), 4),
			Transform.ShiftUp(Input.Button("Jump").Strength, 4),
			Transform.ShiftDown(Input.Button("Crouch").Strength, 4)
		);
	}
}
