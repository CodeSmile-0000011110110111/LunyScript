using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformMoveBlock : ScriptActionBlock
	{
		private VariableBlock _direction;
		private Single _speed;
		private LunySpace _space;

		public static TransformMoveBlock Create(VariableBlock direction, Double speed, LunySpace space = LunySpace.Self) => new(direction, speed, space);

		private TransformMoveBlock(VariableBlock direction, Double speed, LunySpace space)
		{
			_direction = direction;
			_speed = (Single)speed;
			_space =  space;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var direction = _direction.GetValue<LunyVector2>(runtimeContext);
			transform.Translate(direction * _speed, _space);
		}

		public override String ToString() => $"Transform.Move({_direction}, {_speed}, {_space})";
	}
}
