using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformMoveByBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector2> _direction;
		private readonly VariableBlock<LunyVector3> _vector3Direction;
		private readonly VariableBlock _amount;
		private readonly LunyVector3 _axis;
		private readonly VariableBlock _speed;
		private readonly LunyTransformSpace _space;
		private readonly Boolean _useDirection;
		private readonly Boolean _useVector3;
		private readonly Boolean _useBoxClamp;
		private readonly Boolean _useSphereClamp;
		private readonly LunyVector3 _boxMin;
		private readonly LunyVector3 _boxMax;
		private readonly Single _sphereRadius;
		private LunyVector3 _offset;

		public static TransformMoveByBlock CreatePlaneMoveBy(VariableBlock<LunyVector2> direction, VariableBlock speed,
			LunyTransformSpace space, LunyStackTrace trace) =>
			new(direction, null, null, default, speed, space, false, false, default, default, 0f, trace);

		public static TransformMoveByBlock CreateAxisMoveBy(VariableBlock amount, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, LunyStackTrace trace) =>
			new(null, null, amount, axis, speed, space, false, false, default, default, 0f, trace);

		public static TransformMoveByBlock CreateAxisMoveByWithClamp(VariableBlock amount, LunyVector3 axis, VariableBlock speed,
			LunyTransformSpace space, Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace) => new(null, null, amount, axis, speed, space, useBoxClamp,
			useSphereClamp, boxMin, boxMax, sphereRadius, trace);

		public static TransformMoveByBlock CreateVector3MoveBy(VariableBlock<LunyVector3> direction, VariableBlock speed,
			LunyTransformSpace space, Boolean useBoxClamp, LunyVector3 boxMin, LunyVector3 boxMax,
			Boolean useSphereClamp, Single sphereRadius, LunyStackTrace trace) => new(null, direction, null, default, speed, space, useBoxClamp,
			useSphereClamp, boxMin, boxMax, sphereRadius, trace);

		private TransformMoveByBlock(VariableBlock<LunyVector2> direction, VariableBlock<LunyVector3> vector3Direction,
			VariableBlock amount, LunyVector3 axis, VariableBlock speed, LunyTransformSpace space,
			Boolean useBoxClamp, Boolean useSphereClamp, LunyVector3 boxMin, LunyVector3 boxMax, Single sphereRadius,
			LunyStackTrace trace)
			: base(trace)
		{
			_useDirection = direction != null;
			_useVector3 = vector3Direction != null;
			_direction = direction ?? LunyVector2.One;
			_vector3Direction = vector3Direction;
			_amount = amount ?? LiteralVariableBlock.Create(1, trace);
			_axis = axis;
			_speed = speed ?? LiteralVariableBlock.Create(1, trace);
			_space = space;
			_useBoxClamp = useBoxClamp;
			_useSphereClamp = useSphereClamp;
			_boxMin = boxMin;
			_boxMax = boxMax;
			_sphereRadius = sphereRadius;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var speed = _speed.Value * LunyTime.DeltaTime;

			if (_useDirection)
			{
				var translation = _direction.Value * speed;
				transform.Translate(translation, _space);
				return;
			}

			LunyVector3 delta;
			if (_useVector3)
				delta = _vector3Direction.Value * speed;
			else
				delta = _axis * (_amount.Value * speed);

			var previousOffset = _offset;
			_offset += delta;

			if (_useSphereClamp)
				_offset = LunyVector3.ClampMagnitude(_offset, _sphereRadius);

			if (_useBoxClamp)
				_offset = LunyVector3.Max(_boxMin, LunyVector3.Min(_boxMax, _offset));

			var clampedDelta = _offset - previousOffset;
			transform.Translate(clampedDelta, _space);
		}

		public override String ToString() => _useDirection
			? $"By{_direction}, Speed{_speed}, {_space}"
			: _useVector3
				? $"Vec3={_vector3Direction}, Speed{_speed}, {_space}"
				: $"{_axis}={_amount}, Speed{_speed}, {_space}";
	}
}
