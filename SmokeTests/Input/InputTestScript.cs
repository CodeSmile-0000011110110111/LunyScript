namespace LunyScript.SmokeTests.Input
{
	public sealed class InputTestScript : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(
			If(Input.Button("Jump").IsPressed).Then(Debug.LogInfo("JUMP!"))
			// GVar["Jump"].Set(Input.Button("Jump").IsPressed),
			// Debug.LogInfo($"JUMP: {GVar["Jump"]}"),
			// GVar["lookdir"].Set(Input.Direction("Look")),
			// Debug.LogInfo(GVar["lookdir"])
		);
	}

	public sealed class InputMoveInLocalSpace : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(Transform.Move(Input.Direction("Move"), 4));
	}

	public sealed class InputMoveInWorldSpace : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(Transform.Shift(Input.Direction("Move"), 4));
	}
}
