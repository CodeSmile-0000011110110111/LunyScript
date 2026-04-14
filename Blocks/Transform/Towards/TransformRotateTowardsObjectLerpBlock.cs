using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateTowardsObjectLerpBlock : TransformTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformRotateTowardsObjectLerpBlock Create(LunyObjectRef target, Double speed, Double deadZone = 0.1,
			LunyVector3 lockAxis = default, Boolean spherical = false, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, lockAxis, spherical, trace);

		private TransformRotateTowardsObjectLerpBlock(LunyObjectRef target, Double speed, Double deadZone, LunyVector3 lockAxis,
			Boolean spherical, LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var currentRotation = context.LunyObject.Transform.Rotation;
			if (!TryGetTargetRotation(context, currentRotation, out var targetRotation))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.Rotation = _spherical
				? LunyQuaternion.Slerp(currentRotation, targetRotation, t)
				: LunyQuaternion.Lerp(currentRotation, targetRotation, t);
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, Spherical={_spherical}";
	}
}
