using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetWorldBlock : ScriptActionBlock
	{
		private readonly VariableBlock<LunyVector3> _position;

		public static TransformPositionSetWorldBlock Create(VariableBlock<LunyVector3> position) => new(position);

		private TransformPositionSetWorldBlock(VariableBlock<LunyVector3> position) => _position = position;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.Position = _position.Value;

		public override String ToString() => $"{GetType().Name}({_position})";
	}
}
