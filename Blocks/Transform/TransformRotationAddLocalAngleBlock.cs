using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationAddLocalAngleBlock : ActionBlock
	{
		private readonly VariableBlock _deltaAngle;
		private readonly VariableBlock _speed;
		private LunyVector3 _angleAxis;
		private Double _minAngle;
		private Double _maxAngle;
		private Double _currentAngle;

		public static TransformRotationAddLocalAngleBlock Create(VariableBlock deltaAngle, VariableBlock speed, LunyVector3 angleAxis,
			Double minAngle = Double.NegativeInfinity, Double maxAngle = Double.PositiveInfinity) =>
			new(deltaAngle, speed, angleAxis, minAngle, maxAngle);

		private TransformRotationAddLocalAngleBlock(VariableBlock deltaAngle, VariableBlock speed, LunyVector3 angleAxis, Double minAngle,
			Double maxAngle)
		{
			_deltaAngle = deltaAngle;
			_speed = speed;
			_angleAxis = angleAxis;
			_minAngle = minAngle;
			_maxAngle = maxAngle;
			_currentAngle = 0f;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var time = LunyEngine.Instance.Time;

			// Calculate how much we CAN rotate without exceeding limits
			var deltaAngle = _deltaAngle.Value * _speed.Value * time.DeltaTime;
			_currentAngle = Math.Clamp(_currentAngle + deltaAngle, _minAngle, _maxAngle);

			var transform = context.LunyObject.Transform;
			transform.LocalRotation = LunyQuaternion.AngleAxis(_currentAngle, _angleAxis);
		}

		public override String ToString() => $"{GetType().Name}({_deltaAngle})";
	}
}
