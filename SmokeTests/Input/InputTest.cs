namespace LunyScript.SmokeTests.Input
{
	public sealed class InputTest : Script
	{
		public override void Build(ScriptContext context)
		{
			When.InputAction("Move").Continues(Transform.MoveBy(Input.Direction("Move")));
			When.InputAction("Look").Continues(Transform.SetLocalRotation(Input.Direction("Look")));
			When.InputAction("Jump").Begins(Debug.Log("Jump!"), Transform.ShiftUp(Input.Direction("Move")));
			When.InputAction("Crouch").Begins(Debug.Log("Crouch!"), Transform.ShiftDown(Input.Direction("Move")));
			When.InputAction("Attack")
				.Begins(Object.Enable("AttackButtonPressed"))
				.Ends(Object.Disable("AttackButtonPressed"));
		}
	}
}
