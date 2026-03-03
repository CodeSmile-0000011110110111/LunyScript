using System;

namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineFor : Script
	{
		private const Int32 N = 10;

		public override void Build(ScriptContext context)
		{
			/*var forFrames = Coroutine($"For {N} Frames")
				.For(N)
				.Frames()
				.WhenStarted(Debug.Log($"For {N} Frames STARTED"))
				.WhenStopped(Debug.Log($"For {N} Frames STOPPED"))
				.WhenPaused(Debug.Log($"For {N} Frames PAUSED"))
				.WhenResumed(Debug.Log($"For {N} Frames RESUMED"))
				.WhenElapsed(Debug.Log($"For {N} Frames ELAPSED"))
				.Do(Debug.Log($"For {N} Frames Do('every frame')"));

			var beats = N * 2;
			var forBeats = Coroutine($"For {beats} Beats")
				.For(beats)
				.Heartbeats()
				.WhenStarted(Debug.Log($"For {beats} Beats STARTED"))
				.WhenElapsed(Debug.Log($"For {beats} Beats ELAPSED"));

			Coroutine("for pause").In(3).Frames().Do(forFrames.Pause(), Debug.Log("Paused frame routine"));
			Coroutine("for resume").In(8).Frames().Do(forFrames.Resume(), Debug.Log("Resumed frame routine"));
			Coroutine("for pause2").In(13).Frames().Do(forFrames.Pause(), Debug.Log("Paused frame routine"));
			Coroutine("for resume2").In(18).Frames().Do(forFrames.Resume(), Debug.Log("Resumed frame routine"));
			Coroutine("for pause3").In(35).Frames().Do(forFrames.Pause(), Debug.Log("Paused frame routine"));
			Coroutine("for resume3").In(55).Frames().Do(forFrames.Resume(), Debug.Log("Resumed frame routine"));
			Coroutine("for stop").In(11).Frames().Do(forFrames.Stop(), Debug.Log("Stopped frame routine"));
			Coroutine("for start").In(33).Frames().Do(forFrames.Start(), Debug.Log("Started frame routine"));*/
		}
	}
}
