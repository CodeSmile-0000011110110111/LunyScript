namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsSphere : Script
	{
		public override void Build(ScriptContext context) => TimerBuilderStartEx.Do(TimerBuilderStartEx.Seconds(Timer("Sphere: Destroy").In(7.3)), Object.Destroy());
	}
}
