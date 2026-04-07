using Luny;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDisableSelfBlock : ActionBlock
	{
		public static ActionBlock Create() => new ObjectDisableSelfBlock();

		private ObjectDisableSelfBlock() {}

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.IsEnabled = false;
	}

	internal sealed class ObjectDisableTargetBlock : ActionBlock
	{
		private readonly String _name;

		public static ActionBlock Create(String name) => new ObjectDisableTargetBlock(name);

		private ObjectDisableTargetBlock(String name) => _name = name;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = LunyEngine.Instance.TryGetObject(_name);
			if (target == null)
				return;

			target.IsEnabled = false;
		}
	}
}
