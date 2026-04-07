using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetLocalBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector3> _position;

		public static TransformPositionSetLocalBlock Create(VariableBlock<LunyVector3> position) => new(position);

		private TransformPositionSetLocalBlock(VariableBlock<LunyVector3> position) => _position = position;

		protected internal override void Execute(IScriptRuntimeContext context) =>
			context.LunyObject.Transform.LocalPosition = _position.Value;

		public override String ToString() => $"{GetType().Name}({_position})";
	}
}
