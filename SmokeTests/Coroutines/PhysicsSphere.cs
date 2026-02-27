using LunyScript.Api.Coroutine;

namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsSphere : Script
	{
		public override void Build(ScriptContext context) => Timer("Sphere: Destroy").In(7.3).Seconds().Do(Object.Destroy());
	}
}
