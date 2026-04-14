using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		public static TransformRotateTowardsObjectBlock Create(LunyObjectRef target, Double speed, Double deadZone = 0.1,
			LunyVector3 lockAxis = default, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, lockAxis, trace);

		private TransformRotateTowardsObjectBlock(LunyObjectRef target, Double speed, Double deadZone, LunyVector3 lockAxis,
			LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var currentRotation = context.LunyObject.Transform.Rotation;
			if (!TryGetTargetRotation(context, currentRotation, out var targetRotation))
				return;

			context.LunyObject.Transform.Rotation = LunyQuaternion.RotateTowards(currentRotation, targetRotation, ComputeStep());
		}

		public override String ToString() => TowardsObjectParametersToString();
	}
}
