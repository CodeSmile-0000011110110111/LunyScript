using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveTowardsObjectBlock : TransformTowardsObjectBlock
	{
		public static TransformMoveTowardsObjectBlock Create(LunyObjectRef target, VariableBlock speed, Double deadZone = 0.1,
			LunyVector3 lockAxis = default, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, lockAxis, trace);

		private TransformMoveTowardsObjectBlock(LunyObjectRef target, VariableBlock speed, Double deadZone, LunyVector3 lockAxis,
			LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var currentPos = context.LunyObject.Transform.Position;
			if (!TryGetPositionDelta(context, currentPos, out var targetPos))
				return;

			context.LunyObject.Transform.Position = LunyVector3.MoveTowards(currentPos, targetPos, ComputeStep());
		}

		public override String ToString() => TowardsObjectParametersToString();
	}
}
