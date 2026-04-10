using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Evaluates a custom method or lambda (Func returning Boolean).
	/// </summary>
	internal sealed class CheckBlock : ConditionBlock
	{
		private readonly String _name;
		private readonly Func<IScriptRuntimeContext, Boolean> _func;

		public static ConditionBlock Create(String name, Func<IScriptRuntimeContext, Boolean> func, LunyStackTrace trace = null) =>
			new CheckBlock(name, func, trace);

		private CheckBlock(String name, Func<IScriptRuntimeContext, Boolean> func, LunyStackTrace trace)
			: base(trace)
		{
			_name = !String.IsNullOrEmpty(name) ? $"<i>{name}</i>" : Emoji.NotFound;
			_func = func ?? throw new ArgumentNullException(nameof(func));
		}

		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => _func(runtimeContext);

		public override String ToString() => _name;
	}
}
