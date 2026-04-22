using LunyScript;

public class Object_Destroy_SmokeTest : Script
{
	public override void Build()
	{
		On.Ready(Object.Destroy("Object_Destroyed_OnLaunch"));

		var destroyName = "self destructing ...";
		var createDestroyRoutine = Coroutine("create/destroy")
			.In(500)
			.Milliseconds()
			.WhenStarted(Object.Create(destroyName))
			.WhenProcessing(
				Object.Destroy(destroyName), Object.Create(destroyName), // destroy first (may not exist), then create
				Object.Create(destroyName), Object.Destroy(destroyName), // create and immediately destroy again
				Object.Create(nameof(Object_DestroySelf_SmokeTest)) // created object builds script, then destroys itself immediately
			)
			.WhenElapsed(Object.Destroy(destroyName));

		Coroutine("lifecyle").Every(1000).Milliseconds().WhenElapsed(createDestroyRoutine.Start());
	}
}

public class Object_DestroySelf_SmokeTest : Script
{
	// destroys itself immediately
	public override void Build() => On.Created(Object.Destroy());
}
