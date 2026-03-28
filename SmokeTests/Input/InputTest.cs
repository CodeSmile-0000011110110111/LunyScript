namespace LunyScript.SmokeTests
{
	public sealed class InputTest : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			var upscale = Var.Constant("Upscale", 1.2);

			When.Input.Action("Move").Continuing(Transform.MoveBy(Input.Direction("Move"), 25));
			When.Input.Action("Look").Continuing(Transform.SetLocalRotation(Input.Rotation("Look")));
			When.Input.Action("Jump").Performed(Transform.ShiftUp(30));
			When.Input.Action("Crouch").Performed(Transform.ShiftDown(30));
			When.Input.Action("Interact")
				.Performed(Transform.SetLocalScale(upscale))
				.Ended(Transform.SetLocalScale(1));
			When.Input.Action("Attack")
				.Performed(Object.Enable("AttackButtonPressed"))
				.Ended(Object.Disable("AttackButtonPressed"));
		}
	}
}
