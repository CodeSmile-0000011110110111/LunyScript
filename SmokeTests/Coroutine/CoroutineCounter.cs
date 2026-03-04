using System;

namespace LunyScript.SmokeTests.Coroutine
{
	public class CoroutineCounter : Script
	{
		private const Int32 N = 10;

		public override void Build(ScriptContext context)
		{
			// Note: Heartbeats run multiple times per frame and decoupled from frame update, and its frequency
			// is configurable. In Unity: Project Settings / Time / Fixed Timestep (default: 0.02 => 50 Hz)
			// Frame update rate depends on both rendering performance AND the state of VSync/GSync/FreeSync AND
			// the current (monitor's) refresh rate. Thus heartbeat count "every 5 beats" does not equate
			// to a framecount sequence like 1,6,11,16,.. as you might expect.

			var beats = N * 2;

			// Every() => repeating
			var every0 = Coroutine($"Counter EVERY {beats} beats")
				.Every(beats)
				.Heartbeats()
				.WhenPaused(Debug.Log($"Counter EVERY {N} beats PAUSED"))
				.WhenResumed(Debug.Log($"Counter EVERY {N} beats RESUMED"))
				//.WhenProcessed(Debug.Log($"Counter EVERY {N} beats PROCESSED"))
				.WhenElapsed(Debug.Log($"Counter EVERY {beats} beats ELAPSED"));

			var every1 = Coroutine($"Counter EVERY {N} frames")
				.Every(N)
				.Frames()
				.WhenStarted(Debug.Log($"Counter EVERY {N} frames STARTED"))
				.WhenStopped(Debug.Log($"Counter EVERY {N} frames STOPPED"))
				.WhenPaused(Debug.Log($"Counter EVERY {N} frames PAUSED"))
				.WhenResumed(Debug.Log($"Counter EVERY {N} frames RESUMED"))
				//.WhenProcessed(Debug.Log($"Counter EVERY {N} frames PROCESSED"))
				.WhenElapsed(Debug.Log($"Counter EVERY {N} frames ELAPSED"));

			// In() => finite
			var in0 = Coroutine($"Counter IN {beats} beats")
				.In(beats)
				.Heartbeats()
				.WhenElapsed(Debug.Log($"Counter IN {beats} beats ELAPSED"));

			var in1 = Coroutine($"Counter IN {N} frames")
				.In(N)
				.Frames()
				.WhenElapsed(Debug.Log($"Counter IN {N} frames ELAPSED"));

			Coroutine("pause").In(12).Frames().WhenElapsed(every0.Pause(), every1.Pause());
			Coroutine("resume").In(120).Frames().WhenElapsed(every0.Resume(), every1.Resume());

			Coroutine("stop")
				.In(3)
				.Seconds()
				.WhenElapsed(in0.Stop(), in1.Pause(), every0.Stop(), every1.Stop(), Debug.Log("All Counter coroutines stopped."));
		}
	}
}
