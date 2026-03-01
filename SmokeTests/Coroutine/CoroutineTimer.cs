using LunyScript;

namespace LunyScript.SmokeTests.Coroutines
{
	public partial class CoroutineTimer : Script
	{
		public override void Build(ScriptContext context)
		{
			var timeScaled = Timer("tic toc").Every(100).Milliseconds().Do(Debug.Log("Timer every second (100 ms at 10% time scale)")).TimeScale(0.1);
			var slow = Timer("three seconds").In(3).Seconds().Do(Debug.Log("Timer in three seconds"));
		}
	}
}
