namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsCubeSpawner : Script
	{
		public override void Build(ScriptContext context)
		{
			var instantiate = Prefab.Instantiate("Prefabs/PhysicsCube");
			var log = Debug.LogInfo("SPAWN CUBE");

			Counter("Cube: Create").Every(70).Heartbeats().Do(log, instantiate);
			Counter("CubeSpawner: Destroy").In(150).Heartbeats().Do(Object.Destroy());
		}
	}
}
