using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionMoveRelativeBlock : ActionBlock
	{
		private VariableBlock _distance;
		private LunyVector3 _axis;
		private VariableBlock _speed;
		private LunyTransformSpace _space;

		public static TransformPositionMoveRelativeBlock Create(VariableBlock distance, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, StackTrace trace) => new(distance, axis, speed, space, trace);

		private TransformPositionMoveRelativeBlock(VariableBlock distance, LunyVector3 axis, VariableBlock speed, LunyTransformSpace space,
			StackTrace trace)
			: base(trace)
		{
			_distance = distance ?? LiteralVariableBlock.Create(1, trace);
			_axis = axis;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var distance = _distance.Value;
			var speed = _speed.Value;
			var translation = distance * _axis * (speed * LunyTime.DeltaTime);
			transform.Translate(translation, _space);
		}

		public override String ToString() => $"{GetType().Name}({_distance}, {_axis}, {_speed}, {_space})";
	}
}
