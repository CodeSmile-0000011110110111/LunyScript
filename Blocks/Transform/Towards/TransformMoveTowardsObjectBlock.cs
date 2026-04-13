using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveTowardsObjectBlock : TransformInterpolateTowardsObjectBlock
	{
		public static TransformMoveTowardsObjectBlock Create(LunyObjectRef target, Double speed, Double deadZone = 0.1,
			LunyVector3 axisLock = default, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, axisLock, trace);

		private TransformMoveTowardsObjectBlock(LunyObjectRef target, Double speed, Double deadZone, LunyVector3 axisLock,
			LunyStackTrace trace)
			: base(target, speed, deadZone, axisLock, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetPositionDelta(context, out var current, out var maskedTarget))
				return;

			context.LunyObject.Transform.Position = LunyVector3.MoveTowards(current, maskedTarget, ComputeStep());
		}

		public override String ToString() => TowardsObjectParametersToString();
	}
}
