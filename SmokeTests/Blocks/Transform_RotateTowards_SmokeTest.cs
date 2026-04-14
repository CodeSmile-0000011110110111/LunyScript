using Luny.Engine.Bridge;
using LunyScript;

public class Transform_RotateTowards_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var startingSpeed = 60;
		var speed = GVar.Define("speed", startingSpeed);
		On.FrameUpdate(Transform.RotateTowards("follow target").Speed(speed));

		Coroutine("toggle speed")
			.Every(6)
			.Seconds()
			.WhenElapsed(
				If(speed > startingSpeed)
					.Then(speed.Div(5), Object.Enable("Slow"), Object.Disable("Fast"))
					.Else(speed.Mul(5), Object.Disable("Slow"), Object.Enable("Fast"))
			);

		On.Ready(Object.Enable("Slow"), Object.Disable("Fast"));
	}
}

public class Transform_RotateTowards_Lerp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateTowards("follow target").Speed(GVar["speed"]).Lerp());
}

public class Transform_RotateTowards_Slerp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateTowards("follow target").Speed(GVar["speed"]).Slerp());
}


/*
public class Transform_RotateX_Clamp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var direction = Var.Define("direction", 1);
		On.FrameUpdate(Transform.RotateBy(300 * direction, LunyAxis.X).Clamp(-80, 80));

		Coroutine("flip direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(direction.Mul(-1));
	}
}

public class Transform_RotateX_Clamp_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var direction = Var.Define("direction", 1);
		On.FrameUpdate(Transform.RotateBy(300 * direction, LunyAxis.X).Clamp(-80, 80).InWorldSpace());

		Coroutine("flip direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(direction.Mul(-1));
	}
}
*/
