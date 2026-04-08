using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct TransformMoveTerminalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly VariableBlock<LunyVector2> _direction;
		private readonly VariableBlock _amount;
		private readonly LunyVector3 _axis;
		private readonly VariableBlock _speed;
		private readonly LunyTransformSpace _space;
		private readonly Boolean _useDirection;
		private readonly StackTrace _trace;

		internal static TransformMoveTerminalBuilder CreateDirectional(Script script, VariableBlock<LunyVector2> direction,
			VariableBlock speed, LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveTerminalBuilder), "Transform.Move(direction)");
			return new TransformMoveTerminalBuilder(script, token, direction, null, default, speed, space, useDirection: true, trace);
		}

		internal static TransformMoveTerminalBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyVector3 axis,
			VariableBlock speed, LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveTerminalBuilder), "Transform.Move(axis)");
			return new TransformMoveTerminalBuilder(script, token, null, amount, axis, speed, space, useDirection: false, trace);
		}

		private TransformMoveTerminalBuilder(Script script, BuilderToken token, VariableBlock<LunyVector2> direction,
			VariableBlock amount, LunyVector3 axis, VariableBlock speed, LunyTransformSpace space, Boolean useDirection, StackTrace trace)
		{
			_script = script;
			_token = token;
			_direction = direction;
			_amount = amount;
			_axis = axis;
			_speed = speed;
			_space = space;
			_useDirection = useDirection;
			_trace = trace;

			var self = this;
			token.AutoFinish = () => self.Finish();
		}

		/// <summary> Apply movement in world space instead of local space. </summary>
		public TransformPositionMoveBlock InWorldSpace() => Finish(LunyTransformSpace.World);

		internal TransformPositionMoveBlock Finish() => Finish(_space);

		private TransformPositionMoveBlock Finish(LunyTransformSpace space)
		{
			_script.MarkBuilderTokenFinished(_token);
			return _useDirection
				? TransformPositionMoveBlock.CreateDirectional(_direction, _speed, space, _trace)
				: TransformPositionMoveBlock.CreateAxisRelative(_amount, _axis, _speed, space, _trace);
		}
	}
}
