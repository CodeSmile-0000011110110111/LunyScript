using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveByBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector2> _direction;
		private readonly VariableBlock _amount;
		private readonly LunyVector3 _axis;
		private readonly VariableBlock _speed;
		private readonly LunyTransformSpace _space;
		private readonly Boolean _useDirection;

		public static TransformMoveByBlock CreatePlaneMoveBy(VariableBlock<LunyVector2> direction, VariableBlock speed,
			LunyTransformSpace space, LunyStackTrace trace) => new(direction, null, default, speed, space, trace);

		public static TransformMoveByBlock CreateAxisMoveBy(VariableBlock amount, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, LunyStackTrace trace) => new(null, amount, axis, speed, space, trace);

		private TransformMoveByBlock(VariableBlock<LunyVector2> direction, VariableBlock amount, LunyVector3 axis,
			VariableBlock speed, LunyTransformSpace space, LunyStackTrace trace)
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
			var speed = _speed.Value * LunyTime.DeltaTime;
			var translation = _useDirection ? _direction.Value * speed : _axis * (_amount.Value * speed);
			transform.Translate(translation, _space);
		}

		public override String ToString() => _useDirection
			? $"By{_direction}, Speed{_speed}, {_space}"
			: $"{_axis}={_amount}, Speed{_speed}, {_space}";
	}
}
