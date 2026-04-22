using LunyScript;

public class Transform_RotateX_SmokeTest : Script
{
	public override void Build() => On.FrameUpdate(Transform.RotateBy(50).AroundX());
}

public class Transform_RotateY_SmokeTest : Script
{
	public override void Build() => On.FrameUpdate(Transform.RotateBy(50).AroundY());
}

public class Transform_RotateZ_SmokeTest : Script
{
	public override void Build() => On.FrameUpdate(Transform.RotateBy(50).AroundZ());
}

public class Transform_RotateX_InWorldSpace_SmokeTest : Script
{
	public override void Build() => On.FrameUpdate(Transform.RotateBy(50).AroundX().InWorldSpace());
}

public class Transform_RotateY_InWorldSpace_SmokeTest : Script
{
	public override void Build() => On.FrameUpdate(Transform.RotateBy(50).AroundY().InWorldSpace());
}

public class Transform_RotateZ_InWorldSpace_SmokeTest : Script
{
	public override void Build() => On.FrameUpdate(Transform.RotateBy(50).AroundZ().InWorldSpace());
}

public class Transform_RotateX_Clamp_SmokeTest : Script
{
	public override void Build()
	{
		var direction = Var.Define("direction", 1);
		On.FrameUpdate(Transform.RotateBy(300 * direction).AroundX().Clamp(-80, 80));

		Coroutine("flip direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(direction.Mul(-1));
	}
}

public class Transform_RotateX_Clamp_InWorldSpace_SmokeTest : Script
{
	public override void Build()
	{
		var direction = Var.Define("direction", 1);
		On.FrameUpdate(Transform.RotateBy(300 * direction).AroundX().Clamp(-80, 80).InWorldSpace());

		Coroutine("flip direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(direction.Mul(-1));
	}
}
