using System;

namespace LunyScript.SmokeTests.Input
{
	public sealed class Player1 : Script
	{
		public static void HandleInputActions(Script s, String user)
		{
			var upscale = s.Var.Constant("Upscale", 1.2);

			s.When.Input.Action("Move").For(user).Continues(s.Transform.MoveBy(s.Input.Direction("Move")));
			s.When.Input.Action("Look").For(user).Continues(s.Transform.SetLocalRotation(s.Input.Rotation("Look")));
			s.When.Input.Action("Jump").For(user).Begins(s.Transform.ShiftUp(10));
			s.When.Input.Action("Crouch").For(user).Begins(s.Transform.ShiftDown(10));
			s.When.Input.Action("Interact")
				.For(user)
				.Begins(s.Transform.SetLocalScale(upscale))
				.Ends(s.Transform.SetLocalScale(1));
			s.When.Input.Action("Attack")
				.For(user)
				.Begins(s.Object.Enable("AttackButtonPressed"))
				.Ends(s.Object.Disable("AttackButtonPressed"));

			s.When.Input.Action("Leave")
				.For(user)
				.Begins(s.Debug.Log("LEAVE"));
		}

		public override void Build(ScriptContext context)
		{
			Var.Define("PlayerCount", 1);

			On.Ready(Input.AssignUser(nameof(Player1)));

			HandleInputActions(this, nameof(Player1));

		}
	}
}
