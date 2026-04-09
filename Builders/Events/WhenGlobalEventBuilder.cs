using Luny;
namespace LunyScript
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenGlobalEventBuilder
	{
		private readonly Script _script;
		private readonly StackTrace _trace;
		internal WhenGlobalEventBuilder(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		public WhenInputEventBuilder Input => new(_script);
		public WhenSceneEventBuilder Scene => new(_script);
	}
}
