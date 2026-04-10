using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom method or lambda (System.Action).
	/// Useful for quick tests and one-off logic.
	/// Prefer writing custom IBlock implementations for cleaner code and best reusability.
	/// </summary>
	internal sealed class RunBlock : ActionBlock
	{
		private readonly String _name;
		private readonly Action<IScriptRuntimeContext> _action;

		/// <summary>
		/// Usage: `var block = (Action&lt;IScriptRuntimeContext&gt;)(ctx => { /* code here */ });`
		/// </summary>
		public static implicit operator RunBlock(Action<IScriptRuntimeContext> action) => new(nameof(Script.Run), action, null);

		public static ActionBlock Create(String name, Action<IScriptRuntimeContext> action, LunyStackTrace trace = null) =>
			new RunBlock(name, action, trace);

		private RunBlock(String name, Action<IScriptRuntimeContext> action, LunyStackTrace trace)
			: base(trace)
		{
			_name = !String.IsNullOrEmpty(name) ? $"<i>{name}</i>" : Emoji.NotFound;
			_action = action ?? throw new ArgumentNullException(nameof(action));
		}

		protected internal override void Execute(IScriptRuntimeContext context) => _action(context);

		public override String ToString() => _name;
	}
}
