namespace LunyScript.SmokeTests.Input
{
	public sealed class InputToTransformMove : Script
	{
		public override void Build(ScriptContext context)
		{
			On.FrameUpdate(
				If(Input.Direction("Move")).Then(Var["move count"].Inc()),
				If(Input.Button("Jump").IsJustPressed).Then(Var["jump count"].Inc()),
				If(Input.Button("Crouch").IsJustPressed).Then(Var["crouch count"].Inc()),
				If(AND(Var["move count"] > 0), Var["jump count"] > 0, Var["crouch count"] > 0).Then(Debug.LogInfo("yay"))
			);

			On.FrameUpdate(
				Transform.MoveBy(Input.Direction("Move"), 4),
				Transform.MoveUp(Input.Button("Jump").Strength, 4),
				Transform.MoveDown(Input.Button("Crouch").Strength, 4)
			);
		}
	}

	public sealed class InputToTransformShift : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(
			Transform.ShiftBy(Input.Direction("Move"), 4),
			Transform.ShiftUp(Input.Button("Jump").Strength, 4),
			Transform.ShiftDown(Input.Button("Crouch").Strength, 4));
	}
}
