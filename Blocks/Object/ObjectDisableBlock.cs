using Luny;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDisableSelfBlock : ScriptActionBlock
	{
		public static ScriptActionBlock Create() => new ObjectDisableSelfBlock();

		private ObjectDisableSelfBlock() {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => runtimeContext.LunyObject.IsEnabled = false;
	}

	internal sealed class ObjectDisableTargetBlock : ScriptActionBlock
	{
		private readonly String _name;

		public static ScriptActionBlock Create(String name) => new ObjectDisableTargetBlock(name);

		private ObjectDisableTargetBlock(String name) => _name = name;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var target = LunyEngine.Instance.TryGetObject(_name);
			if (target == null)
				return;

			target.IsEnabled = false;
		}
	}
}
