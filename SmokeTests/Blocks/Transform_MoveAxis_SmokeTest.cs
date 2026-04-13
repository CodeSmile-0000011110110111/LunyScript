using Luny.Engine.Bridge;
using LunyScript;

public class Transform_MoveAxis_SmokeTest : Script
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

public class Transform_MoveUp_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.FrameUpdate(Transform.MoveUp(0.1).Speed(3));
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
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveUp(0.1).InWorldSpace().Speed(5));
}

public class Transform_MoveDown_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveDown(0.1).InWorldSpace().Speed(3));
}

public class Transform_MoveRight_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveRight(0.1).InWorldSpace().Speed(3));
}

public class Transform_MoveLeft_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveLeft(0.1).InWorldSpace().Speed(3));
}

public class Transform_MoveForward_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveForward(0.1).InWorldSpace().Speed(3));
}

public class Transform_MoveBack_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Transform.MoveBack(0.1).InWorldSpace().Speed(3));
}
