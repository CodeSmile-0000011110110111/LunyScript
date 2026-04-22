using Luny.Engine.Bridge;
using LunyScript;

public class Transform_ScaleBy_Uniform_SmokeTest : Script
{
	public override void Build()
	{
		var dir = Var.Define("scale up/down", 1);
		On.FrameUpdate(If(dir == 1)
			.Then(Transform.ScaleBy(1).Speed(0.334))
			.ElseIf(dir == 2)
			.Then(Transform.ScaleBy(-1).Speed(0.667))
		);

		Coroutine("cycle direction")
			.Every(3.667)
			.Seconds()
			.WhenElapsed(dir.Dec(), If(dir == 0).Then(dir.Set(2)));
	}
}

public class Transform_ScaleByX_SmokeTest : Script
{
	public override void Build()
	{
		var dir = Var.Define("scale up/down", 1);
		On.FrameUpdate(If(dir == 1)
			.Then(Transform.ScaleBy(LunyVector3.Right).Speed(2))
			.ElseIf(dir == 2)
			.Then(Transform.ScaleBy(LunyVector3.Left).Speed(2))
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(dir.Dec(), If(dir == 0).Then(dir.Set(2)));
	}
}

public class Transform_ScaleByY_SmokeTest : Script
{
	public override void Build()
	{
		var dir = Var.Define("scale up/down", 1);
		On.FrameUpdate(If(dir == 1)
			.Then(Transform.ScaleBy(LunyVector3.Up).Speed(2))
			.ElseIf(dir == 2)
			.Then(Transform.ScaleBy(LunyVector3.Down).Speed(2))
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(dir.Dec(), If(dir == 0).Then(dir.Set(2)));
	}
}

public class Transform_ScaleByZ_SmokeTest : Script
{
	public override void Build()
	{
		var dir = Var.Define("scale up/down", 1);
		On.FrameUpdate(If(dir == 1)
			.Then(Transform.ScaleBy(LunyVector3.Forward).Speed(2))
			.ElseIf(dir == 2)
			.Then(Transform.ScaleBy(LunyVector3.Back).Speed(2))
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(dir.Dec(), If(dir == 0).Then(dir.Set(2)));
	}
}

public class Transform_ScaleBy_Clamp_SmokeTest : Script
{
	public override void Build()
	{
		var dir = Var.Define("scale up/down", 1);
		On.FrameUpdate(If(dir == 1)
			.Then(Transform.ScaleBy(new LunyVector3(0.1, 0.2, .3)).Speed(5).Clamp(-LunyVector3.One, LunyVector3.One))
			.ElseIf(dir == 2)
			.Then(Transform.ScaleBy(-new LunyVector3(0.1, 0.2, .3)).Speed(5).Clamp(-LunyVector3.One, LunyVector3.One))
		);

		Coroutine("cycle direction")
			.Every(3)
			.Seconds()
			.WhenElapsed(dir.Dec(), If(dir == 0).Then(dir.Set(2)));
	}
}
