using Luny;
using System;
namespace LunyScript
{
	public readonly struct WhenInputEventBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;
		internal WhenInputEventBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary>
		/// Input Action Map events.
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public WhenInputActionBuilder Action(String actionName) =>
			new(_script, _script.CreateBuilderToken(actionName, "When.Input.Action"), actionName, _trace.Add(nameof(Action)), nameof(Action));
	}
}
