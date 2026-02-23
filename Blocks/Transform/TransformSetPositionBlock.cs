using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformSetPositionBlock : ScriptActionBlock
	{
		private readonly VariableBlock _position;

		public static TransformSetPositionBlock Create(VariableBlock position) => new(position);

		private TransformSetPositionBlock(VariableBlock position) => _position = position;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.Position = _position.GetValue<LunyVector3>();

		public override String ToString() => $"{GetType().Name}({_position})";
	}
}
