using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotateByBlock : ActionBlock
	{
		private readonly VariableBlock _deltaAngle;
		private readonly LunyAxis _axis;
		private readonly Double _minAngle;
		private readonly Double _maxAngle;
		private readonly LunyTransformSpace _space;
		private Double _currentAngle;

		public static TransformRotateByBlock Create(VariableBlock amount, LunyAxis axis,
			LunyTransformSpace space, Double minAngle = Double.NegativeInfinity, Double maxAngle = Double.PositiveInfinity,
			LunyStackTrace trace = null) => new(amount, axis, space, minAngle, maxAngle, trace);

		private TransformRotateByBlock(VariableBlock deltaAngle, LunyAxis axis,
			LunyTransformSpace space, Double minAngle, Double maxAngle, LunyStackTrace trace)
			: base(trace)
		{
			_deltaAngle = deltaAngle;
			_axis = axis;
			_space = space;
			_minAngle = minAngle;
			_maxAngle = maxAngle;
			_currentAngle = 0.0;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var time = LunyEngine.Instance.Time;
			var transform = context.LunyObject.Transform;

			var deltaAngle = _deltaAngle.Value * time.DeltaTime;
			_currentAngle = Math.Clamp(_currentAngle + deltaAngle, _minAngle, _maxAngle);

			if (_space == LunyTransformSpace.World)
				transform.Rotation = LunyQuaternion.AngleAxis(_currentAngle, _axis.ToVector3());
			else
				transform.LocalRotation = LunyQuaternion.AngleAxis(_currentAngle, _axis.ToVector3());
		}

		public override String ToString()
		{
			var axis = _axis switch
			{
				LunyAxis.X => "X",
				LunyAxis.Y => "Y",
				LunyAxis.Z => "Z",
			};

			return $"{axis}={_deltaAngle}, {_space}";
		}
	}
}
