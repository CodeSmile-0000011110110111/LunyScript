using Luny.Engine.Bridge;
using LunyScript;

public class Rigidbody_AddAngularForce_SmokeTest : Script
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
			If(GVar["Fire!"]).Then(Rigidbody.Dynamic.AddAngularForce(new LunyVector3(1f, 0f, -0.1) * 35))
		);
	}
}

public class Rigidbody_AddAngularForce_IgnoreMass_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"]).Then(Rigidbody.Dynamic.AddAngularForce(new LunyVector3(1f, 0f, -0.1) * 35).IgnoreMass())
	);
}

public class Rigidbody_AddAngularImpulse_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"] && !Var["did fire"])
			.Then(Var["did fire"].Set(true),
				Rigidbody.Dynamic.AddAngularImpulse(new LunyVector3(1f, 0f, -0.1) * 35).IgnoreMass())
	);
}

public class Rigidbody_AddAngularImpulse_IgnoreMass_SmokeTest : Script
{
	public override void Build() => On.Heartbeat(
		If(GVar["Fire!"] && !Var["did fire"])
			.Then(Var["did fire"].Set(true),
				Rigidbody.Dynamic.AddAngularImpulse(new LunyVector3(1f, 0f, -0.1) * 35).IgnoreMass())
	);
}
