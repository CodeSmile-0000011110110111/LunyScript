using LunyScript;

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
