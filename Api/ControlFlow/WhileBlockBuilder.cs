using LunyScript.Blocks;

namespace LunyScript.Api
{
	/// <summary>
	/// Builder for 'While' loops.
	/// </summary>
	public readonly struct WhileBlockBuilder
	{
		private readonly ScriptConditionBlock[] _conditions;

		internal WhileBlockBuilder(ScriptConditionBlock[] conditions) => _conditions = conditions;

		public ScriptActionBlock Do(params ScriptActionBlock[] blocks) => WhileBlock.Create(_conditions, blocks);
	}
}
