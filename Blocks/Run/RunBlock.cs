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
		private readonly Action<IScriptRuntimeContext> _action;

		/// <summary>
		/// Usage: `var block = (Action&lt;IScriptRuntimeContext&gt;)(ctx => { /* code here */ });`
		/// </summary>
		public static implicit operator RunBlock(Action<IScriptRuntimeContext> action) => new(action, null);

		public static ActionBlock Create(Action<IScriptRuntimeContext> action, StackTrace trace = null) => new RunBlock(action, trace);

		private RunBlock(Action<IScriptRuntimeContext> action, StackTrace trace) : base(trace)
		{
			_action = action ?? throw new ArgumentNullException(nameof(action));
		}

		protected internal override void Execute(IScriptRuntimeContext context) => _action(context);
	}
}
