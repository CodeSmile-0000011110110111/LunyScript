using LunyScript;

namespace LunyScript.SmokeTests.Coroutines
{
	public partial class CoroutineFor : Script
	{
		public override void Build(ScriptContext context)
		{
			var forRoutine = Coroutine("for")
				.For(10)
				.Frames()
				.OnFrameUpdate(Debug.Log("for UPDATE"))
				.WhenStarted(Debug.Log("for STARTED"))
				.WhenPaused(Debug.Log("for PAUSED"))
				.WhenResumed(Debug.Log("for RESUMED"))
				.WhenStopped(Debug.Log("for STOPPED"))
				.WhenElapsed(Debug.Log("for ELAPSED"));

			Counter("for pause").Every(4).Frames().Do(forRoutine.Pause());
			Counter("for resume").Every(8).Frames().Do(forRoutine.Resume());
			Counter("for stop").In(10).Frames().Do(forRoutine.Stop());
			Counter("for start").In(20).Frames().Do(forRoutine.Start());
		}
	}
}
