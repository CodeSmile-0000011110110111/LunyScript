using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformTranslateBlock : ScriptActionBlock
	{
		private VariableBlock _direction;
		private VariableBlock _speed;
		private LunySpace _space;

		public static TransformTranslateBlock Create(VariableBlock direction, VariableBlock speed, LunySpace space) =>
			new(direction, speed, space);

		private TransformTranslateBlock(VariableBlock direction, VariableBlock speed, LunySpace space)
		{
			_direction = direction;
			_speed = speed;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var direction = _direction?.GetValue<LunyVector2>() ?? LunyVector2.Zero;
			var deltaTime = LunyEngine.Instance.Time.DeltaTime;
			var speed = _speed?.GetValue<Double>() ?? 1d;
			var translation = direction * (speed * deltaTime);
			transform.Translate(translation, _space);
		}

		public override String ToString() => $"{GetType().Name}({_direction}, {_speed}, {_space})";
	}
}
