using LunyScript.Blocks;

namespace LunyScript.Api
{
	/// <summary>
	/// Builder for 'While' loops.
	/// </summary>
	public readonly struct WhileBlockBuilder
	{
		private readonly ConditionBlock[] _conditions;

		internal WhileBlockBuilder(ConditionBlock[] conditions) => _conditions = conditions;

		public ActionBlock Do(params ActionBlock[] blocks) => WhileBlock.Create(_conditions, blocks);
	}
}
