namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineTimer : Script
	{
		public override void Build(ScriptContext context)
		{
			var timer = Coroutine("Coroutine.Every.{Unit}")
				.Every(3)
				.Milliseconds()
				.WhenStarted(Debug.Log("Every Timer STARTED"))
				.WhenStopped(Debug.Log("Every Timer STOPPED"))
				.WhenPaused(Debug.Log("Every Timer PAUSED"))
				.WhenResumed(Debug.Log("Every Timer RESUMED"))
				.Do(Debug.Log("Every Timer ELAPSED"));
			timer.TimeScale(0.1);

			Coroutine("Coroutine.In.{Unit}")
				.In(0.98765)
				.Seconds()
				.WhenStarted(Debug.Log("In Timer STARTED"))
				.Do(Debug.Log("In Timer ELAPSED"));

			//Timer("se").In(4).Seconds().WhenPaused(Debug.Log("In Timer PAUSED")).Do(Debug.Log("In Timer DO"));

			Counter("stop").In(30).Frames().Do(timer.Stop(), Debug.Log("Stopped timer coroutine"));
		}
	}
}
