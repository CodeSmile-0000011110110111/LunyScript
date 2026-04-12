using Luny.Engine.Bridge;
using LunyScript;

public class Transform_SetLocalPosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var positionRoutine = Coroutine("set local position")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetPosition(new LunyVector3(-3.5, -1, 1))) // to origin
			.WhenElapsed(Transform.SetPosition(new LunyVector3(0, 0, 0))); // to parent position

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}

public class Transform_SetWorldPosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var positionRoutine = Coroutine("set world position")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetPosition(new LunyVector3(3.5, 1, -1)).InWorldSpace()) // to other's parent
			.WhenElapsed(Transform.SetPosition(new LunyVector3(0, 0, 0)).InWorldSpace()); // to origin

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}
