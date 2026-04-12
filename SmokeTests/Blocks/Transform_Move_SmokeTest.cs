using Luny.Engine.Bridge;
using LunyScript;

public class Transform_Move_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var positionRoutine = Coroutine("set position")
			.Every(4)
			.Seconds()
			.WhenStarted(Transform.SetPosition(new LunyVector3(0, 0, 0)))
			.WhenElapsed(Transform.SetPosition(new LunyVector3(-5, 2, 15)));

		Coroutine("restart").Every(5).Seconds().WhenElapsed(positionRoutine.Start());
	}
}

public class Transform_MoveBy_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current dir", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveOnPlane(new LunyVector2(1, 1)))
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveOnPlane(new LunyVector2(-1, -1)))
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}
public class Transform_MoveBy_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current dir", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveOnPlane(new LunyVector2(1, 1)).InWorldSpace())
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveOnPlane(new LunyVector2(-1, -1)).InWorldSpace())
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}
public class Transform_MoveTowards_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current target", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveTowards("target a").Speed(2.2).Lerp())
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveTowards("target b").Speed(3).Lerp())
		);

		Coroutine("cycle targets")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}

public class Transform_MoveUp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveUp(0.1));
}

public class Transform_MoveDown_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveDown(0.1));
}

public class Transform_MoveRight_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveRight(0.1));
}

public class Transform_MoveLeft_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveLeft(0.1));
}

public class Transform_MoveForward_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveForward(0.1));
}

public class Transform_MoveBack_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveBack(0.1));
}

public class Transform_MoveUp_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveUp(0.2).InWorldSpace());
}

public class Transform_MoveDown_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveDown(0.2).InWorldSpace());
}

public class Transform_MoveRight_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveRight(0.2).InWorldSpace());
}

public class Transform_MoveLeft_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveLeft(0.2).InWorldSpace());
}

public class Transform_MoveForward_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveForward(0.2).InWorldSpace());
}

public class Transform_MoveBack_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveBack(0.2).InWorldSpace());
}
