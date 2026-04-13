using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleTowardsVariableBlock : TransformInterpolateTowardsVariableBlock
	{
		public static TransformScaleTowardsVariableBlock Create(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, LunyStackTrace trace = null) =>
			new(targetScale, speed, deadZone, axisLock, trace);

		private TransformScaleTowardsVariableBlock(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone, LunyVector3 axisLock,
			LunyStackTrace trace)
			: base(targetScale, speed, deadZone, axisLock, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetScaleDelta(context, out var current, out var maskedTarget))
				return;

			context.LunyObject.Transform.LocalScale = LunyVector3.MoveTowards(current, maskedTarget, ComputeStep());
		}

		public override String ToString() => TowardsVariableParametersToString();
	}
}
