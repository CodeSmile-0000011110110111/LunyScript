using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionMoveByBlock : ActionBlock
	{
		private VariableBlock<LunyVector2> _direction;
		private VariableBlock _speed;
		private LunyTransformSpace _space;

		public static TransformPositionMoveByBlock Create(VariableBlock<LunyVector2> direction, VariableBlock speed, LunyTransformSpace space,
			StackTrace trace) => new(direction, speed, space, trace);

		private TransformPositionMoveByBlock(VariableBlock<LunyVector2> direction, VariableBlock speed, LunyTransformSpace space,
			StackTrace trace)
			: base(trace)
		{
			_direction = direction ?? LunyVector2.One;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var direction = _direction.Value;
			var speed = _speed.Value;
			var translation = direction * (speed * LunyTime.DeltaTime);
			transform.Translate(translation, _space);
		}

		public override String ToString() => $"({_direction}, {_speed}, {_space})";
	}
}
