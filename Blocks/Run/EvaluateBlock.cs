using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Evaluates a custom method or lambda (Func returning Boolean).
	/// Useful for quick tests and one-off conditions.
	/// </summary>
	internal sealed class EvaluateBlock : ScriptConditionBlock
	{
		private readonly Func<IScriptRuntimeContext, Boolean> _func;

		public static ScriptConditionBlock Create(Func<IScriptRuntimeContext, Boolean> func) => new EvaluateBlock(func);

		private EvaluateBlock(Func<IScriptRuntimeContext, Boolean> func) => _func = func ?? throw new ArgumentNullException(nameof(func));

		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => _func(runtimeContext);
	}
}
