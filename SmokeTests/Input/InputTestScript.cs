namespace LunyScript.SmokeTests.Input
{
	public sealed class InputTestScript : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(
			If(Input.Button("Jump").IsPressed).Then(Debug.LogInfo("JUMP!")),
			// GVar["Jump"].Set(Input.Button("Jump").IsPressed),
			// Debug.LogInfo($"JUMP: {GVar["Jump"]}"),
			GVar["Move"].Set(Input.Direction("Move")),
			Debug.LogInfo($"MOVE: {GVar["Move"]}")
		);
	}
}
