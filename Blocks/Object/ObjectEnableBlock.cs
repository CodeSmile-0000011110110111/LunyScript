using Luny;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : ActionBlock
	{
		public static ActionBlock Create() => new ObjectEnableSelfBlock();

		private ObjectEnableSelfBlock() {}

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.IsEnabled = true;
	}

	internal sealed class ObjectEnableTargetBlock : ActionBlock
	{
		private readonly String _name;

		public static ActionBlock Create(String name) => new ObjectEnableTargetBlock(name);

		private ObjectEnableTargetBlock(String name) => _name = name;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = LunyEngine.Instance.TryGetObject(_name);
			if (target == null)
				return;

			target.IsEnabled = true;
		}
	}
}
