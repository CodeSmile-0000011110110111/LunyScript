using Luny;
using Luny.Engine.Bridge;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : ActionBlock
	{
		public static ActionBlock Create(StackTrace trace) => new ObjectEnableSelfBlock(trace);
		private ObjectEnableSelfBlock(StackTrace trace)
			: base(trace) {}
		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.IsEnabled = true;
	}

	internal sealed class ObjectEnableTargetBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target, StackTrace trace) => new ObjectEnableTargetBlock(target, trace);
		private ObjectEnableTargetBlock(LunyObjectRef target, StackTrace trace)
			: base(trace) => _target = target;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;

			target.IsEnabled = true;
		}
	}

	internal sealed class ObjectSetEnabledBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		private readonly VariableBlock _enabled;
		public static ActionBlock Create(LunyObjectRef target, VariableBlock enabled, StackTrace trace) => new ObjectSetEnabledBlock(target, enabled, trace);

		private ObjectSetEnabledBlock(LunyObjectRef target, VariableBlock enabled, StackTrace trace)
			: base(trace)
		{
			_target = target;
			_enabled = enabled;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target != null ? _target.Value : context.LunyObject;
			if (target == null)
				return;

			target.IsEnabled = _enabled.Evaluate(context);
		}
	}
}
