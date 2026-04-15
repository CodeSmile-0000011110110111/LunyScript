using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateByBlock : ActionBlock
	{
		private readonly VariableBlock _deltaAngle;
		private readonly VariableBlock<LunyVector3> _eulerAnglesPerSecond;
		private readonly VariableBlock _speed;
		private readonly LunyVector3 _axis;
		private readonly Double _minAngle;
		private readonly Double _maxAngle;
		private readonly LunyTransformSpace _space;
		private readonly Boolean _useEuler;
		private Double _currentAngle;
		private Double _previousAngle;

		public static TransformRotateByBlock Create(VariableBlock amount, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, Double minAngle = Double.NegativeInfinity, Double maxAngle = Double.PositiveInfinity,
			LunyStackTrace trace = null) => new(amount, axis, speed, space, minAngle, maxAngle, trace);

		public static TransformRotateByBlock CreateEuler(VariableBlock<LunyVector3> eulerAnglesPerSecond, VariableBlock speed,
			LunyTransformSpace space, LunyStackTrace trace = null) => new(eulerAnglesPerSecond, speed, space, trace);

		private TransformRotateByBlock(VariableBlock deltaAngle, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, Double minAngle, Double maxAngle, LunyStackTrace trace)
			: base(trace)
		{
			_deltaAngle = deltaAngle;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_axis = axis;
			_space = space;
			_minAngle = minAngle;
			_maxAngle = maxAngle;
			_useEuler = false;
		}

		private TransformRotateByBlock(VariableBlock<LunyVector3> eulerAnglesPerSecond, VariableBlock speed,
			LunyTransformSpace space, LunyStackTrace trace)
			: base(trace)
		{
			_eulerAnglesPerSecond = eulerAnglesPerSecond;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_space = space;
			_minAngle = Double.NegativeInfinity;
			_maxAngle = Double.PositiveInfinity;
			_useEuler = true;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;

			if (_useEuler)
			{
				var eulerDelta = _eulerAnglesPerSecond.Value * (_speed.Value * LunyTime.DeltaTime);
				var deltaRotation = LunyQuaternion.Euler(eulerDelta);
				if (_space == LunyTransformSpace.World)
					transform.Rotation = deltaRotation * transform.Rotation; // delta intentionally is on the left-side, can't use *= !
				else
					transform.LocalRotation *= deltaRotation;
				return;
			}

			var deltaAngle = _deltaAngle.Value * (_speed.Value * LunyTime.DeltaTime);
			var hasClamp = _minAngle != Double.NegativeInfinity || _maxAngle != Double.PositiveInfinity;
			if (hasClamp)
			{
				_currentAngle = Math.Clamp(_currentAngle + deltaAngle, _minAngle, _maxAngle);
				var actualDelta = _currentAngle - _previousAngle;
				_previousAngle = _currentAngle;
				deltaAngle = actualDelta;
			}

			var rotation = LunyQuaternion.AngleAxis(deltaAngle, _axis);
			if (_space == LunyTransformSpace.World)
				transform.Rotation = rotation * transform.Rotation;
			else
				transform.LocalRotation *= rotation;
		}

		public override String ToString() => _useEuler
			? $"Euler={_eulerAnglesPerSecond}, {_space}"
			: $"Axis={_axis}, Delta={_deltaAngle}, {_space}";
	}
}
