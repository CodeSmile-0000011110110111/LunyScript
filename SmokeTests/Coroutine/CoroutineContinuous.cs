using LunyScript;

namespace LunyScript.SmokeTests.Coroutines
{
	public partial class CoroutineContinuous : Script
	{
		public override void Build(ScriptContext context)
		{
			//var beat0 = Coroutine("beats").OnHeartbeat(Debug.Log("beat")).Do(Debug.Log("beat Do()"));

			var updateRoutine = Coroutine("coroutine on frame update").OnFrameUpdate(Debug.Log("frame update coroutine")).Do();
			var beatRoutine = Coroutine("coroutine on hearbeat").OnHeartbeat(Debug.Log("heartbeat coroutine")).Do();
			On.Created(updateRoutine.Stop(), beatRoutine.Stop());
			Counter("start unbounded routines").In(70).Frames().Do(updateRoutine.Start(), beatRoutine.Start());
			Counter("stop unbounded routines").In(85).Frames().Do(updateRoutine.Stop(), beatRoutine.Stop());

		}
	}
}
