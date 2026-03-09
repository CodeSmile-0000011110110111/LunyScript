using System;

namespace LunyScript
{
	public readonly struct OnInputEventBuilder
	{
		private readonly Script _script;
		internal OnInputEventBuilder(Script script) => _script = script;

		public WhenInputActionBuilder Action(String actionName) =>
			new(_script, _script.CreateBuilderToken(actionName, "On.Input.Action"), actionName);
	}
}
