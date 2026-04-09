using Luny;
using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Builder for 'While' loops.
	/// </summary>
	public readonly struct WhileBlockBuilder
	{
		private readonly ConditionBlock[] _conditions;
		private readonly LunyStackTrace _trace;

		internal WhileBlockBuilder(ConditionBlock[] conditions, LunyStackTrace trace)
		{
			_conditions = conditions;
			_trace = trace;
		}

		public ActionBlock Do(params ActionBlock[] blocks) => WhileBlock.Create(_conditions, blocks, _trace);
	}
}
