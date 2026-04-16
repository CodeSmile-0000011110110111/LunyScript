using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodySetKinematicBlock : ActionBlock
	{
		private readonly VariableBlock _enabled;
		internal static RigidbodySetKinematicBlock Create(VariableBlock enabled, LunyStackTrace trace) => new(enabled, trace);

		private RigidbodySetKinematicBlock(VariableBlock enabled, LunyStackTrace trace)
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
			rigidbody.IsKinematic = _enabled.Variable.IsTrue;
		}

		public override String ToString() => _enabled.ToString();
	}
}
