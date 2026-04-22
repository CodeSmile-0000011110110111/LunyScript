using Luny.Engine.Bridge;
using LunyScript;

public class Rigidbody_Rotate_DirectionToggle_SmokeTest : Script
{
	public override void Build()
	{
		var direction = GVar.Define("direction", 0);

		Coroutine("direction toggle")
			.Every(30)
			.Heartbeats()
			.WhenElapsed(If(direction <= 0).Then(direction.Set(1)).Else(direction.Set(-1)));
	}
}

public class Rigidbody_RotateAxisX_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(Rigidbody.Kinematic.RotateBy(GVar["direction"] * 10, LunyAxis.X));
}

public class Rigidbody_RotateAxisY_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(Rigidbody.Kinematic.RotateBy(GVar["direction"] * 10, LunyAxis.Y));
}

public class Rigidbody_RotateAxisZ_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(Rigidbody.Kinematic.RotateBy(GVar["direction"] * -10, LunyAxis.Z));
}

public class Rigidbody_RotateAxisX_InWorldSpace_SmokeTest : Script
{
	public override void Build() =>
		On.Heartbeat(Rigidbody.Kinematic.RotateBy(GVar["direction"] * 10, LunyAxis.X).InWorldSpace());
}

public class Rigidbody_RotateAxisY_InWorldSpace_SmokeTest : Script
{
	public override void Build() =>
		On.Heartbeat(Rigidbody.Kinematic.RotateBy(GVar["direction"] * 10, LunyAxis.Y).InWorldSpace());
}

public class Rigidbody_RotateAxisZ_InWorldSpace_SmokeTest : Script
{
	public override void Build() =>
		On.Heartbeat(Rigidbody.Kinematic.RotateBy(GVar["direction"] * -10, LunyAxis.Z).InWorldSpace());
}

public class Rigidbody_RotatePosition_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(Rigidbody.Kinematic.RotateBy(new LunyVector3(0, 12, 0)));
}

public class Rigidbody_RotatePosition_InWorldSpace_SmokeTest : Script
{
	public override void Build() =>
		On.Heartbeat(Rigidbody.Kinematic.RotateBy(new LunyVector3(0, 12, 0)).InWorldSpace());
}
