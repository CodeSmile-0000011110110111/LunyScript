using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleByBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly VariableBlock<LunyVector3> _scalePerSecond;
		private readonly Boolean _useVector3;
		private readonly Boolean _useBoxClamp;
		private readonly Boolean _useSphereClamp;
		private readonly LunyVector3 _boxMin;
		private readonly LunyVector3 _boxMax;
		private readonly Single _sphereRadius;
		private LunyVector3 _accumulatedScale;

		public static TransformScaleByBlock CreateUniform(VariableBlock amount,
			Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace) =>
			new(amount, null, useBoxClamp, boxMin, boxMax, useSphereClamp, sphereRadius, trace);

		public static TransformScaleByBlock CreateVector3(VariableBlock<LunyVector3> scalePerSecond,
			Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace) =>
			new(null, scalePerSecond, useBoxClamp, boxMin, boxMax, useSphereClamp, sphereRadius, trace);

		private TransformScaleByBlock(VariableBlock amount, VariableBlock<LunyVector3> scalePerSecond,
			Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace)
			: base(trace)
		{
			_useVector3 = scalePerSecond != null;
			_amount = amount ?? LiteralVariableBlock.Create(1, trace);
			_scalePerSecond = scalePerSecond;
			_useBoxClamp = useBoxClamp;
			_useSphereClamp = useSphereClamp;
			_boxMin = boxMin;
			_boxMax = boxMax;
			_sphereRadius = sphereRadius;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var deltaTime = (Single)LunyTime.DeltaTime;

			LunyVector3 delta;
			if (_useVector3)
				delta = _scalePerSecond.Value * deltaTime;
			else
			{
				var uniformDelta = _amount.Value * deltaTime;
				delta = new LunyVector3(uniformDelta, uniformDelta, uniformDelta);
			}

			var previousScale = _accumulatedScale;
			_accumulatedScale += delta;

			if (_useSphereClamp)
				_accumulatedScale = LunyVector3.ClampMagnitude(_accumulatedScale, _sphereRadius);

			if (_useBoxClamp)
				_accumulatedScale = LunyVector3.Max(_boxMin, LunyVector3.Min(_boxMax, _accumulatedScale));

			var clampedDelta = _accumulatedScale - previousScale;
			transform.LocalScale += clampedDelta;
		}

		public override String ToString() => _useVector3
			? $"ScaleBy Vec3={_scalePerSecond}"
			: $"ScaleBy Amount={_amount}";
	}
}
