using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleTowardsVariableBlock : TransformTowardsVariableBlock
	{
		private readonly LunyInterpolation _interpolation;

		public static TransformScaleTowardsVariableBlock Create(VariableBlock<LunyVector3> targetScale, VariableBlock speed,
			VariableBlock deadZone, LunyVector3 lockAxis = default, LunyInterpolation interpolation = LunyInterpolation.Towards,
			LunyStackTrace trace = null) => new(targetScale, speed, deadZone, lockAxis, interpolation, trace);

		private TransformScaleTowardsVariableBlock(VariableBlock<LunyVector3> targetScale, VariableBlock speed, VariableBlock deadZone,
			LunyVector3 lockAxis, LunyInterpolation interpolation, LunyStackTrace trace)
			: base(targetScale, speed, deadZone, lockAxis, trace) => _interpolation = interpolation;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetScaleDelta(context, out var current, out var maskedTarget))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.LocalScale = _interpolation switch
			{
				LunyInterpolation.Spherical => LunyVector3.Slerp(current, maskedTarget, t),
				LunyInterpolation.Linear => LunyVector3.Lerp(current, maskedTarget, t),
				var _ => LunyVector3.MoveTowards(current, maskedTarget, t),
			};
		}

		public override String ToString() => $"{TowardsVariableParametersToString()}, Interpolation={_interpolation}";
	}
}
