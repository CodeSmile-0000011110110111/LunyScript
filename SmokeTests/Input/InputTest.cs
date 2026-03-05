namespace LunyScript.SmokeTests.Input
{
	public sealed class InputTest : Script
	{
		public override void Build(ScriptContext context)
		{
			var upscale = Var.Constant("Upscale", 1.2);

			When.InputAction("Move").Continues(Transform.MoveBy(Input.Direction("Move")));
			When.InputAction("Look").Continues(Transform.SetLocalRotation(Input.Rotation("Look")));
			When.InputAction("Jump").Begins(Transform.ShiftUp(10));
			When.InputAction("Crouch").Begins(Transform.ShiftDown(10));
			When.InputAction("Interact")
				.Begins(Transform.SetLocalScale(upscale))
				.Ends(Transform.SetLocalScale(1));
			When.InputAction("Attack")
				.Begins(Object.Enable("AttackButtonPressed"))
				.Ends(Object.Disable("AttackButtonPressed"));
		}
	}
}
