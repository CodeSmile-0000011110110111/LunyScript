using LunyScript.Api.Coroutine;

namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsCube : Script
	{
		public override void Build(ScriptContext context) => Timer("Cube: Destroy").In(5.5).Seconds().Do(Object.Destroy());
	}
}
