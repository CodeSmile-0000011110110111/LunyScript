using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformTranslateBlock : ScriptActionBlock
	{
		private VariableBlock _direction;
		private Single _speed;
		private LunySpace _space;

		public static TransformTranslateBlock Create(VariableBlock direction, Double speed, LunySpace space) =>
			new(direction, speed, space);

		private TransformTranslateBlock(VariableBlock direction, Double speed, LunySpace space)
		{
			_direction = direction;
			_speed = (Single)speed;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var direction = _direction.GetValue<LunyVector2>(runtimeContext);
			var deltaTime = LunyEngine.Instance.Time.DeltaTime;
			transform.Translate(direction * (_speed * deltaTime), _space);
		}

		public override String ToString() => $"Transform.Move({_direction}, {_speed}, {_space})";
	}
}
