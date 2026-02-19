using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : ScriptActionBlock
	{
		public static ScriptActionBlock Create() => new ObjectEnableSelfBlock();

		private ObjectEnableSelfBlock() {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => runtimeContext.LunyObject.IsEnabled = true;
	}

	internal sealed class ObjectEnableTargetBlock : ScriptActionBlock
	{
		private readonly String _name;

		public static ScriptActionBlock Create(String name) => new ObjectEnableTargetBlock(name);

		private ObjectEnableTargetBlock(String name) => _name = name;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(ObjectEnableTargetBlock)} with name '{_name}' not implemented");
	}
}
