using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		public static TransformRotateTowardsObjectBlock Create(LunyObjectRef target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, axisLock, trace);

		private TransformRotateTowardsObjectBlock(LunyObjectRef target, Double speed, Double deadZone, LunyVector3 axisLock,
			LunyStackTrace trace)
			: base(target, speed, deadZone, axisLock, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetTargetRotation(context, out var currentRotation, out var targetRotation))
				return;

			context.LunyObject.Transform.Rotation =
				LunyQuaternion.RotateTowards(currentRotation, targetRotation, ComputeStep());
		}

		public override String ToString() => TowardsObjectParametersToString();
	}
}
