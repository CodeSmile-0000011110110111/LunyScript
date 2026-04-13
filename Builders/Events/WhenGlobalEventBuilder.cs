using Luny;

namespace LunyScript
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenGlobalEventBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal WhenGlobalEventBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		public WhenInputBuilder Input => new(_script, _trace.Add(nameof(Input)));
		public WhenSceneEventBuilder Scene => new(_script, _trace.Add(nameof(Scene)));
	}
}
