using Luny;
using Luny.Engine.Bridge;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDestroySelfBlock : ActionBlock
	{
		public static ActionBlock Create(StackTrace trace) => new ObjectDestroySelfBlock(trace);
		private ObjectDestroySelfBlock(StackTrace trace)
			: base(trace) {}
		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.Destroy();
	}

	internal sealed class ObjectDestroyTargetBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target, StackTrace trace) => new ObjectDestroyTargetBlock(target, trace);
		private ObjectDestroyTargetBlock(LunyObjectRef target, StackTrace trace)
			: base(trace) => _target = target;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;

			target.Destroy();
		}
	}
}
