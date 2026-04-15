using LunyScript;

public class Rigidbody_ToggleKinematic_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var kinematic = Var.Define("is kinematic", false);

		Coroutine("kinematic toggle")
			.Every(.5)
			.Seconds()
			.WhenElapsed(kinematic.Toggle(), Rigidbody.SetKinematic(kinematic));
	}
}

public class Rigidbody_ToggleGravity_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var useGravity = Var.Define("uses gravity", false);

		Coroutine("gravity toggle")
			.Every(.5)
			.Seconds()
			.WhenElapsed(useGravity.Toggle(), Rigidbody.SetUsesGravity(useGravity));
	}
}
