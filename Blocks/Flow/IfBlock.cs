using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for constructing 'If' blocks with 'ElseIf' and 'Else' branches.
	/// </summary>
	public sealed class IfBlockBuilder : ScriptActionBlock
	{
		private readonly List<(ScriptConditionBlock[] conditions, ScriptActionBlock[] blocks)> _branches = new();
		private ScriptActionBlock[] _elseBlocks;
		private ScriptActionBlock _cachedBlock;

		internal IfBlockBuilder(ScriptConditionBlock[] conditions) => _branches.Add((conditions, Array.Empty<ScriptActionBlock>()));

		protected internal  override void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

		public IfBlockBuilder Then(params ScriptActionBlock[] blocks)
		{
			var lastIndex = _branches.Count - 1;
			_branches[lastIndex] = (_branches[lastIndex].conditions, blocks);
			return this;
		}

		public IfBlockBuilder ElseIf(params ScriptConditionBlock[] conditions)
		{
			_branches.Add((conditions, Array.Empty<ScriptActionBlock>()));
			return this;
		}

		public ScriptActionBlock Else(params ScriptActionBlock[] blocks)
		{
			_elseBlocks = blocks;
			return Build();
		}

		private ScriptActionBlock Build() => IfBlock.Create(_branches, _elseBlocks);
	}

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

		protected internal  override void Execute(IScriptRuntimeContext runtimeContext)
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
