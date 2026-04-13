using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for Towards blocks whose target is a <see cref="VariableBlock"/> value
	/// (Scale towards).
	/// </summary>
	public abstract class TransformInterpolateTowardsVariableBlock : TransformInterpolateBlock
	{
		protected readonly VariableBlock<LunyVector3> TargetScale;

		protected TransformInterpolateTowardsVariableBlock(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone,
			LunyVector3 axisLock, Double responsiveness, LunyStackTrace trace)
			: base(speed, deadZone, axisLock, responsiveness, trace) => TargetScale = targetScale;

		/// <summary>
		/// Computes the axis-masked scale delta and masked target.
		/// Returns false (and skips) when the distance is within the dead zone.
		/// </summary>
		protected Boolean TryGetScaleDelta(IScriptRuntimeContext ctx, out LunyVector3 current, out LunyVector3 maskedTarget)
		{
			var transform = ctx.LunyObject.Transform;
			current = transform.LocalScale;
			var delta = (TargetScale.Value - current) * AxisLock;
			maskedTarget = current + delta;
			return delta.Magnitude >= DeadZone;
		}

		protected String TowardsVariableParametersToString() => $"{TargetScale}, {ParametersToString()}";
	}
}
