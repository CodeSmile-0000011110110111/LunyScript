using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateTowardsObjectLerpBlock : TransformInterpolateTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformRotateTowardsObjectLerpBlock Create(LunyObjectRef target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0, Boolean spherical = false, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, axisLock, responsiveness, spherical, trace);

		private TransformRotateTowardsObjectLerpBlock(LunyObjectRef target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness, Boolean spherical, LunyStackTrace trace)
			: base(target, speed, deadZone, axisLock, responsiveness, trace) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetTargetRotation(context, out var currentRotation, out var targetRotation))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.Rotation = _spherical
				? LunyQuaternion.Slerp(currentRotation, targetRotation, t)
				: LunyQuaternion.Lerp(currentRotation, targetRotation, t);
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, Spherical={_spherical}";
	}
}
