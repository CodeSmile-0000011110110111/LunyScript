namespace LunyScript.Api
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenGlobalEventBuilder
	{
		private readonly Script _script;
		internal WhenGlobalEventBuilder(Script script) => _script = script;

		public WhenInputEventBuilder Input => new(_script);
		public WhenSceneEventBuilder Scene => new(_script);
	}
}
