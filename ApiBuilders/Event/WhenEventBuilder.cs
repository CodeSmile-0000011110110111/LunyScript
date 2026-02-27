namespace LunyScript.ApiBuilders.Event
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenEventBuilder
	{
		private readonly Script _script;
		internal WhenEventBuilder(Script script) => _script = script;

		public WhenInputEventBuilder Input => new(_script);
		public WhenSceneEventBuilder Scene => new(_script);
	}
}
