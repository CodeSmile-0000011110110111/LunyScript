using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleByBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly VariableBlock<LunyVector3> _scalePerSecond;
		private readonly VariableBlock _speed;
		private readonly Boolean _useVector3;
		private readonly Boolean _useBoxClamp;
		private readonly Boolean _useSphereClamp;
		private readonly LunyVector3 _boxMin;
		private readonly LunyVector3 _boxMax;
		private readonly Single _sphereRadius;

		public static TransformScaleByBlock CreateUniform(VariableBlock amount, VariableBlock speed,
			Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace) =>
			new(amount, null, speed, useBoxClamp, boxMin, boxMax, useSphereClamp, sphereRadius, trace);

		public static TransformScaleByBlock CreateVector3(VariableBlock<LunyVector3> scalePerSecond, VariableBlock speed,
			Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace) => new(null, scalePerSecond, speed, useBoxClamp, boxMin, boxMax,
			useSphereClamp, sphereRadius, trace);

		private TransformScaleByBlock(VariableBlock amount, VariableBlock<LunyVector3> scalePerSecond, VariableBlock speed,
			Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace)
			: base(trace)
		{
			_useVector3 = scalePerSecond != null;
			_amount = amount ?? LiteralVariableBlock.Create(1, trace);
			_scalePerSecond = scalePerSecond;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_useBoxClamp = useBoxClamp;
			_useSphereClamp = useSphereClamp;
			_boxMin = boxMin;
			_boxMax = boxMax;
			_sphereRadius = sphereRadius;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var deltaTime = LunyTime.DeltaTime;

			LunyVector3 delta;
			var speedDeltaTime = _speed.Value * deltaTime;
			if (_useVector3)
				delta = _scalePerSecond.Value * speedDeltaTime;
			else
			{
				var uniformDelta = _amount.Value * speedDeltaTime;
				delta = new LunyVector3(uniformDelta, uniformDelta, uniformDelta);
			}

			var newScale = transform.LocalScale + delta;

			if (_useSphereClamp)
				newScale = LunyVector3.ClampMagnitude(newScale, _sphereRadius);

			if (_useBoxClamp)
				newScale = LunyVector3.Max(_boxMin, LunyVector3.Min(_boxMax, newScale));

			transform.LocalScale = newScale;
		}

		public override String ToString() => _useVector3
			? $"Vec3={_scalePerSecond}"
			: $"Amount={_amount}";
	}
}
