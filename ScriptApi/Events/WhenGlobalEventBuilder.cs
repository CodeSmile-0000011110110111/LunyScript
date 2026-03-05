using System;

namespace LunyScript
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenGlobalEventBuilder
	{
		private readonly Script _script;
		internal WhenGlobalEventBuilder(Script script) => _script = script;

		// public WhenInputEventBuilder Input => new(_script);
		public WhenInputActionBuilder InputAction(String actionName) =>
			new(_script, _script.CreateBuilderToken(actionName, "When.InputAction"), actionName);

		public WhenSceneEventBuilder Scene => new(_script);
	}

	/*
	public readonly struct WhenInputEventBuilder
	{
		private readonly Script _script;
		internal WhenInputEventBuilder(Script script) => _script = script;
		private ScriptEventScheduler Scheduler => _script.Scheduler;
	}
	*/
}
