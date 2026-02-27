namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsCube : Script
	{
		public override void Build(ScriptContext context) => TimerBuilderStartEx.Do(TimerBuilderStartEx.Seconds(Timer("Cube: Destroy").In(5.5)), Object.Destroy());
	}
}
