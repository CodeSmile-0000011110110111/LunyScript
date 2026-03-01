namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineTimer : Script
	{
		public override void Build(ScriptContext context)
		{
			var timeScaled = Timer("tic toc")
				.Every(100)
				.Milliseconds()
				.WhenStarted(Debug.Log("tic toc STARTED"))
				.WhenStopped(Debug.Log("tic toc STOPPED"))
				.WhenPaused(Debug.Log("tic toc PAUSED"))
				.WhenResumed(Debug.Log("tic toc RESUMED"))
				.WhenElapsed(Debug.Log("tic toc ELAPSED"))
				.Do(Debug.Log("Timer Every UPDATE"));
			timeScaled.TimeScale(0.1);

			Counter("stop").In(20).Frames().Do(timeScaled.Stop(), Debug.Log("Stopped Every(100/0.1) timer"));
		}
	}
}
