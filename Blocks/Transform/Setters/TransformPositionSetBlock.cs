using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector3> _position;
		private readonly LunyTransformSpace _space;

		public static TransformPositionSetBlock Create(VariableBlock<LunyVector3> position, LunyTransformSpace space, LunyStackTrace trace) =>
			new(position, space, trace);

		private TransformPositionSetBlock(VariableBlock<LunyVector3> position, LunyTransformSpace space, LunyStackTrace trace)
			: base(trace)
		{
			_position = position;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyGameObject.Transform;

			if (_space == LunyTransformSpace.World)
				transform.Position = _position.Value;
			else
				transform.LocalPosition = _position.Value;
		}

		public override String ToString() => $"{_position}, {_space}";
	}
}
