using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformMoveAbsoluteBlock : ScriptActionBlock
	{
		private VariableBlock _distance;
		private LunyVector3 _axis;
		private VariableBlock _speed;
		private LunySpace _space;

		public static TransformMoveAbsoluteBlock Create(VariableBlock distance, LunyVector3 axis, VariableBlock speed, LunySpace space) =>
			new(distance, axis, speed, space);

		private TransformMoveAbsoluteBlock(VariableBlock distance, LunyVector3 axis, VariableBlock speed, LunySpace space)
		{
			_distance = distance;
			_axis = axis;
			_speed = speed;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var deltaTime = LunyEngine.Instance.Time.DeltaTime;
			var distance = _distance.GetValue<Double>();
			var speed = _speed.GetValue<Double>();
			var translation = distance * _axis * (speed * deltaTime);
			transform.Translate(translation, _space);
		}

		public override String ToString() => $"{GetType().Name}({_distance}, {_axis}, {_speed}, {_space})";
	}
}
