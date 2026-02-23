using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformRotateTowardsBlock : TransformTowardsObjectBlock
	{
		public static TransformRotateTowardsBlock Create(
			ILunyObject target,
			Double speed,
			Double deadZone = 0.1,
			Boolean lockX = false,
			Boolean lockY = false,
			Boolean lockZ = false,
			Double responsiveness = 1.0) => new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness);

		private TransformRotateTowardsBlock(
			ILunyObject target,
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness)
			: base(target, speed, deadZone, lockX, lockY, lockZ, responsiveness) { }

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
}
