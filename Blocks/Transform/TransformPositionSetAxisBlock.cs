using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetAxisBlock : ActionBlock
	{
		private readonly LunyAxis _axis;
		private readonly Double _value;
		private readonly LunyTransformSpace _space;

		public static TransformPositionSetAxisBlock Create(LunyAxis axis, Double value, LunyTransformSpace space, LunyStackTrace trace) =>
			new(axis, value, space, trace);

		private TransformPositionSetAxisBlock(LunyAxis axis, Double value, LunyTransformSpace space, LunyStackTrace trace)
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
				var pos = transform.Position;
				if (_axis == LunyAxis.X)
					transform.Position = new LunyVector3(_value, pos.Y, pos.Z);
				else if (_axis == LunyAxis.Y)
					transform.Position = new LunyVector3(pos.X, _value, pos.Z);
				else
					transform.Position = new LunyVector3(pos.X, pos.Y, _value);
			}
			else
			{
				var pos = transform.LocalPosition;
				if (_axis == LunyAxis.X)
					transform.LocalPosition = new LunyVector3(_value, pos.Y, pos.Z);
				else if (_axis == LunyAxis.Y)
					transform.LocalPosition = new LunyVector3(pos.X, _value, pos.Z);
				else
					transform.LocalPosition = new LunyVector3(pos.X, pos.Y, _value);
			}
		}

		public override String ToString() => $"{GetType().Name}({_axis}, {_value}, {_space})";
	}
}
