using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Conditional execution block.
	/// </summary>
	internal sealed class IfBlock : ScriptActionBlock
	{
		private readonly List<(ScriptConditionBlock[] conditions, ScriptActionBlock[] blocks)> _branches;
		private readonly ScriptActionBlock[] _elseBlocks;

		public static IfBlock Create(List<(ScriptConditionBlock[] conditions, ScriptActionBlock[] blocks)> branches,
			ScriptActionBlock[] elseBlocks) => new(branches, elseBlocks);

		private IfBlock(List<(ScriptConditionBlock[] conditions, ScriptActionBlock[] blocks)> branches, ScriptActionBlock[] elseBlocks)
		{
			_branches = branches;
			_elseBlocks = elseBlocks;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			foreach (var (conditions, blocks) in _branches)
			{
				if (EvaluateAll(runtimeContext, conditions))
				{
					ExecuteAll(runtimeContext, blocks);
					return;
				}
			}

			if (_elseBlocks != null)
				ExecuteAll(runtimeContext, _elseBlocks);
		}

		private Boolean EvaluateAll(IScriptRuntimeContext runtimeContext, ScriptConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				return true;

			foreach (var condition in conditions)
			{
				if (condition == null || !condition.Evaluate(runtimeContext))
					return false;
			}

			return true;
		}

		private void ExecuteAll(IScriptRuntimeContext runtimeContext, ScriptActionBlock[] blocks)
		{
			if (blocks == null)
				return;

			foreach (var block in blocks)
				block.Execute(runtimeContext);
		}
	}
}
