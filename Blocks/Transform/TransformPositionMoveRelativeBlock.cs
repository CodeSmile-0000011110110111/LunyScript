using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionMoveRelativeBlock : ScriptActionBlock
	{
		private VariableBlock _distance;
		private LunyVector3 _axis;
		private VariableBlock _speed;
		private LunySpace _space;

		public static TransformPositionMoveRelativeBlock Create(VariableBlock distance, LunyVector3 axis, VariableBlock speed, LunySpace space) =>
			new(distance, axis, speed, space);

		private TransformPositionMoveRelativeBlock(VariableBlock distance, LunyVector3 axis, VariableBlock speed, LunySpace space)
		{
			_distance = distance;
			_axis = axis;
			_speed = speed ?? ConstantVariableBlock.Create(1);
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var distance = _distance.GetValue<Double>();
			var speed = _speed.GetValue<Double>();
			var translation = distance * _axis * (speed * LunyTime.DeltaTime);
			transform.Translate(translation, _space);
		}

		public override String ToString() => $"{GetType().Name}({_distance}, {_axis}, {_speed}, {_space})";
	}
}
