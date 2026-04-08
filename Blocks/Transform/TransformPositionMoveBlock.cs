using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionMoveBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector2> _direction;
		private readonly VariableBlock _amount;
		private readonly LunyVector3 _axis;
		private readonly VariableBlock _speed;
		private readonly LunyTransformSpace _space;
		private readonly Boolean _useDirection;

		public static TransformPositionMoveBlock CreateDirectional(VariableBlock<LunyVector2> direction, VariableBlock speed,
			LunyTransformSpace space, StackTrace trace) => new(direction, null, default, speed, space, trace);

		public static TransformPositionMoveBlock CreateAxisRelative(VariableBlock amount, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, StackTrace trace) => new(null, amount, axis, speed, space, trace);

		private TransformPositionMoveBlock(VariableBlock<LunyVector2> direction, VariableBlock amount, LunyVector3 axis,
			VariableBlock speed, LunyTransformSpace space, StackTrace trace)
			: base(trace)
		{
			_useDirection = direction != null;
			_direction = direction ?? LunyVector2.One;
			_amount = amount ?? LiteralVariableBlock.Create(1, trace);
			_axis = axis;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var speed = _speed.Value;
			if (_useDirection)
			{
				var translation = _direction.Value * (speed * LunyTime.DeltaTime);
				transform.Translate(translation, _space);
			}
			else
			{
				var translation = _amount.Value * _axis * (speed * LunyTime.DeltaTime);
				transform.Translate(translation, _space);
			}
		}

		public override String ToString() => _useDirection
			? $"{GetType().Name}({_direction}, {_speed}, {_space})"
			: $"{GetType().Name}({_amount}, {_axis}, {_speed}, {_space})";
	}
}
