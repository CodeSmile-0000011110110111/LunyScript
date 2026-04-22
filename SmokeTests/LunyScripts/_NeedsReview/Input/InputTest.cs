namespace LunyScript.SmokeTests
{
	public sealed class InputTest : Script
	{
		public override void Build()
		{
			var upscale = Var.Constant("Upscale", 1.2);

			When.Input.Action("Move").Continuing(Transform.MoveBy(Input.Direction("Move")).Speed(25));
			When.Input.Action("Look").Continuing(Transform.SetRotation(Input.Rotation("Look")));
			When.Input.Action("Jump").Performed(Transform.MoveUp(30).InWorldSpace());
			When.Input.Action("Crouch").Performed(Transform.MoveDown(30).InWorldSpace());
			When.Input.Action("Interact")
				.Performed(Transform.SetScale(upscale))
				.Ended(Transform.SetScale(1));
			When.Input.Action("Attack")
				.Performed(Object.Enable("AttackButtonPressed"))
				.Ended(Object.Disable("AttackButtonPressed"));
		}
	}
}
