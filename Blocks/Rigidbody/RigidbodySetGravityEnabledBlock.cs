using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodySetGravityEnabledBlock : ActionBlock
	{
		private readonly VariableBlock _enabled;
		internal static RigidbodySetGravityEnabledBlock Create(VariableBlock enabled, LunyStackTrace trace) => new(enabled, trace);

		private RigidbodySetGravityEnabledBlock(VariableBlock enabled, LunyStackTrace trace)
			: base(trace) => _enabled = enabled;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyGameObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodySetGravityEnabledBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyGameObject.Name}'",
					context.LunyGameObject);
				return;
			}
			rigidbody.UseGravity = _enabled.Variable.IsTrue;
		}

		public override String ToString() => _enabled.ToString();
	}
}
