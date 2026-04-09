using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodySetKinematicBlock : ActionBlock
	{
		private readonly Boolean _enabled;
		internal static RigidbodySetKinematicBlock Create(Boolean enabled, StackTrace trace) => new(enabled, trace);

		private RigidbodySetKinematicBlock(Boolean enabled, StackTrace trace)
			: base(trace) => _enabled = enabled;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodySetKinematicBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'",
					context.LunyObject);
				return;
			}
			rigidbody.SetKinematic(_enabled);
		}

		public override String ToString() => $"{GetType().Name}({_enabled})";
	}
}
