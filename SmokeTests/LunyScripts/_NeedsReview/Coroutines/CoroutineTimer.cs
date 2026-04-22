using System;

namespace LunyScript.SmokeTests
{
	public class CoroutineTimer : Script
	{
		private const Int32 N = 40;

		public override void Build()
		{
			// Every() => repeating
			var timer = Coroutine("Coroutine.Every.{Unit}")
				.Every(N)
				.Milliseconds()
				.WhenStarted(Debug.Log($"Timer EVERY {N} ms STARTED"))
				.WhenStopped(Debug.Log($"Timer EVERY {N} ms STOPPED"))
				.WhenPaused(Debug.Log($"Timer EVERY {N} ms PAUSED"))
				.WhenResumed(Debug.Log($"Timer EVERY {N} ms RESUMED"))
				//.WhenProcessed(Debug.Log($"Timer EVERY {N} ms PROCESSED"))
				.WhenElapsed(Debug.Log($"Timer EVERY {N} ms ELAPSED"));
			timer.TimeScale(0.1);

			// In() => finite
			var finite = N / 20;
			Coroutine("Coroutine.In.{Unit}")
				.In(finite)
				.Seconds()
				.WhenStarted(Debug.Log($"Timer IN {finite} s STARTED"))
				.WhenElapsed(Debug.Log($"Timer IN {finite} s ELAPSED"));

			Coroutine("pause").In(12).Frames().WhenElapsed(timer.Pause());
			Coroutine("resume").In(120).Frames().WhenElapsed(timer.Resume());

			Coroutine("stop").In(3).Seconds().WhenElapsed(timer.Stop(), Debug.Log("All Timer coroutines stopped."));
		}
	}
}
