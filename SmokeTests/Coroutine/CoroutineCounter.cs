using System;

namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineCounter : Script
	{
		private const Int32 N = 5;

		public override void Build(ScriptContext context)
		{
			// Note: Heartbeats run multiple times per frame and decoupled from frame update, and its frequency
			// is configurable. In Unity: Project Settings / Time / Fixed Timestep (default: 0.02 => 50 Hz)
			// Frame update rate depends on both rendering performance AND the state of VSync/GSync/FreeSync AND
			// the current (monitor's) refresh rate. Thus heartbeat count "every 5 beats" does not equate
			// to a framecount sequence like 1,6,11,16,.. as you might expect.

			// Every() => repeating
			var every0 = Counter("Counter EVERY beats").Every(N).Heartbeats().Do(Debug.Log($"Counter EVERY {N} beats"));
			var every1 = Counter("Counter EVERY frames")
				.Every(N)
				.Frames()
				.WhenStarted(Debug.Log("Counter EVERY frames STARTED"))
				.WhenStopped(Debug.Log("Counter EVERY frames STOPPED"))
				.WhenPaused(Debug.Log("Counter EVERY frames PAUSED"))
				.WhenResumed(Debug.Log("Counter EVERY frames RESUMED"))
				.Do(Debug.Log($"Counter EVERY {N} frames"));

			// In() => once-only
			var in0 = Counter("Counter IN beats").In(N).Heartbeats().Do(Debug.Log($"Counter IN {N} beats"));
			var in1 = Counter("Counter IN frames").In(N).Frames().Do(Debug.Log($"Counter IN {N} frames"));

			Counter("pause").In(10).Frames().Do(every1.Pause());
			Counter("resume").In(40).Frames().Do(every1.Resume());

			Counter("stop")
				.In(60)
				.Frames()
				.Do(in0.Stop(), in1.Pause(), every0.Stop(), every1.Stop(), Debug.Log("All counters stopped."));
		}
	}
}
