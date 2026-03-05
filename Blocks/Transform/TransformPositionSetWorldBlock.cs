using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetWorldBlock : ScriptActionBlock
	{
		private readonly VariableBlock _position;

		public static TransformPositionSetWorldBlock Create(VariableBlock position) => new(position);

		private TransformPositionSetWorldBlock(VariableBlock position) => _position = position;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.Position = _position.GetValue<LunyVector3>();

		public override String ToString() => $"{GetType().Name}({_position})";
	}
}
