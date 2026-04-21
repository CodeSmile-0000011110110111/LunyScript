using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateTowardsObjectBlock : TransformTowardsObjectBlock
	{
		private readonly LunyVector3 _worldUp;
		private readonly LunyInterpolation _interpolation;

		public static TransformRotateTowardsObjectBlock Create(LunyObjectRef target, VariableBlock speed, VariableBlock deadZone,
			LunyVector3 lockAxis = default, LunyInterpolation interpolation = LunyInterpolation.Towards,
			LunyVector3 worldUp = default, LunyStackTrace trace = null) =>
			new(target, speed, deadZone, lockAxis, interpolation, worldUp, trace);

		private TransformRotateTowardsObjectBlock(LunyObjectRef target, VariableBlock speed, VariableBlock deadZone, LunyVector3 lockAxis,
			LunyInterpolation interpolation, LunyVector3 worldUp, LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace)
		{
			_interpolation = interpolation;
			_worldUp = worldUp;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var currentRotation = context.LunyGameObject.Transform.Rotation;
			if (!TryGetTargetRotation(context, currentRotation, _worldUp, out var targetRotation, out var deltaAngle))
				return;

			var t = ComputeStep();
			context.LunyGameObject.Transform.Rotation = _interpolation switch
			{
				LunyInterpolation.Spherical => LunyQuaternion.Slerp(currentRotation, targetRotation, t),
				LunyInterpolation.Linear => LunyQuaternion.Lerp(currentRotation, targetRotation, t),
				LunyInterpolation.Towards => LunyQuaternion.RotateTowards(currentRotation, targetRotation, t * deltaAngle),
				LunyInterpolation.Instant => targetRotation,
				var _ => throw new ArgumentOutOfRangeException(nameof(_interpolation), $"unhandled interpolation: {_interpolation}"),
			};
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, WorldUp={_worldUp}, Interpolation={_interpolation}";
	}
}
