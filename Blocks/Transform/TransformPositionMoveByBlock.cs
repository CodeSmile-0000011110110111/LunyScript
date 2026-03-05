using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionMoveByBlock : ScriptActionBlock
	{
		private VariableBlock<LunyVector2> _direction;
		private VariableBlock _speed;
		private LunyTransformSpace _space;

		public static TransformPositionMoveByBlock
			Create(VariableBlock<LunyVector2> direction, VariableBlock speed, LunyTransformSpace space) => new(direction, speed, space);

		private TransformPositionMoveByBlock(VariableBlock<LunyVector2> direction, VariableBlock speed, LunyTransformSpace space)
		{
			_direction = direction ?? LunyVector2.One;
			_speed = speed ?? ConstantVariableBlock.Create(1);
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var direction = _direction.Value;
			var speed = _speed.Value;
			var translation = direction * (speed * LunyTime.DeltaTime);
			transform.Translate(translation, _space);
		}

		public override String ToString() => $"{GetType().Name}({_direction}, {_speed}, {_space})";
	}
}
