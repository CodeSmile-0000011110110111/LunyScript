using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleTowardsVariableLerpBlock : TransformInterpolateTowardsVariableBlock
	{
		private readonly Boolean _spherical;

		public static TransformScaleTowardsVariableLerpBlock Create(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone = 0.1,
			LunyVector3 lockAxis = default, Boolean spherical = false, LunyStackTrace trace = null) =>
			new(targetScale, speed, deadZone, lockAxis, spherical, trace);

		private TransformScaleTowardsVariableLerpBlock(VariableBlock<LunyVector3> targetScale, Double speed, Double deadZone,
			LunyVector3 lockAxis, Boolean spherical, LunyStackTrace trace)
			: base(targetScale, speed, deadZone, lockAxis, trace) => _spherical = spherical;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (!TryGetScaleDelta(context, out var current, out var maskedTarget))
				return;

			var t = ComputeStep();
			context.LunyObject.Transform.LocalScale = _spherical
				? LunyVector3.Slerp(current, maskedTarget, t)
				: LunyVector3.Lerp(current, maskedTarget, t);
		}

		public override String ToString() => $"{TowardsVariableParametersToString()}, Spherical={_spherical}";
	}
}
