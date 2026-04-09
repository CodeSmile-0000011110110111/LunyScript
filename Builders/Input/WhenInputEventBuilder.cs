using System;

namespace LunyScript
{
	public readonly struct WhenInputEventBuilder
	{
		private readonly Script _script;
		internal WhenInputEventBuilder(Script script) => _script = script;

		/// <summary>
		/// Input Action Map events.
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public WhenInputActionBuilder Action(String actionName) =>
			new(_script, _script.CreateBuilderToken(actionName, "When.Input.Action"), actionName);
	}
}
