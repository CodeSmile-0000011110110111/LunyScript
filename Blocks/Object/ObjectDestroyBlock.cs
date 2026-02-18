using Luny;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDestroySelfBlock : ScriptActionBlock
	{
		public static ScriptActionBlock Create() => new ObjectDestroySelfBlock();

		private ObjectDestroySelfBlock() {}

		protected internal  override void Execute(IScriptRuntimeContext runtimeContext) => runtimeContext.LunyObject.Destroy();
	}

	internal sealed class ObjectDestroyTargetBlock : ScriptActionBlock
	{
		private readonly String _name;

		public static ScriptActionBlock Create(String name) => new ObjectDestroyTargetBlock(name);

		private ObjectDestroyTargetBlock(String name) => _name = name;

		protected internal  override void Execute(IScriptRuntimeContext runtimeContext) => LunyEngine.Instance.Objects.GetByName(_name)?.Destroy();
	}
}
