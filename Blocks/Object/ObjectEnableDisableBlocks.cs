using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : ActionBlock
	{
		public static ActionBlock Create(LunyStackTrace trace) => new ObjectEnableSelfBlock(trace);

		private ObjectEnableSelfBlock(LunyStackTrace trace)
			: base(trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyGameObject.IsEnabled = true;
		public override String ToString() => "self";
	}

	internal sealed class ObjectDisableSelfBlock : ActionBlock
	{
		public static ActionBlock Create(LunyStackTrace trace) => new ObjectDisableSelfBlock(trace);

		private ObjectDisableSelfBlock(LunyStackTrace trace)
			: base(trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyGameObject.IsEnabled = false;
		public override String ToString() => "self";
	}

	internal sealed class ObjectEnableTargetBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target, LunyStackTrace trace) => new ObjectEnableTargetBlock(target, trace);

		private ObjectEnableTargetBlock(LunyObjectRef target, LunyStackTrace trace)
			: base(trace) => _target = target;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;

			target.IsEnabled = true;
		}

		public override String ToString() => _target?.ToString();
	}

	internal sealed class ObjectDisableTargetBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target, LunyStackTrace trace) => new ObjectDisableTargetBlock(target, trace);

		private ObjectDisableTargetBlock(LunyObjectRef target, LunyStackTrace trace)
			: base(trace) => _target = target;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;

			target.IsEnabled = false;
		}

		public override String ToString() => _target?.ToString();
	}

	internal sealed class ObjectSetEnabledBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		private readonly VariableBlock _enabled;

		public static ActionBlock Create(LunyObjectRef target, VariableBlock enabled, LunyStackTrace trace) =>
			new ObjectSetEnabledBlock(target, enabled, trace);

		private ObjectSetEnabledBlock(LunyObjectRef target, VariableBlock enabled, LunyStackTrace trace)
			: base(trace)
		{
			_target = target;
			_enabled = enabled;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target != null ? _target.Value : context.LunyGameObject;
			if (target == null)
				return;

			target.IsEnabled = _enabled.Evaluate(context);
		}

		public override String ToString() => _target?.ToString();
	}
}
