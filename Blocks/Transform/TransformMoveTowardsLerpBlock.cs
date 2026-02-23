using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformMoveTowardsLerpBlock : TransformTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformMoveTowardsLerpBlock Create(
			ILunyObject target,
			Double speed,
			Double deadZone = 0.1,
			Boolean lockX = false,
			Boolean lockY = false,
			Boolean lockZ = false,
			Double responsiveness = 1.0,
			Boolean spherical = false) => new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness, spherical);

		private TransformMoveTowardsLerpBlock(
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

		public override String ToString() =>
			$"{GetType().Name}({TowardsObjectToString()}, spherical={_spherical})";
	}
}
