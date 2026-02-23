using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformScaleTowardsLerpBlock : TransformTowardsVariableBlock
	{
		private readonly Boolean _spherical;

		public static TransformScaleTowardsLerpBlock Create(
			VariableBlock targetScale,
			Double speed,
			Double deadZone = 0.1,
			Boolean lockX = false,
			Boolean lockY = false,
			Boolean lockZ = false,
			Double responsiveness = 1.0,
			Boolean spherical = false) => new(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness, spherical);

		private TransformScaleTowardsLerpBlock(
			VariableBlock targetScale,
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness,
			Boolean spherical)
			: base(targetScale, speed, deadZone, lockX, lockY, lockZ, responsiveness)
		{
			_spherical = spherical;
		}

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

		public override String ToString() =>
			$"{GetType().Name}({TowardsVariableToString()}, spherical={_spherical})";
	}
}
