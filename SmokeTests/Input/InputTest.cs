using LunyScript.Api;

namespace LunyScript.SmokeTests
{
	public sealed class InputTest : Script
	{
		public override void Build(ScriptContext context)
		{
			var upscale = Var.Constant("Upscale", 1.2);

			When.Input.Action("Move").Continues(Transform.MoveBy(Input.Direction("Move"), 25));
			When.Input.Action("Look").Continues(Transform.SetLocalRotation(Input.Rotation("Look")));
			When.Input.Action("Jump").Begins(Transform.ShiftUp(30));
			When.Input.Action("Crouch").Begins(Transform.ShiftDown(30));
			When.Input.Action("Interact")
				.Begins(Transform.SetLocalScale(upscale))
				.Ends(Transform.SetLocalScale(1));
			When.Input.Action("Attack")
				.Begins(Object.Enable("AttackButtonPressed"))
				.Ends(Object.Disable("AttackButtonPressed"));
		}
	}
}
