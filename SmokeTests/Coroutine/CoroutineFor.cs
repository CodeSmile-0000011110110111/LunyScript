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
				.WhenStopped(Debug.Log("For n Frames STOPPED"))
				.WhenPaused(Debug.Log("For n Frames PAUSED"))
				.WhenResumed(Debug.Log("For n Frames RESUMED"))
				.WhenElapsed(Debug.Log("For n Frames ELAPSED"))
				.Do(Debug.Log("For n Frames Do(every frame)"));

			var forBeats = Coroutine("For n Beats")
				.For(10)
				.Heartbeats()
				.WhenStarted(Debug.Log("For n Beats STARTED"))
				.WhenElapsed(Debug.Log("For n Beats ELAPSED"))
				.Do(Debug.Log("For n Beats Do(every beat)"));

			Counter("for pause").In(3).Frames().Do(forFrames.Pause(), Debug.Log("Paused frame routine"));
			Counter("for resume").In(8).Frames().Do(forFrames.Resume(), Debug.Log("Resumed frame routine"));
			Counter("for pause2").In(13).Frames().Do(forFrames.Pause(), Debug.Log("Paused frame routine"));
			Counter("for resume2").In(18).Frames().Do(forFrames.Resume(), Debug.Log("Resumed frame routine"));
			Counter("for pause3").In(35).Frames().Do(forFrames.Pause(), Debug.Log("Paused frame routine"));
			Counter("for resume3").In(55).Frames().Do(forFrames.Resume(), Debug.Log("Resumed frame routine"));
			Counter("for stop").In(11).Frames().Do(forFrames.Stop(), Debug.Log("Stopped frame routine"));
			Counter("for start").In(33).Frames().Do(forFrames.Start(), Debug.Log("Started frame routine"));
		}
	}
}
