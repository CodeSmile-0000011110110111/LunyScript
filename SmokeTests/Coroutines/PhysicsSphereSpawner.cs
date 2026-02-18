namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsSphereSpawner : Script
	{
		public override void Build(ScriptContext context)
		{
			var instantiate = Prefab.Instantiate("Prefabs/PhysicsSphere");
			var log = Debug.LogInfo("SPAWN SPHERE");
			On.Ready(log, instantiate);

			var createCounter = Counter("Sphere: Create").Every(50).Heartbeats().Do(log, instantiate);
			Counter("SphereSpawner: Destroy").In(50).Heartbeats().Do(createCounter.Stop());

			Timer("RELOAD").In(8).Seconds().Do(Scene.Reload());
		}
	}
}
