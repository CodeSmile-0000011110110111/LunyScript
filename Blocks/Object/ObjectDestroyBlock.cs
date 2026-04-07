using Luny;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDestroySelfBlock : ActionBlock
	{
		public static ActionBlock Create() => new ObjectDestroySelfBlock();

		private ObjectDestroySelfBlock() {}

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.Destroy();
	}

	internal sealed class ObjectDestroyTargetBlock : ActionBlock
	{
		private readonly String _name;

		public static ActionBlock Create(String name) => new ObjectDestroyTargetBlock(name);

		private ObjectDestroyTargetBlock(String name) => _name = name;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = LunyEngine.Instance.TryGetObject(_name);
			if (target == null)
				return;

			//LunyLogger.LogInfo($"Destroy: {target.Transform.LocalPosition} {target}");
			target.Destroy();
		}
	}
}
