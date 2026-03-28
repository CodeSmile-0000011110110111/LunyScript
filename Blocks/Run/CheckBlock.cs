using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Evaluates a custom method or lambda (Func returning Boolean).
	/// </summary>
	internal sealed class CheckBlock : ConditionBlock
	{
		private readonly Func<IScriptRuntimeContext, Boolean> _func;

		public static ConditionBlock Create(Func<IScriptRuntimeContext, Boolean> func, StackTrace trace = null) => new CheckBlock(func, trace);

		private CheckBlock(Func<IScriptRuntimeContext, Boolean> func, StackTrace trace) : base(trace)
		{
			_func = func ?? throw new ArgumentNullException(nameof(func));
		}

		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => _func(runtimeContext);
	}
}
