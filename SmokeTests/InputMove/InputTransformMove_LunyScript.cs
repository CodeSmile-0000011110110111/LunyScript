namespace LunyScript.SmokeTests
{
	/// <summary>
	/// LunyScript implementing a 3-way motion relative to object's orientation
	/// => 11 lines of code, 327 characters (excluding: empty lines, comments, namespace, usings)
	/// </summary>
	public sealed class InputTransformMove_LunyScript : Script
	{
		public override void Build(ScriptBuildContext context) => On.FrameUpdate(
			Transform.MoveBy(Input.Direction("Move")).Speed(4),
			Transform.MoveUp(Input.Button("Jump").Strength).Speed(4),
			Transform.MoveDown(Input.Button("Crouch").Strength).Speed(4)
		);
	}
}
