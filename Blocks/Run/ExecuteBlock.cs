using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom method or lambda (System.Action).
	/// Useful for quick tests and one-off logic.
	/// Prefer writing custom IBlock implementations for cleaner code and best reusability.
	/// </summary>
	internal sealed class ExecuteBlock : ScriptActionBlock
	{
		private readonly Action<IScriptRuntimeContext> _action;

		/// <summary>
		/// Usage: ScriptActionBlock block = (Action<IScriptRuntimeContext>)(ctx => { ... });
		/// </summary>
		public static implicit operator ExecuteBlock(Action<IScriptRuntimeContext> action) => new(action);

		public static ScriptActionBlock Create(Action<IScriptRuntimeContext> action) => new ExecuteBlock(action);

		private ExecuteBlock(Action<IScriptRuntimeContext> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _action(runtimeContext);
	}
}
