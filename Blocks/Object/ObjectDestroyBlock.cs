using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDestroySelfBlock : ActionBlock
	{
		public static ActionBlock Create(LunyStackTrace trace) => new ObjectDestroySelfBlock(trace);

		private ObjectDestroySelfBlock(LunyStackTrace trace)
			: base(trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyGameObject.Destroy();
		public override String ToString() => "self";
	}

	internal sealed class ObjectDestroyTargetBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		public static ActionBlock Create(LunyObjectRef target, LunyStackTrace trace) => new ObjectDestroyTargetBlock(target, trace);

		private ObjectDestroyTargetBlock(LunyObjectRef target, LunyStackTrace trace)
			: base(trace) => _target = target;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var target = _target.Value;
			if (target == null)
				return;

			target.Destroy();
		}

		public override String ToString() => _target?.ToString();
	}
}
