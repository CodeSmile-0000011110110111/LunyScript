using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Builder for 'For' loops.
	/// </summary>
	public readonly struct ForBlockBuilder
	{
		private readonly VariableBlock _limit;
		private readonly VariableBlock _step;

		internal ForBlockBuilder(VariableBlock limit)
			: this(limit, 1) {}

		internal ForBlockBuilder(VariableBlock limit, VariableBlock step)
		{
			_limit = limit;
			_step = step;
		}

		public ActionBlock Do(params ActionBlock[] blocks) => ForBlock.Create(_limit, _step, blocks);
	}
}
