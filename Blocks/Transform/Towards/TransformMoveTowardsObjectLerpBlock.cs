using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveTowardsObjectLerpBlock : TransformInterpolateTowardsObjectBlock
	{
		private readonly Boolean _spherical;

		public static TransformMoveTowardsObjectLerpBlock Create(LunyObjectRef target, VariableBlock speed, Double deadZone = 0.1,
			LunyVector3 lockAxis = default, Boolean spherical = false, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, lockAxis, spherical, trace);

		private TransformMoveTowardsObjectLerpBlock(LunyObjectRef target, VariableBlock speed, Double deadZone, LunyVector3 lockAxis,
			Boolean spherical, LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var currentPos = transform.Position;

			if (!TryGetPositionDelta(context, currentPos, out var targetPos))
				return;

			if (_spherical && currentPos == LunyVector3.Zero)
				currentPos = LunyVector3.Forward * 0.0001f; // avoid NaN when Slerping a zero Vector

			var t = ComputeStep();
			var newPos = _spherical ? LunyVector3.Slerp(currentPos, targetPos, t) : LunyVector3.Lerp(currentPos, targetPos, t);
			transform.Position = newPos;
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, Spherical={_spherical}";
	}
}
