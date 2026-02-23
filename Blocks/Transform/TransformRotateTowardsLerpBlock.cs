using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformRotateTowardsLerpBlock : TransformTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformRotateTowardsLerpBlock Create(
			ILunyObject target,
			Double speed,
			Double deadZone = 0.1,
			Boolean lockX = false,
			Boolean lockY = false,
			Boolean lockZ = false,
			Double responsiveness = 1.0,
			Boolean spherical = false) => new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness, spherical);

		private TransformRotateTowardsLerpBlock(
			ILunyObject target,
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness,
			Boolean spherical)
			: base(target, speed, deadZone, lockX, lockY, lockZ, responsiveness)
		{
			_spherical = spherical;
		}

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

		public override String ToString() =>
			$"{GetType().Name}({TowardsObjectToString()}, spherical={_spherical})";
	}
}
