using LunyScript.Events;

namespace LunyScript.ApiBuilders.Event
{
	public readonly struct WhenInputEventBuilder
	{
		private readonly Script _script;
		internal WhenInputEventBuilder(Script script) => _script = script;
		private ScriptEventScheduler Scheduler => _script.Scheduler;
	}
}
