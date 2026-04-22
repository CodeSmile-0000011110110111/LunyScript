using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveTowardsObjectBlock : TransformTowardsObjectBlock
	{
		private readonly LunyInterpolation _interpolation;

		public static TransformMoveTowardsObjectBlock Create(LunyGameObjectRef target, VariableBlock speed, VariableBlock deadZone,
			LunyVector3 lockAxis = default, LunyInterpolation interpolation = LunyInterpolation.Towards,
			LunyStackTrace trace = null) => new(target, speed, deadZone, lockAxis, interpolation, trace);

		private TransformMoveTowardsObjectBlock(LunyGameObjectRef target, VariableBlock speed, VariableBlock deadZone, LunyVector3 lockAxis,
			LunyInterpolation interpolation, LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace) => _interpolation = interpolation;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyGameObject.Transform;
			var currentPos = transform.Position;
			if (!TryGetPositionDelta(context, currentPos, out var targetPos))
				return;

			var t = ComputeStep();
			transform.Position = _interpolation switch
			{
				LunyInterpolation.Spherical when currentPos == LunyVector3.Zero =>
					LunyVector3.Slerp(LunyVector3.Forward * 0.0001f, targetPos, t),
				LunyInterpolation.Spherical => LunyVector3.Slerp(currentPos, targetPos, t),
				LunyInterpolation.Linear => LunyVector3.Lerp(currentPos, targetPos, t),
				LunyInterpolation.Towards => LunyVector3.MoveTowards(currentPos, targetPos, t),
				var _ => throw new ArgumentOutOfRangeException(nameof(_interpolation), $"unhandled interpolation: {_interpolation}"),
			};
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, Interpolation={_interpolation}";
	}
}
