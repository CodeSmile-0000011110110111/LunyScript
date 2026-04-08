using Luny;
using Luny.Engine.Bridge;

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
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target) => new ObjectDisableTargetBlock(target);
		private ObjectDisableTargetBlock(LunyObjectRef target) => _target = target;
		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;
			target.IsEnabled = false;
		}
	}
}
