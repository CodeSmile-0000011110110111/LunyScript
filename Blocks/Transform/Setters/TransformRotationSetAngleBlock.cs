using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationSetAngleBlock : ActionBlock
	{
		private readonly LunyAxis _axis;
		private readonly Double _angle;
		private readonly LunyTransformSpace _space;

		public static TransformRotationSetAngleBlock Create(LunyAxis axis, Double value, LunyTransformSpace space, LunyStackTrace trace) =>
			new(axis, value, space, trace);

		private TransformRotationSetAngleBlock(LunyAxis axis, Double angle, LunyTransformSpace space, LunyStackTrace trace)
			: base(trace)
		{
			_axis = axis;
			_angle = angle;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			if (_space == LunyTransformSpace.World)
			{
				var euler = transform.EulerAngles;
				if (_axis == LunyAxis.X)
					transform.EulerAngles = new LunyVector3(_angle, euler.Y, euler.Z);
				else if (_axis == LunyAxis.Y)
					transform.EulerAngles = new LunyVector3(euler.X, _angle, euler.Z);
				else
					transform.EulerAngles = new LunyVector3(euler.X, euler.Y, _angle);
			}
			else
			{
				var euler = transform.LocalEulerAngles;
				if (_axis == LunyAxis.X)
					transform.LocalEulerAngles = new LunyVector3(_angle, euler.Y, euler.Z);
				else if (_axis == LunyAxis.Y)
					transform.LocalEulerAngles = new LunyVector3(euler.X, _angle, euler.Z);
				else
					transform.LocalEulerAngles = new LunyVector3(euler.X, euler.Y, _angle);
			}
		}

		public override String ToString()
		{
			var axis = _axis switch
			{
				LunyAxis.X => "X",
				LunyAxis.Y => "Y",
				LunyAxis.Z => "Z",
			};

			return $"{axis}={_angle}, {_space}";
		}
	}
}
