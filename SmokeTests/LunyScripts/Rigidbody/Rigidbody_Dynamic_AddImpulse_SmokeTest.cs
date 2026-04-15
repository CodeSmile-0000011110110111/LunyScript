using Luny.Engine.Bridge;
using LunyScript;

public class Rigidbody_AddImpulse_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var applyForce = GVar.Define("Fire!", false);
		var didFire = GVar.Define("did Fire!", false);

		On.Ready(Object.Enable("WarmUp"));

		Coroutine("warmup timer")
			.In(2)
			.Seconds()
			.WhenElapsed(applyForce.Set(true), Object.Disable("WarmUp"), Object.Enable("Fire"));

		On.Heartbeat(
			If(GVar["Fire!"])
				.Then(Rigidbody.Dynamic.AddImpulse(new LunyVector3(0.02, 1f, 0.1f) * 15),
					didFire.Set(true))
		);

		// disable after firing, since Impulse applies the whole "over time" force all at once (more or less, and rather more than less)
		// resetting the flag in FrameUpdate ensures we fire the impulse once while allowing other objects to fire it in their own Heartbeat
		On.AfterFrameUpdate(If(didFire).Then(applyForce.Set(false), Object.Disable("Fire"), Object.Enable("Done")));
	}
}

public class Rigidbody_AddImpulse_IgnoreMass_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(
		If(GVar["Fire!"]).Then(Rigidbody.Dynamic.AddImpulse(new LunyVector3(0.02, 1f, 0.1f) * 15).IgnoreMass())
	);
}

public class Rigidbody_AddImpulse_AtChildPosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddImpulse(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.AtPosition("AddImpulse_AtPosition_Child"))
	);
}

public class Rigidbody_AddImpulse_IgnoreMass_AtChildPosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddImpulse(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.IgnoreMass()
				.AtPosition("AddImpulse_IgnoreMass_AtPosition_Child"))
	);
}

public class Rigidbody_AddImpulse_AtPosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddImpulse(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.AtPosition(new LunyVector3(-0.45, -0.6, -0.45)))
	);
}

public class Rigidbody_AddImpulse_IgnoreMass_AtPosition_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context) => On.Heartbeat(
		If(GVar["Fire!"])
			.Then(Rigidbody.Dynamic.AddImpulse(new LunyVector3(0.02, 1f, 0.1f) * 15)
				.IgnoreMass()
				.AtPosition(new LunyVector3(-0.45, -0.6, -0.45)))
	);
}
