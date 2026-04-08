using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationSetAxisBlock : ActionBlock
	{
		private readonly LunyAxis _axis;
		private readonly Double _value;
		private readonly LunyTransformSpace _space;

		public static TransformRotationSetAxisBlock Create(LunyAxis axis, Double value, LunyTransformSpace space, StackTrace trace) =>
			new(axis, value, space, trace);

		private TransformRotationSetAxisBlock(LunyAxis axis, Double value, LunyTransformSpace space, StackTrace trace)
			: base(trace)
		{
			_axis = axis;
			_value = value;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			if (_space == LunyTransformSpace.World)
			{
				var euler = transform.EulerAngles;
				if (_axis == LunyAxis.X)
					transform.EulerAngles = new LunyVector3(_value, euler.Y, euler.Z);
				else if (_axis == LunyAxis.Y)
					transform.EulerAngles = new LunyVector3(euler.X, _value, euler.Z);
				else
					transform.EulerAngles = new LunyVector3(euler.X, euler.Y, _value);
			}
			else
			{
				var euler = transform.LocalEulerAngles;
				if (_axis == LunyAxis.X)
					transform.LocalEulerAngles = new LunyVector3(_value, euler.Y, euler.Z);
				else if (_axis == LunyAxis.Y)
					transform.LocalEulerAngles = new LunyVector3(euler.X, _value, euler.Z);
				else
					transform.LocalEulerAngles = new LunyVector3(euler.X, euler.Y, _value);
			}
		}

		public override String ToString() => $"{GetType().Name}({_axis}, {_value}, {_space})";
	}
}
