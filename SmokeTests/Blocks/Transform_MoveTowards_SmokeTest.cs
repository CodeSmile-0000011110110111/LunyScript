using LunyScript;

public class Transform_MoveTowards_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var startingSpeed = 2;
		var speed = GVar.Define("speed", startingSpeed);
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(speed));

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

public class Transform_MoveTowards_DeadZone_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).DeadZone(1.5));
	}
}


public class Transform_MoveTowards_Lerp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).Lerp());
}

public class Transform_MoveTowards_Slerp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).Slerp());
}

public class Transform_MoveTowards_LockX_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).LockX());
}

public class Transform_MoveTowards_LockY_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).LockY());
}

public class Transform_MoveTowards_LockZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).LockZ());
}

public class Transform_MoveTowards_LockYZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).LockY().LockZ().Lerp());
}

public class Transform_MoveTowards_LockXZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).LockX().LockZ().Lerp());
}

public class Transform_MoveTowards_LockXY_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.FrameUpdate(Transform.MoveTowards("follow target").Speed(GVar["speed"]).LockX().LockY().Lerp());
}
