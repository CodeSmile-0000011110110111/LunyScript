using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetSingleAxisBlock : ActionBlock
	{
		private readonly LunyAxis _axis;
		private readonly Double _value;
		private readonly LunyTransformSpace _space;

		public static TransformPositionSetSingleAxisBlock Create(LunyAxis axis, Double value, LunyTransformSpace space, LunyStackTrace trace) =>
			new(axis, value, space, trace);

		internal TransformPositionSetSingleAxisBlock(LunyAxis axis, Double value, LunyTransformSpace space, LunyStackTrace trace)
			: base(trace)
		{
			_axis = axis;
			_value = value;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyGameObject.Transform;
			var pos = _space == LunyTransformSpace.World ? transform.Position : transform.LocalPosition;

			pos = _axis switch
			{
				LunyAxis.X => new LunyVector3(_value, pos.Y, pos.Z),
				LunyAxis.Y => new LunyVector3(pos.X, _value, pos.Z),
				LunyAxis.Z => new LunyVector3(pos.X, pos.Y, _value),
			};

			if (_space == LunyTransformSpace.World)
				transform.Position = pos;
			else
				transform.LocalPosition = pos;
		}

		public override String ToString()
		{
			var axis = _axis switch
			{
				LunyAxis.X => "X",
				LunyAxis.Y => "Y",
				LunyAxis.Z => "Z",
			};

			return $"{axis}={_value}, {_space}";
		}
	}
}
