using Luny.Engine.Bridge;
using LunyScript;

public class Transform_RotateX_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateBy(50, LunyAxis.X));
}

public class Transform_RotateY_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateBy(50, LunyAxis.Y));
}

public class Transform_RotateZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateBy(50, LunyAxis.Z));
}

public class Transform_RotateX_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateBy(50, LunyAxis.X).InWorldSpace());
}

public class Transform_RotateY_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateBy(50, LunyAxis.Y).InWorldSpace());
}

public class Transform_RotateZ_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.RotateBy(50, LunyAxis.Z).InWorldSpace());
}

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
