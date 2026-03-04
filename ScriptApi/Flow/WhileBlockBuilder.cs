using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Builder for 'While' loops.
	/// </summary>
	public sealed class WhileBlockBuilder : ScriptActionBlock
	{
		private readonly ScriptConditionBlock[] _conditions;
		private ScriptActionBlock[] _blocks;
		private ScriptActionBlock _cachedBlock;

		internal WhileBlockBuilder(ScriptConditionBlock[] conditions) => _conditions = conditions;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

		public ScriptActionBlock Do(params ScriptActionBlock[] blocks)
		{
			_blocks = blocks;
			return Build();
		}

		private ScriptActionBlock Build() => WhileBlock.Create(_conditions, _blocks);
	}
}
