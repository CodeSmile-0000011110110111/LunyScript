using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveTowardsObjectLerpBlock : TransformInterpolateTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformMoveTowardsObjectLerpBlock Create(LunyObjectRef target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, Double responsiveness = 1.0, Boolean spherical = false, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, axisLock, responsiveness, spherical, trace);

		private TransformMoveTowardsObjectLerpBlock(LunyObjectRef target, Double speed, Double deadZone, LunyVector3 axisLock,
			Double responsiveness, Boolean spherical, LunyStackTrace trace)
			: base(target, speed, deadZone, axisLock, responsiveness, trace) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetPositionDelta(context, out var current, out var maskedTarget))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.Position =
				_spherical ? LunyVector3.Slerp(current, maskedTarget, t) : LunyVector3.Lerp(current, maskedTarget, t);
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, Spherical={_spherical}";
	}
}
