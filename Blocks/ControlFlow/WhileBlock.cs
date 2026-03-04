using LunyScript.Exceptions;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// While loop execution block with safety limits.
	/// </summary>
	internal sealed class WhileBlock : ScriptActionBlock
	{
		private readonly ScriptConditionBlock[] _conditions;
		private readonly ScriptActionBlock[] _blocks;

		public static WhileBlock Create(ScriptConditionBlock[] conditions, ScriptActionBlock[] blocks) => new(conditions, blocks);

		private WhileBlock(ScriptConditionBlock[] conditions, ScriptActionBlock[] blocks)
		{
			_conditions = conditions;
			_blocks = blocks;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
#endif

			var limit = ScriptEngine.MaxLoopIterations;

			while (EvaluateAll(runtimeContext))
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > limit)
					throw new LunyScriptMaxIterationException(nameof(WhileBlock), limit);
#endif
				ExecuteAll(runtimeContext);
			}
		}

		private Boolean EvaluateAll(IScriptRuntimeContext runtimeContext)
		{
			if (_conditions == null || _conditions.Length == 0)
				return false; // Infinite loop prevention if no conditions

			foreach (var condition in _conditions)
			{
				if (!condition.Evaluate(runtimeContext))
					return false;
			}

			return true;
		}

		private void ExecuteAll(IScriptRuntimeContext runtimeContext)
		{
			if (_blocks == null)
				return;

			foreach (var block in _blocks)
				block.Execute(runtimeContext);
		}
	}
}
