using Luny;
using Luny.Engine.Bridge;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDisableSelfBlock : ActionBlock
	{
		public static ActionBlock Create(StackTrace trace) => new ObjectDisableSelfBlock(trace);
		private ObjectDisableSelfBlock(StackTrace trace)
			: base(trace) {}
		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.IsEnabled = false;
	}

	internal sealed class ObjectDisableTargetBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target, StackTrace trace) => new ObjectDisableTargetBlock(target, trace);
		private ObjectDisableTargetBlock(LunyObjectRef target, StackTrace trace)
			: base(trace) => _target = target;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;

			target.IsEnabled = false;
		}
	}
}
