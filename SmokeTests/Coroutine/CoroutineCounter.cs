using System;

namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineCounter : Script
	{
		public override void Build(ScriptContext context)
		{
			const Int32 n = 5;

			// Counts frames or heartbeats:
			// In() => once-only
			// Every() => repeating
			var in0 = Counter("counter in beats").In(n).Heartbeats().Do(Debug.Log($"Counter IN {n} beats"));
			var in1 = Counter("counter in frames").In(n).Frames().Do(Debug.Log($"Counter IN {n} frames"));
			var every0 = Counter("counter every beats").Every(n).Heartbeats().Do(Debug.Log($"Counter EVERY {n} beats"));
			var every1 = Counter("counter every frames").Every(n).Frames().Do(Debug.Log($"Counter EVERY {n} frames"));

			Counter("stop").In(60).Frames().Do(in0.Stop(), in1.Pause(), every0.Stop(), every1.Pause(), Debug.Log("All counters stopped."));

			// Example output
			// Note: Heartbeats run multiple times per frame and decoupled from frame update, and its frequency
			// is configurable. In Unity: Project Settings / Time / Fixed Timestep (default: 0.02 => 50 Hz)
			// Frame update rate depends on both rendering performance AND the state of VSync/GSync/FreeSync AND
			// the current (monitor's) refresh rate. Thus heartbeat count "every 5 beats" does not equate
			// to a framecount sequence like 1,6,11,16,.. as you might expect.
			/*
			[3] [DebugLogInfoBlock] IN 5 beats (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[3] [DebugLogInfoBlock] EVERY 5 beats (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[6] [DebugLogInfoBlock] IN 5 frames (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[6] [DebugLogInfoBlock] EVERY 5 frames (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[12] [DebugLogInfoBlock] EVERY 5 frames (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[18] [DebugLogInfoBlock] EVERY 5 frames (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[20] [DebugLogInfoBlock] EVERY 5 beats (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[24] [DebugLogInfoBlock] EVERY 5 frames (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			[30] [DebugLogInfoBlock] EVERY 5 frames (LunyScriptID:15 -> ☑ CoroutineCounter (LunyObjectID:3, LunyNativeObjectID:-30282))
			 */

		}
	}
}
