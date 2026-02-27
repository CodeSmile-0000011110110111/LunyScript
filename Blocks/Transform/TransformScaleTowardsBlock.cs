using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleTowardsBlock : TransformTowardsVariableBlock
	{
		public static TransformScaleTowardsBlock Create(
			VariableBlock targetScale,
			Double speed,
			Double deadZone = 0.1,
			Boolean lockX = false,
			Boolean lockY = false,
			Boolean lockZ = false,
			Double responsiveness = 1.0) => new(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness);

		private TransformScaleTowardsBlock(
			VariableBlock targetScale,
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness)
			: base(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness) { }

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
}
