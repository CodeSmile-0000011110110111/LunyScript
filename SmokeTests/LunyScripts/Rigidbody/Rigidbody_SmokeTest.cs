using LunyScript;

public class Rigidbody_ToggleKinematic_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var kinematic = Var.Define("is kinematic", false);

		Coroutine("kinematic toggle")
			.Every(.4)
			.Seconds()
			.WhenElapsed(kinematic.Toggle(), Rigidbody.SetKinematic(kinematic),
				If(kinematic)
					.Then(Object.Enable("On-On-On-On-And-On"), Object.Disable("Off"))
					.Else(Object.Disable("On-On-On-On-And-On"), Object.Enable("Off"))
			);
	}
}

public class Rigidbody_ToggleGravity_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var useGravity = Var.Define("uses gravity", false);

		Coroutine("gravity toggle")
			.Every(.4)
			.Seconds()
			.WhenElapsed(useGravity.Toggle(), Rigidbody.SetUsesGravity(useGravity));
	}
}
