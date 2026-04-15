using Luny.Engine.Bridge;
using LunyScript;

public class Transform_ScaleTowards_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var startingSpeed = 0.2;
		var speed = GVar.Define("scale speed", startingSpeed);

		On.FrameUpdate(Transform.ScaleTowards(new LunyVector3(1, 2, 4)).Speed(speed));

		Coroutine("toggle speed")
			.Every(6)
			.Seconds()
			.WhenElapsed(
				If(speed > startingSpeed)
					.Then(speed.Div(10), Object.Enable("Slow"), Object.Disable("Fast"))
					.Else(speed.Mul(10), Object.Disable("Slow"), Object.Enable("Fast"))
			);

		On.Ready(Object.Enable("Slow"), Object.Disable("Fast"));
	}
}
