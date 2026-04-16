using Luny.Engine.Bridge;
using LunyScript;

public class Rigidbody_Move_DirectionToggle_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var direction = GVar.Define("direction", 0);

		Coroutine("direction toggle")
			.Every(100)
			.Heartbeats()
			.WhenElapsed(If(direction <= 0).Then(direction.Set(0.01)).Else(direction.Set(-0.01)));
	}
}

public class Rigidbody_MoveAxisX_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Rigidbody.Kinematic.MoveBy(GVar["direction"] * 10, LunyAxis.X));
}

public class Rigidbody_MoveAxisY_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Rigidbody.Kinematic.MoveBy(GVar["direction"] * 10, LunyAxis.Y));
}

public class Rigidbody_MoveAxisZ_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Rigidbody.Kinematic.MoveBy(GVar["direction"] * -10, LunyAxis.Z));
}

public class Rigidbody_MoveAxisX_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.Heartbeat(Rigidbody.Kinematic.MoveBy(GVar["direction"] * 10, LunyAxis.X).InWorldSpace());
}

public class Rigidbody_MoveAxisY_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.Heartbeat(Rigidbody.Kinematic.MoveBy(GVar["direction"] * 10, LunyAxis.Y).InWorldSpace());
}

public class Rigidbody_MoveAxisZ_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.Heartbeat(Rigidbody.Kinematic.MoveBy(GVar["direction"] * -10, LunyAxis.Z).InWorldSpace());
}

public class Rigidbody_MovePosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(Rigidbody.Kinematic.MoveBy(new LunyVector3(0.02, 0.01, 0.01)));
}

public class Rigidbody_MovePosition_InWorldSpace_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) =>
		On.Heartbeat(Rigidbody.Kinematic.MoveBy(new LunyVector3(0.02, 0.01, 0.01)).InWorldSpace());
}
