using System;

namespace LunyScript.Api
{
	public readonly struct WhenInputEventBuilder
	{
		private readonly Script _script;
		internal WhenInputEventBuilder(Script script) => _script = script;

		public WhenInputActionBuilder Action(String actionName) =>
			new(_script, _script.CreateBuilderToken(actionName, "When.Input.Action"), actionName);
	}
}
