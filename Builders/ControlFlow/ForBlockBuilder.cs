using Luny;
using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Builder for 'For' loops.
	/// </summary>
	public readonly struct ForBlockBuilder
	{
		private readonly VariableBlock _limit;
		private readonly VariableBlock _step;
		private readonly LunyStackTrace _trace;

		internal ForBlockBuilder(VariableBlock limit, LunyStackTrace trace)
			: this(limit, 1, trace) {}

		internal ForBlockBuilder(VariableBlock limit, VariableBlock step, LunyStackTrace trace)
		{
			_limit = limit;
			_step = step;
			_trace = trace;
		}

		public ActionBlock Do(params ActionBlock[] blocks) => ForBlock.Create(_limit, _step, blocks, _trace);
	}
}
