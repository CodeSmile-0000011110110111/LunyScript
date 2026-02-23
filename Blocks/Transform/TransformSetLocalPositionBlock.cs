using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformSetLocalPositionBlock : ScriptActionBlock
	{
		private readonly VariableBlock _position;

		public static TransformSetLocalPositionBlock Create(VariableBlock position) => new(position);

		private TransformSetLocalPositionBlock(VariableBlock position) => _position = position;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalPosition = _position.GetValue<LunyVector3>();

		public override String ToString() => $"{GetType().Name}({_position})";
	}
}
