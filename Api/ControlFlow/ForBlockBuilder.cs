using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Builder for 'For' loops.
	/// </summary>
	public readonly struct ForBlockBuilder
	{
		private readonly Int32 _limit;
		private readonly Int32 _step;

		internal ForBlockBuilder(Int32 limit, Int32 step = 1)
		{
			_limit = limit;
			_step = step;
		}

		public ScriptActionBlock Do(params ScriptActionBlock[] blocks) => ForBlock.Create(_limit, _step, blocks);
	}
}
