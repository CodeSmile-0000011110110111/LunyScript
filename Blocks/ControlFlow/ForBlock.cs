using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// For loop execution block with 1-based indexing and safety limits.
	/// </summary>
	internal sealed class ForBlock : ScriptActionBlock
	{
		private readonly Int32 _limit;
		private readonly Int32 _step;
		private readonly ScriptActionBlock[] _actions;

		public static ForBlock Create(Int32 limit, Int32 step, ScriptActionBlock[] actions) => new(limit, step, actions);

		private ForBlock(Int32 limit, Int32 step, ScriptActionBlock[] actions)
		{
			_limit = limit;
			_step = step == 0 ? 1 : step; // Prevent division by zero/infinite loop if step is 0
			_actions = actions;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
			var maxLimit = ScriptEngine.MaxLoopIterations;
#endif

			var start = _step > 0 ? 1 : _limit;
			var end = _step > 0 ? _limit : 1;
			for (var i = start; _step > 0 ? i <= end : i >= end; i += _step)
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > maxLimit)
					throw new LunyScriptMaxIterationException(runtimeContext, nameof(ForBlock), maxLimit);
#endif

				ControlFlow.ExecuteAll(runtimeContext, _actions);
			}
		}
	}
}
