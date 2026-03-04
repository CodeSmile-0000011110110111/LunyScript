using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that interpolate or step a transform property toward a target over time.
	/// Holds the shared optional parameters: speed, deadZone, axisLock, responsiveness.
	/// </summary>
	public abstract class TransformTowardsBlock : ScriptActionBlock
	{
		protected readonly Single Speed;
		protected readonly Single DeadZone;
		protected readonly LunyVector3 AxisLock;
		protected readonly Single Responsiveness;

		protected TransformTowardsBlock(Double speed, Double deadZone, Boolean lockX, Boolean lockY, Boolean lockZ, Double responsiveness)
		{
			Speed = speed > 0f ? (Single)speed : 1f;
			DeadZone = (Single)deadZone;
			AxisLock = new LunyVector3(lockX ? 0d : 1d, lockY ? 0d : 1d, lockZ ? 0d : 1d);
			Responsiveness = responsiveness > 0f ? (Single)responsiveness : 1f;
		}

		protected String TowardsToString() => $"speed={Speed}, deadZone={DeadZone}, axisLock={AxisLock}, responsiveness={Responsiveness}";
	}

	/// <summary>
	/// Abstract base for Towards blocks whose target is a <see cref="VariableBlock"/> value
	/// (Scale towards).
	/// </summary>
	public abstract class TransformTowardsVariableBlock : TransformTowardsBlock
	{
		protected readonly VariableBlock TargetScale;

		protected TransformTowardsVariableBlock(VariableBlock targetScale, Double speed, Double deadZone, Boolean lockX, Boolean lockY,
			Boolean lockZ, Double responsiveness)
			: base(speed, deadZone, lockX, lockY, lockZ, responsiveness) => TargetScale = targetScale;

		protected String TowardsVariableToString() => $"{TargetScale}, {TowardsToString()}";
	}

	public sealed class TransformScaleTowardsBlock : TransformTowardsVariableBlock
	{
		public static TransformScaleTowardsBlock Create(VariableBlock targetScale, Double speed, Double deadZone = 0.1, Boolean lockX = false,
			Boolean lockY = false, Boolean lockZ = false, Double responsiveness = 1.0) =>
			new(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness);

		private TransformScaleTowardsBlock(VariableBlock targetScale, Double speed, Double deadZone, Boolean lockX, Boolean lockY,
			Boolean lockZ, Double responsiveness)
			: base(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var current = transform.LocalScale;
			var target = TargetScale.GetValue<LunyVector3>();
			var delta = (target - current) * AxisLock;
			var distance = delta.Magnitude;
			if (distance < DeadZone)
				return;

			var deltaTime = (Single)LunyEngine.Instance.Time.DeltaTime;
			var step = Speed * deltaTime * Responsiveness;
			var maskedTarget = current + delta;
			transform.LocalScale = LunyVector3.MoveTowards(current, maskedTarget, step);
		}

		public override String ToString() => $"{GetType().Name}({TowardsVariableToString()})";
	}

	public sealed class TransformScaleTowardsLerpBlock : TransformTowardsVariableBlock
	{
		private readonly Boolean _spherical;

		public static TransformScaleTowardsLerpBlock Create(VariableBlock targetScale, Double speed, Double deadZone = 0.1,
			Boolean lockX = false, Boolean lockY = false, Boolean lockZ = false, Double responsiveness = 1.0, Boolean spherical = false) =>
			new(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness, spherical);

		private TransformScaleTowardsLerpBlock(VariableBlock targetScale, Double speed, Double deadZone, Boolean lockX, Boolean lockY,
			Boolean lockZ, Double responsiveness, Boolean spherical)
			: base(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var current = transform.LocalScale;
			var target = TargetScale.GetValue<LunyVector3>();
			var delta = (target - current) * AxisLock;
			var distance = delta.Magnitude;
			if (distance < DeadZone)
				return;

			var deltaTime = (Single)LunyEngine.Instance.Time.DeltaTime;
			var t = Speed * deltaTime * Responsiveness;
			var maskedTarget = current + delta;
			transform.LocalScale = _spherical
				? LunyVector3.Slerp(current, maskedTarget, t)
				: LunyVector3.Lerp(current, maskedTarget, t);
		}

		public override String ToString() => $"{GetType().Name}({TowardsVariableToString()}, spherical={_spherical})";
	}

	/// <summary>
	/// Abstract base for Towards blocks whose target is an <see cref="ILunyObject"/> in the scene
	/// (Move and Rotate towards).
	/// </summary>
	public abstract class TransformTowardsObjectBlock : TransformTowardsBlock
	{
		protected readonly ILunyObject Target;

		protected TransformTowardsObjectBlock(ILunyObject target, Double speed, Double deadZone, Boolean lockX, Boolean lockY, Boolean lockZ,
			Double responsiveness)
			: base(speed, deadZone, lockX, lockY, lockZ, responsiveness) => Target = target;

		protected String TowardsObjectToString() => $"{Target}, {TowardsToString()}";
	}

	public sealed class TransformMoveTowardsBlock : TransformTowardsObjectBlock
	{
		public static TransformMoveTowardsBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1, Boolean lockX = false,
			Boolean lockY = false, Boolean lockZ = false, Double responsiveness = 1.0) =>
			new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness);

		private TransformMoveTowardsBlock(ILunyObject target, Double speed, Double deadZone, Boolean lockX, Boolean lockY, Boolean lockZ,
			Double responsiveness)
			: base(target, speed, deadZone, lockX, lockY, lockZ, responsiveness) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var current = transform.Position;
			var targetPos = Target.Transform.Position;
			var delta = targetPos - current;
			var maskedDelta = delta * AxisLock;
			var distance = maskedDelta.Magnitude;
			if (distance < DeadZone)
				return;

			var deltaTime = (Single)LunyEngine.Instance.Time.DeltaTime;
			var step = Speed * deltaTime * Responsiveness;
			var maskedTarget = current + maskedDelta;
			transform.Position = LunyVector3.MoveTowards(current, maskedTarget, step);
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()})";
	}

	public sealed class TransformMoveTowardsLerpBlock : TransformTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformMoveTowardsLerpBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1, Boolean lockX = false,
			Boolean lockY = false, Boolean lockZ = false, Double responsiveness = 1.0, Boolean spherical = false) =>
			new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness, spherical);

		private TransformMoveTowardsLerpBlock(ILunyObject target, Double speed, Double deadZone, Boolean lockX, Boolean lockY, Boolean lockZ,
			Double responsiveness, Boolean spherical)
			: base(target, speed, deadZone, lockX, lockY, lockZ, responsiveness) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var current = transform.Position;
			var targetPos = Target.Transform.Position;
			var delta = targetPos - current;
			var maskedDelta = delta * AxisLock;
			var distance = maskedDelta.Magnitude;
			if (distance < DeadZone)
				return;

			var deltaTime = (Single)LunyEngine.Instance.Time.DeltaTime;
			var t = Speed * deltaTime * Responsiveness;
			var maskedTarget = current + maskedDelta;
			transform.Position = _spherical
				? LunyVector3.Slerp(current, maskedTarget, t)
				: LunyVector3.Lerp(current, maskedTarget, t);
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()}, spherical={_spherical})";
	}

	public sealed class TransformRotateTowardsBlock : TransformTowardsObjectBlock
	{
		public static TransformRotateTowardsBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1, Boolean lockX = false,
			Boolean lockY = false, Boolean lockZ = false, Double responsiveness = 1.0) =>
			new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness);

		private TransformRotateTowardsBlock(ILunyObject target, Double speed, Double deadZone, Boolean lockX, Boolean lockY, Boolean lockZ,
			Double responsiveness)
			: base(target, speed, deadZone, lockX, lockY, lockZ, responsiveness) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var currentPos = transform.Position;
			var targetPos = Target.Transform.Position;
			var direction = (targetPos - currentPos) * AxisLock;
			if (direction.SqrMagnitude < Single.Epsilon)
				return;

			var targetRotation = LunyQuaternion.LookRotation(direction.Normalized);
			var angle = LunyQuaternion.Angle(transform.Rotation, targetRotation);
			if (angle < DeadZone)
				return;

			var deltaTime = (Single)LunyEngine.Instance.Time.DeltaTime;
			var maxDegrees = Speed * deltaTime * Responsiveness;
			transform.Rotation = LunyQuaternion.RotateTowards(transform.Rotation, targetRotation, maxDegrees);
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()})";
	}

	public sealed class TransformRotateTowardsLerpBlock : TransformTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformRotateTowardsLerpBlock Create(ILunyObject target, Double speed, Double deadZone = 0.1, Boolean lockX = false,
			Boolean lockY = false, Boolean lockZ = false, Double responsiveness = 1.0, Boolean spherical = false) =>
			new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness, spherical);

		private TransformRotateTowardsLerpBlock(ILunyObject target, Double speed, Double deadZone, Boolean lockX, Boolean lockY, Boolean lockZ,
			Double responsiveness, Boolean spherical)
			: base(target, speed, deadZone, lockX, lockY, lockZ, responsiveness) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var currentPos = transform.Position;
			var targetPos = Target.Transform.Position;
			var direction = (targetPos - currentPos) * AxisLock;
			if (direction.SqrMagnitude < Single.Epsilon)
				return;

			var targetRotation = LunyQuaternion.LookRotation(direction.Normalized);
			var angle = LunyQuaternion.Angle(transform.Rotation, targetRotation);
			if (angle < DeadZone)
				return;

			var deltaTime = (Single)LunyEngine.Instance.Time.DeltaTime;
			var t = Speed * deltaTime * Responsiveness;
			transform.Rotation = _spherical
				? LunyQuaternion.Slerp(transform.Rotation, targetRotation, t)
				: LunyQuaternion.Lerp(transform.Rotation, targetRotation, t);
		}

		public override String ToString() => $"{GetType().Name}({TowardsObjectToString()}, spherical={_spherical})";
	}
}
