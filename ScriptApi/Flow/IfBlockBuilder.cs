using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript
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

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

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
}
