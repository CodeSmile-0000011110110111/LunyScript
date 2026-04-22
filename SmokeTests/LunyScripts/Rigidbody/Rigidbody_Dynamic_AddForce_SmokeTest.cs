using Luny.Engine.Bridge;
using LunyScript;

public class Rigidbody_AddForce_SmokeTest : Script
{
	public override void Build()
	{
		var applyForce = GVar.Define("Fire!", false);

		On.Ready(Object.Enable("WarmUp"));

		Coroutine("warmup timer")
			.In(2)
			.Seconds()
			.WhenElapsed(applyForce.Set(true), Object.Disable("WarmUp"), Object.Enable("Fire"));

		Coroutine("engines shutdown")
			.In(3.5)
			.Seconds()
			.WhenElapsed(applyForce.Set(false), Object.Disable("Fire"), Object.Enable("Done"));

		On.Heartbeat(
			If(GVar["Fire!"]).Then(Rigidbody.Dynamic.AddForce(new LunyVector3(0.02, 1f, 0.1f) * 15))
		);
	}
}

public class Rigidbody_AddForce_IgnoreMass_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"]).Then(Rigidbody.Dynamic.AddForce(new LunyVector3(0.02, 1f, 0.1f) * 15).IgnoreMass())
	);
}

public class Rigidbody_AddForce_AtChildPosition_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddForce(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.AtPosition("AddForce_AtPosition_Child"))
	);
}

public class Rigidbody_AddForce_IgnoreMass_AtChildPosition_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddForce(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.IgnoreMass()
				.AtPosition("AddForce_IgnoreMass_AtPosition_Child"))
	);
}

public class Rigidbody_AddForce_AtPosition_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddForce(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.AtPosition(new LunyVector3(-0.45, -0.6, -0.45)))
	);
}

public class Rigidbody_AddForce_IgnoreMass_AtPosition_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddForce(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.IgnoreMass()
				.AtPosition(new LunyVector3(-0.45, -0.6, -0.45)))
	);
}
