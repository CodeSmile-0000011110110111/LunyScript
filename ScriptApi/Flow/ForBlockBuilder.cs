using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Builder for 'For' loops.
	/// </summary>
	public sealed class ForBlockBuilder : ScriptActionBlock
	{
		private readonly Int32 _limit;
		private readonly Int32 _step;
		private ScriptActionBlock[] _blocks;
		private ScriptActionBlock _cachedBlock;

		internal ForBlockBuilder(Int32 limit, Int32 step = 1)
		{
			_limit = limit;
			_step = step;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

		public ScriptActionBlock Do(params ScriptActionBlock[] blocks)
		{
			_blocks = blocks;
			return Build();
		}

		private ScriptActionBlock Build() => ForBlock.Create(_limit, _step, _blocks);
	}
}
