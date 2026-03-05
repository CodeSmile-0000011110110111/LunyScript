using Luny.Engine.Bridge;

namespace LunyScript.SmokeTests.Input
{
	public sealed class InputTest : Script
	{
		public override void Build(ScriptContext context)
		{
			When.InputAction("Move").Continues(Transform.MoveBy(Input.Direction("Move")));
			When.InputAction("Look").Continues(Transform.SetLocalRotation(Input.Direction("Look")));
			When.InputAction("Jump").Begins(Transform.ShiftUp(10));
			When.InputAction("Crouch").Begins(Transform.ShiftDown(10));
			When.InputAction("Interact")
				.Begins(Transform.SetLocalScale(1.2))
				.Ends(Transform.SetLocalScale(1));
			When.InputAction("Attack")
				.Begins(Object.Enable("AttackButtonPressed"))
				.Ends(Object.Disable("AttackButtonPressed"));
		}
	}
}
