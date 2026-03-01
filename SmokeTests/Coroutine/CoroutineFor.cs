namespace LunyScript.SmokeTests.Coroutines
{
	public class CoroutineFor : Script
	{
		public override void Build(ScriptContext context)
		{
			var forFrames = Coroutine("For n Frames")
				.For(10)
				.Frames()
				.WhenStarted(Debug.Log("For n Frames STARTED"))
				.WhenPaused(Debug.Log("For n Frames PAUSED"))
				.WhenResumed(Debug.Log("For n Frames RESUMED"))
				.WhenStopped(Debug.Log("For n Frames STOPPED"))
				.WhenElapsed(Debug.Log("For n Frames ELAPSED"))
				.Do(Debug.Log("For n Frames Do(every frame)"));

			var forBeats = Coroutine("For n Beats")
				.For(10)
				.Heartbeats()
				.WhenStarted(Debug.Log("For n Beats STARTED"))
				.WhenElapsed(Debug.Log("For n Beats ELAPSED"))
				.Do(Debug.Log("For n Beats Do(every beat)"));

			Counter("for pause").Every(4).Frames().Do(forFrames.Pause());
			Counter("for resume").Every(8).Frames().Do(forFrames.Resume());
			Counter("for stop").In(10).Frames().Do(forFrames.Stop());
			Counter("for start").In(20).Frames().Do(forFrames.Start());
		}
	}
}
