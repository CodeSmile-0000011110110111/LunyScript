using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for Towards blocks whose target is an <see cref="ILunyObject"/> in the scene
	/// (Move and Rotate towards).
	/// </summary>
	public abstract class TransformInterpolateTowardsObjectBlock : TransformInterpolateBlock
	{
		protected readonly LunyObjectRef Target;

		protected TransformInterpolateTowardsObjectBlock(LunyObjectRef target, VariableBlock speed, Double deadZone, LunyVector3 lockAxis,
			LunyStackTrace trace)
			: base(speed, deadZone, lockAxis, trace) => Target = target;

		/// <summary>
		/// Computes the axis-masked position delta and masked target position.
		/// Returns false (and skips) when the distance is within the dead zone.
		/// </summary>
		protected Boolean TryGetPositionDelta(IScriptRuntimeContext ctx, LunyVector3 currentPos, out LunyVector3 targetPos)
		{
			targetPos = LunyVector3.Zero;

			var targetTransform = Target?.Value?.Transform;
			if (targetTransform == null)
				return false;

			var maskedDelta = (targetTransform.Position - currentPos) * LockAxis;
			targetPos = currentPos + maskedDelta;
			return maskedDelta.Magnitude >= DeadZone;
		}

		/// <summary>
		/// Computes the target rotation from the direction toward the target object.
		/// Returns false (and skips) when the direction is zero or the angle is within the dead zone.
		/// </summary>
		protected Boolean TryGetTargetRotation(IScriptRuntimeContext ctx, LunyQuaternion currentRot, out LunyQuaternion targetRot)
		{
			var transform = ctx.LunyObject.Transform;
			currentRot = transform.Rotation;
			targetRot = LunyQuaternion.Identity;

			var targetTransform = Target?.Value?.Transform;
			if (targetTransform == null)
				return false;

			if (!VectorUtil.TryGetMaskedDirection(transform.Position, targetTransform.Position, LockAxis, out var direction))
			{
				targetRot = default;
				return false;
			}
			targetRot = LunyQuaternion.LookRotation(direction.Normalized);
			return LunyQuaternion.Angle(currentRot, targetRot) >= DeadZone;
		}

		protected String TowardsObjectParametersToString() => $"{Target}, {ParametersToString()}";
	}
}
