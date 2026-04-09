using Luny.Engine.Bridge;

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
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target) => new ObjectDestroyTargetBlock(target);
		private ObjectDestroyTargetBlock(LunyObjectRef target) => _target = target;
		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;
			target.Destroy();
		}
	}
}
