using LunyScript;

public class Transform_SetUniformScale_SmokeTest : Script
{
	public override void Build()
	{
		var positionRoutine = Coroutine("set scale")
			.Every(1)
			.Seconds()
			.WhenStarted(Transform.SetScale(1.5))
			.WhenElapsed(Transform.SetScale(0.5));

		Coroutine("restart").Every(2).Seconds().WhenElapsed(positionRoutine.Start());
	}
}

public class Transform_SetUniformScale_Variable_SmokeTest : Script
{
	public override void Build()
	{
		var scale = Var.Define("scale", 1);
		On.FrameUpdate(scale.Add(0.02), Transform.SetScale(scale), If(scale > 1.5).Then(scale.Set(0)));
	}
}
