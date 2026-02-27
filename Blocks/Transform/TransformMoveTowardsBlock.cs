using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveTowardsBlock : TransformTowardsObjectBlock
	{
		public static TransformMoveTowardsBlock Create(
			ILunyObject target,
			Double speed,
			Double deadZone = 0.1,
			Boolean lockX = false,
			Boolean lockY = false,
			Boolean lockZ = false,
			Double responsiveness = 1.0) => new(target, speed, deadZone, lockX, lockY, lockZ, responsiveness);

		private TransformMoveTowardsBlock(
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
}
