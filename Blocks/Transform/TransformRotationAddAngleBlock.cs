using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationAddAngleBlock : ActionBlock
	{
		private readonly VariableBlock _deltaAngle;
		private readonly VariableBlock _speed;
		private readonly LunyVector3 _angleAxis;
		private readonly Double _minAngle;
		private readonly Double _maxAngle;
		private readonly LunyTransformSpace _space;
		private Double _currentAngle;

		public static TransformRotationAddAngleBlock Create(VariableBlock deltaAngle, VariableBlock speed, LunyVector3 angleAxis,
			LunyTransformSpace space, Double minAngle = Double.NegativeInfinity, Double maxAngle = Double.PositiveInfinity,
			StackTrace trace = null) => new(deltaAngle, speed, angleAxis, space, minAngle, maxAngle, trace);

		private TransformRotationAddAngleBlock(VariableBlock deltaAngle, VariableBlock speed, LunyVector3 angleAxis,
			LunyTransformSpace space, Double minAngle, Double maxAngle, StackTrace trace)
			: base(trace)
		{
			_deltaAngle = deltaAngle;
			_speed = speed;
			_angleAxis = angleAxis;
			_space = space;
			_minAngle = minAngle;
			_maxAngle = maxAngle;
			_currentAngle = 0.0;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var time = LunyEngine.Instance.Time;
			var deltaAngle = _deltaAngle.Value * _speed.Value * time.DeltaTime;
			_currentAngle = Math.Clamp(_currentAngle + deltaAngle, _minAngle, _maxAngle);
			var transform = context.LunyObject.Transform;
			if (_space == LunyTransformSpace.World)
				transform.Rotation = LunyQuaternion.AngleAxis(_currentAngle, _angleAxis);
			else
				transform.LocalRotation = LunyQuaternion.AngleAxis(_currentAngle, _angleAxis);
		}

		public override String ToString() => $"{GetType().Name}({_deltaAngle}, {_angleAxis}, {_space})";
	}
}
