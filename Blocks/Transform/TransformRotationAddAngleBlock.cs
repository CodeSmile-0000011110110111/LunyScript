using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationAddAngleBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly Double _minAngle;
		private readonly Double _maxAngle;
		private readonly LunyTransformSpace _space;
		private Double _currentAngle;

		public static TransformRotationAddAngleBlock Create(VariableBlock amount, LunyAxis axis,
			LunyTransformSpace space, Double minAngle = Double.NegativeInfinity, Double maxAngle = Double.PositiveInfinity,
			LunyStackTrace trace = null) => new(amount, axis, space, minAngle, maxAngle, trace);

		private static LunyVector3 AxisToVector(LunyAxis axis)
		{
			if (axis == LunyAxis.X)
				return LunyVector3.Right;
			if (axis == LunyAxis.Y)
				return LunyVector3.Up;

			return LunyVector3.Forward;
		}

		private TransformRotationAddAngleBlock(VariableBlock amount, LunyAxis axis,
			LunyTransformSpace space, Double minAngle, Double maxAngle, LunyStackTrace trace)
			: base(trace)
		{
			_amount = amount;
			_axis = axis;
			_space = space;
			_minAngle = minAngle;
			_maxAngle = maxAngle;
			_currentAngle = 0.0;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var time = LunyEngine.Instance.Time;
			var deltaAngle = _amount.Value * time.DeltaTime;
			_currentAngle = Math.Clamp(_currentAngle + deltaAngle, _minAngle, _maxAngle);
			var axisVec = AxisToVector(_axis);
			var transform = context.LunyObject.Transform;
			if (_space == LunyTransformSpace.World)
				transform.Rotation = LunyQuaternion.AngleAxis(_currentAngle, axisVec);
			else
				transform.LocalRotation = LunyQuaternion.AngleAxis(_currentAngle, axisVec);
		}

		public override String ToString() => $"{GetType().Name}({_amount}, {_axis}, {_space})";
	}
}
