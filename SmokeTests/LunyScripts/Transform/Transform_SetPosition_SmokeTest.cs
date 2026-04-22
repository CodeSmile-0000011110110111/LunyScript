using LunyScript;

public class Transform_SetLocalPosition_SmokeTest : Script
{
	public override void Build()
	{
		var positionRoutine = Coroutine("set local position")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetPosition(-3.5, -1, 1)) // to origin
			.WhenElapsed(Transform.SetPosition(0, 0, 0)); // to parent position

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}

public class Transform_SetWorldPosition_SmokeTest : Script
{
	public override void Build()
	{
		var positionRoutine = Coroutine("set world position")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetPosition(3.5, 1, -1).InWorldSpace()) // to parent position
			.WhenElapsed(Transform.SetPosition(0, 0, 0).InWorldSpace()); // to origin

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}
