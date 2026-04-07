using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that interpolate or step a transform property toward a target over time.
	/// Holds the shared optional parameters: speed, deadZone, axisLock, responsiveness.
	/// </summary>
	public abstract class TransformInterpolateBlock : ActionBlock
	{
		protected readonly Single Speed;
		protected readonly Single DeadZone;
		protected readonly LunyVector3 AxisLock;
		protected readonly Single Responsiveness;

		protected TransformInterpolateBlock(Double speed, Double deadZone, LunyVector3 axisLock, Double responsiveness)
		{
			Speed = speed > 0f ? (Single)speed : 1f;
			DeadZone = (Single)deadZone;
			AxisLock = axisLock;
			Responsiveness = responsiveness > 0f ? (Single)responsiveness : 1f;
		}

		/// <summary>Returns Speed * deltaTime * Responsiveness, ready to use as a step or lerp t value.</summary>
		protected Single ComputeStep() => Speed * LunyTime.DeltaTime * Responsiveness;

		protected String TowardsToString() => $"speed={Speed}, deadZone={DeadZone}, axisLock={AxisLock}, responsiveness={Responsiveness}";
	}

	/// <summary>
	/// Abstract base for Towards blocks whose target is a <see cref="VariableBlock"/> value
	/// (Scale towards).
	/// </summary>
	public abstract class TransformInterpolateTowardsVariableBlock : TransformInterpolateBlock
	{
		protected readonly VariableBlock<LunyVector3> TargetScale;

		protected TransformInterpolateTowardsVariableBlock(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone,
			LunyVector3 axisLock,
			Double responsiveness)
			: base(speed, deadZone, axisLock, responsiveness) => TargetScale = targetScale;

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

		protected String TowardsVariableToString() => $"{TargetScale}, {TowardsToString()}";
	}

	public sealed class TransformScaleTowardsBlock : TransformInterpolateTowardsVariableBlock
	{
		public static TransformScaleTowardsBlock Create(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0) => new(targetScale, speed, deadZone, axisLock, responsiveness);

		private TransformScaleTowardsBlock(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness)
			: base(targetScale, speed, deadZone, axisLock, responsiveness) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetScaleDelta(context, out var current, out var maskedTarget))
				return;

			context.LunyObject.Transform.LocalScale = LunyVector3.MoveTowards(current, maskedTarget, ComputeStep());
		}

		public override String ToString() => $"{GetType().Name}({TowardsVariableToString()})";
	}

	public sealed class TransformScaleTowardsLerpBlock : TransformInterpolateTowardsVariableBlock
	{
		private readonly Boolean _spherical;

		public static TransformScaleTowardsLerpBlock Create(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0, Boolean spherical = false) =>
			new(targetScale, speed, deadZone, axisLock, responsiveness, spherical);

		private TransformScaleTowardsLerpBlock(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness, Boolean spherical)
			: base(targetScale, speed, deadZone, axisLock, responsiveness) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetScaleDelta(context, out var current, out var maskedTarget))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.LocalScale = _spherical
				? LunyVector3.Slerp(current, maskedTarget, t)
				: LunyVector3.Lerp(current, maskedTarget, t);
		}

		public override String ToString() => $"{GetType().Name}({TowardsVariableToString()}, spherical={_spherical})";
	}

	/// <summary>
	/// Abstract base for Towards blocks whose target is an <see cref="ILunyObject"/> in the scene
	/// (Move and Rotate towards).
	/// </summary>
	public abstract class TransformInterpolateTowardsObjectBlock : TransformInterpolateBlock
	{
		protected readonly ILunyObject Target;

		protected TransformInterpolateTowardsObjectBlock(ILunyObject target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness)
			: base(speed, deadZone, axisLock, responsiveness) => Target = target;

		/// <summary>
		/// Computes the axis-masked position delta and masked target position.
		/// Returns false (and skips) when the distance is within the dead zone.
		/// </summary>
		protected Boolean TryGetPositionDelta(IScriptRuntimeContext ctx, out LunyVector3 current, out LunyVector3 maskedTarget)
		{
			current = ctx.LunyObject.Transform.Position;
			var maskedDelta = (Target.Transform.Position - current) * AxisLock;
			maskedTarget = current + maskedDelta;
			return maskedDelta.Magnitude >= DeadZone;
		}

		/// <summary>
		/// Computes the target rotation from the direction toward the target object.
		/// Returns false (and skips) when the direction is zero or the angle is within the dead zone.
		/// </summary>
		protected Boolean TryGetTargetRotation(IScriptRuntimeContext ctx, out LunyQuaternion currentRotation, out LunyQuaternion targetRotation)
		{
			var transform = ctx.LunyObject.Transform;
			currentRotation = transform.Rotation;
			if (!VectorUtil.TryGetMaskedDirection(transform.Position, Target.Transform.Position, AxisLock, out var direction))
			{
				targetRotation = default;
				return false;
			}
			targetRotation = LunyQuaternion.LookRotation(direction.Normalized);
			return LunyQuaternion.Angle(currentRotation, targetRotation) >= DeadZone;
		}

		protected String TowardsObjectToString() => $"{Target}, {TowardsToString()}";
	}

	public sealed class TransformPositionLinearTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		public static TransformPositionLinearTowardsObjectBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0) => new(target, speed, deadZone, axisLock, responsiveness);

		private TransformPositionLinearTowardsObjectBlock(ILunyObject target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness)
			: base(target, speed, deadZone, axisLock, responsiveness) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetPositionDelta(context, out var current, out var maskedTarget))
				return;

			context.LunyObject.Transform.Position = LunyVector3.MoveTowards(current, maskedTarget, ComputeStep());
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()})";
	}

	public sealed class TransformPositionLerpTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformPositionLerpTowardsObjectBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0, Boolean spherical = false) =>
			new(target, speed, deadZone, axisLock, responsiveness, spherical);

		private TransformPositionLerpTowardsObjectBlock(ILunyObject target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness, Boolean spherical)
			: base(target, speed, deadZone, axisLock, responsiveness) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetPositionDelta(context, out var current, out var maskedTarget))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.Position =
				_spherical ? LunyVector3.Slerp(current, maskedTarget, t) : LunyVector3.Lerp(current, maskedTarget, t);
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()}, spherical={_spherical})";
	}

	public sealed class TransformRotationLinearTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		public static TransformRotationLinearTowardsObjectBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0) => new(target, speed, deadZone, axisLock, responsiveness);

		private TransformRotationLinearTowardsObjectBlock(ILunyObject target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness)
			: base(target, speed, deadZone, axisLock, responsiveness) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetTargetRotation(context, out var currentRotation, out var targetRotation))
				return;

			context.LunyObject.Transform.Rotation =
				LunyQuaternion.RotateTowards(currentRotation, targetRotation, ComputeStep());
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()})";
	}

	public sealed class TransformRotationLerpTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformRotationLerpTowardsObjectBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0, Boolean spherical = false) =>
			new(target, speed, deadZone, axisLock, responsiveness, spherical);

		private TransformRotationLerpTowardsObjectBlock(ILunyObject target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness, Boolean spherical)
			: base(target, speed, deadZone, axisLock, responsiveness) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetTargetRotation(context, out var currentRotation, out var targetRotation))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.Rotation = _spherical
				? LunyQuaternion.Slerp(currentRotation, targetRotation, t)
				: LunyQuaternion.Lerp(currentRotation, targetRotation, t);
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()}, spherical={_spherical})";
	}
}
