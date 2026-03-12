using LunyScript.Api;
using System;

namespace LunyScript.SmokeTests
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

			var cubletContainer = "Cublets";
			var cubletPath = "Packages/de.codesmile.lunyscript/LunyScript.Unity/SmokeTests/Prefabs/Cublet";
			var cubletSpawnRate = 2;

			On.Ready(Object.Create(cubletContainer));

			Coroutine("Counter: spawn")
				.Every(cubletSpawnRate)
				.Frames()
				.WhenElapsed(Object.Create("Cublet").With(cubletPath).LocalPosition(1, 1, 1).Parent(cubletContainer));

			var createMegaCublet = Object.Create("Mega-Cublet")
				.With(cubletPath)
				.LocalScale(0.3)
				.LocalPosition(1, 1, 1)
				.LocalRotation(-45, -45, -45);

			var destroyCoroutine = Coroutine("Counter: destroy all cubelets")
				.In(1000)
				.Heartbeats()
				.WhenStarted(createMegaCublet)
				.WhenElapsed(Object.Destroy(cubletContainer), Object.Create(cubletContainer),
					Object.Disable("Directional Light"), Object.Enable("Moon"));

			Coroutine("Counter: restart")
				.Every(1234)
				.Heartbeats()
				.WhenElapsed(destroyCoroutine.Start(), Object.Enable("Directional Light"), Object.Disable("Moon"));

			Coroutine("Timer: destroy")
				.Every(cubletSpawnRate * 2 + 1)
				.Frames()
				.WhenElapsed(Object.Destroy("Cublet"), Object.Destroy("Cublet"));

			Coroutine("Timer: spawn more Mega-Cublets")
				.Every(2)
				.Seconds()
				.WhenElapsed(For(3).Do(createMegaCublet));

			var destroyMegaCublet = Object.Destroy("Mega-Cublet");
			Coroutine("Timer: destroy some Mega-Cublets")
				.Every(1)
				.Minutes()
				.WhenElapsed(For(80).Do(destroyMegaCublet));

			var tictoc = Var.Define("tictoc", false);
			var tic = Coroutine("Timer: tic-toc tic");
			var toc = Coroutine("Timer: tic-toc toc");
			var ticBlock = tic.In(1).Seconds().WhenStarted(Object.Enable("Tic")).WhenElapsed(Object.Disable("Tic"));
			var tocBlock = toc.In(1)
				.Seconds()
				.WhenStarted(Object.Enable("Toc"))
				.WhenStopped(Object.Disable("Toc"))
				.WhenElapsed(Object.Disable("Toc"));
			// TODO: current necessity since we can't yet use "tocBlock" in "ticBlock" or vice versa
			Coroutine("TicHelper")
				.Every(1)
				.Seconds()
				.WhenElapsed(
					If(tictoc == 0)
						.Then(tictoc.Inc(), tocBlock.Start())
						.ElseIf(tictoc == 1)
						.Then(tictoc.Dec(), ticBlock.Start())
				);

			On.Ready(tocBlock.Stop());

			// Non-visual tests ...
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
