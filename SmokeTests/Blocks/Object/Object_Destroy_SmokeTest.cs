using LunyScript;

public class Object_Destroy_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		On.Ready(Object.Destroy("Object_Destroyed_OnLaunch"));

		var destroyName = "self destructing ...";
		var createDestroyRoutine = Coroutine("create/destroy")
			.In(500)
			.Milliseconds()
			.WhenStarted(Object.Create(destroyName), Object.Create(destroyName))
			.WhenElapsed(Object.Destroy(destroyName), Object.Destroy(destroyName),
				Object.Create(nameof(Object_DestroySelf_SmokeTest)));

		Coroutine("lifecyle").Every(1000).Milliseconds().WhenElapsed(createDestroyRoutine.Start());
	}
}

public class Object_DestroySelf_SmokeTest : Script
{
	// destroys itself immediately
	public override void Build(ScriptBuildContext context) => On.Created(Object.Destroy());
}
