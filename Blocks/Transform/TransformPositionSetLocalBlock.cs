using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformPositionSetLocalBlock : ScriptActionBlock
	{
		private readonly VariableBlock _position;

		public static TransformPositionSetLocalBlock Create(VariableBlock position) => new(position);

		private TransformPositionSetLocalBlock(VariableBlock position) => _position = position;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalPosition = _position.GetValue<LunyVector3>();

		public override String ToString() => $"{GetType().Name}({_position})";
	}
}
