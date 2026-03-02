namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineTimer : Script
	{
		public override void Build(ScriptContext context)
		{
			var timer = Timer("tic toc")
				.Every(3)
				.Milliseconds()
				.WhenStarted(Debug.Log("tic toc STARTED"))
				.WhenStopped(Debug.Log("tic toc STOPPED"))
				.WhenPaused(Debug.Log("tic toc PAUSED"))
				.WhenResumed(Debug.Log("tic toc RESUMED"))
				.WhenElapsed(Debug.Log("tic toc ELAPSED"))
				.Do(Debug.Log("Timer Every UPDATE"));
			timer.TimeScale(0.1);

			Counter("stop").In(30).Frames().Do(timer.Stop(), Debug.Log("Stopped Every(30 ms = 3/0.1) timer"));
		}
	}
}
