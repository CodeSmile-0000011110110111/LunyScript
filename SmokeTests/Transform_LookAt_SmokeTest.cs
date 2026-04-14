using LunyScript;

public class Transform_LookAt_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var startingSpeed = 2;
		var speed = GVar.Define("speed", startingSpeed);
		On.FrameUpdate(Transform.LookAt("follow target"));

		Coroutine("toggle speed")
			.Every(6)
			.Seconds()
			.WhenElapsed(
				If(speed > startingSpeed)
					.Then(speed.Div(3), Object.Enable("Slow"), Object.Disable("Fast"))
					.Else(speed.Mul(3), Object.Disable("Slow"), Object.Enable("Fast"))
			);

		On.Ready(Object.Enable("Slow"), Object.Disable("Fast"));
	}
}

public class Transform_LookAt_LockX_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.LookAt("follow target").LockX());
}

public class Transform_LookAt_LockY_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.LookAt("follow target").LockY());
}

public class Transform_LookAt_LockZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.LookAt("follow target").LockZ());
}
