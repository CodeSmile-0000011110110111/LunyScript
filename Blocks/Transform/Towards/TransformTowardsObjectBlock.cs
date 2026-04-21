using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for Towards blocks whose target is an <see cref="ILunyGameObject"/> in the scene
	/// (Move and Rotate towards).
	/// </summary>
	public abstract class TransformTowardsObjectBlock : TransformTowardsBlock
	{
		private readonly LunyObjectRef Target;

		protected TransformTowardsObjectBlock(LunyObjectRef target, VariableBlock speed, VariableBlock deadZone, LunyVector3 lockAxis,
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
			return maskedDelta.Magnitude >= DeadZone.Value;
		}

		/// <summary>
		/// Computes the target rotation from the direction toward the target object.
		/// Returns false (and skips) when the direction is zero or the angle is within the dead zone.
		/// </summary>
		protected Boolean TryGetTargetRotation(IScriptRuntimeContext ctx, LunyQuaternion currentRot, LunyVector3 worldUp,
			out LunyQuaternion targetRot, out Double deltaAngle)
		{
			var transform = ctx.LunyGameObject.Transform;
			currentRot = transform.Rotation;
			targetRot = LunyQuaternion.Identity;
			deltaAngle = 0f;

			var targetTransform = Target?.Value?.Transform;
			if (targetTransform == null)
				return false;

			if (!VectorUtil.TryGetMaskedDirection(transform.Position, targetTransform.Position, LockAxis, out var direction))
			{
				targetRot = default;
				return false;
			}
			targetRot = LunyQuaternion.LookRotation(direction.Normalized, worldUp);
			deltaAngle = LunyQuaternion.Angle(currentRot, targetRot);
			return deltaAngle >= DeadZone.Value;
		}

		protected String TowardsObjectParametersToString() => $"{Target}, {ParametersToString()}";
	}
}
