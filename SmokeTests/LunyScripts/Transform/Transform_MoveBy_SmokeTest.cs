using Luny.Engine.Bridge;
using LunyScript;

/*
 * NOTE: MoveOnPlane is mainly used for directed input since Input.GetDirection() returns a LunyVector2.
 * It currently doesn't have a parameter to specify the plane, ie it is constrained to X/Y axis.
 */

public class Transform_MoveByX_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current dir", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveBy(new LunyVector2(-1, 0)))
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveBy(new LunyVector2(1, 0)))
		);

		//On.FrameUpdate(Transform.RotateBy(135, LunyAxis.Z));

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}

public class Transform_MoveByX_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current dir", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveBy(new LunyVector2(1, 0)).InWorldSpace())
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveBy(new LunyVector2(-1, 0)).InWorldSpace())
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}

public class Transform_MoveByZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current dir", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveBy(new LunyVector2(0, -1)).Speed(2))
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveBy(new LunyVector2(0, 1)).Speed(2))
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}

public class Transform_MoveByZ_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var targetNum = Var.Define("current dir", 2);
		On.FrameUpdate(If(targetNum == 1)
			.Then(Transform.MoveBy(new LunyVector2(0, 1)).Speed(2).InWorldSpace())
			.ElseIf(targetNum == 2)
			.Then(Transform.MoveBy(new LunyVector2(0, -1)).Speed(2).InWorldSpace())
		);

		Coroutine("cycle direction")
			.Every(2)
			.Seconds()
			.WhenElapsed(targetNum.Dec(), If(targetNum == 0).Then(targetNum.Set(2)));
	}
}
