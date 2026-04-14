using LunyScript;

public class Transform_RotateTowards_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var startingSpeed = 1.1;
		var speed = GVar.Define("rotate speed", startingSpeed);

		On.FrameUpdate(Transform.RotateTowards("follow target").Speed(speed));

		Coroutine("toggle speed")
			.Every(6)
			.Seconds()
			.WhenElapsed(
				If(speed > startingSpeed)
					.Then(speed.Div(10), Object.Enable("Slow"), Object.Disable("Fast"))
					.Else(speed.Mul(10), Object.Disable("Slow"), Object.Enable("Fast"))
			);

		On.Ready(Object.Enable("Slow"), Object.Disable("Fast"));

		var motion = Var.Define("motion", true);
		Coroutine("toggle motion")
			.Every(3.5)
			.Seconds()
			.WhenElapsed(
				motion.Toggle(),
				If(motion)
					.Then(Object.Enable("Rotate360"))
					.Else(Object.Disable("Rotate360"))
			);
	}
}

public class Transform_RotateTowards_Instant_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateTowards("follow target").Instant());
}

public class Transform_RotateTowards_Lerp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.RotateTowards("follow target").Linear().Speed(GVar["rotate speed"]));
}

public class Transform_RotateTowards_Slerp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.RotateTowards("follow target").Spherical().Speed(GVar["rotate speed"]));
}

public class Transform_RotateTowards_LockX_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.RotateTowards("follow target axislock").Instant().LockX());
}

public class Transform_RotateTowards_LockY_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.RotateTowards("follow target axislock").Instant().LockY());
}

public class Transform_RotateTowards_LockZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.RotateTowards("follow target axislock").Instant().LockZ());
}
