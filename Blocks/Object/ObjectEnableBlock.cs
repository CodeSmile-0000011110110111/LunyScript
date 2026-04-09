using Luny.Engine.Bridge;

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
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target) => new ObjectEnableTargetBlock(target);
		private ObjectEnableTargetBlock(LunyObjectRef target) => _target = target;

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
		public static ActionBlock Create(LunyObjectRef target, VariableBlock enabled) => new ObjectSetEnabledBlock(target, enabled);

		private ObjectSetEnabledBlock(LunyObjectRef target, VariableBlock enabled)
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
