using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateTowardsObjectBlock : TransformTowardsObjectBlock
	{
		private readonly LunyInterpolation _interpolation;

		public static TransformRotateTowardsObjectBlock Create(LunyObjectRef target, VariableBlock speed, VariableBlock deadZone,
			LunyVector3 lockAxis = default, LunyInterpolation interpolation = LunyInterpolation.ConstantSpeed,
			LunyStackTrace trace = null) => new(target, speed, deadZone, lockAxis, interpolation, trace);

		private TransformRotateTowardsObjectBlock(LunyObjectRef target, VariableBlock speed, VariableBlock deadZone, LunyVector3 lockAxis,
			LunyInterpolation interpolation, LunyStackTrace trace)
			: base(target, speed, deadZone, lockAxis, trace) => _interpolation = interpolation;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var currentRotation = context.LunyObject.Transform.Rotation;
			if (!TryGetTargetRotation(context, currentRotation, out var targetRotation))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.Rotation = _interpolation switch
			{
				LunyInterpolation.Spherical => LunyQuaternion.Slerp(currentRotation, targetRotation, t),
				LunyInterpolation.Linear => LunyQuaternion.Lerp(currentRotation, targetRotation, t),
				var _ => LunyQuaternion.RotateTowards(currentRotation, targetRotation, t),
			};
		}

		public override String ToString() => $"{TowardsObjectParametersToString()}, Interpolation={_interpolation}";
	}
}
