using LunyScript;

public class Transform_SetLocalRotation_SmokeTest : Script
{
	public override void Build()
	{
		var positionRoutine = Coroutine("set local rotation")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetRotation(-45, -90, 90))
			.WhenElapsed(Transform.SetRotation(0, 0, 0));

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}

public class Transform_SetWorldRotation_SmokeTest : Script
{
	public override void Build()
	{
		var positionRoutine = Coroutine("set world rotation")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetRotation(45, 180, -90).InWorldSpace())
			.WhenElapsed(Transform.SetRotation(0, 0, 0).InWorldSpace());

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}
