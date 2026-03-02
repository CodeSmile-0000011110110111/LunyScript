namespace LunyScript.SmokeTests.Coroutines
{
	public class Coroutine : Script
	{
		public override void Build(ScriptContext context)
		{
			// var updateRoutine = Coroutine("coroutine on frame update").OnFrameUpdate(Debug.Log("frame update coroutine"));
			// var beatRoutine = Coroutine("coroutine on hearbeat").OnHeartbeat(Debug.Log("heartbeat coroutine"));

			/*On.Ready(updateRoutine.Stop(), beatRoutine.Stop());

			Counter("start unbounded routines")
				.In(20)
				.Frames()
				.Do(updateRoutine.Start(), beatRoutine.Start(), Debug.Log("Started unbounded coroutines"));
			Counter("stop unbounded routines")
				.In(60)
				.Frames()
				.Do(updateRoutine.Stop(), beatRoutine.Stop(), Debug.Log("Stopped unbounded coroutines"));*/
		}
	}
}
