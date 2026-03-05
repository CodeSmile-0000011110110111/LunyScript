using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetLocalBlock : ScriptActionBlock
	{
		private readonly VariableBlock _scale;

		public static TransformScaleSetLocalBlock Create(VariableBlock scale) => new(scale);

		private TransformScaleSetLocalBlock(VariableBlock scale) => _scale = scale;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalScale = _scale.GetValue<LunyVector3>();

		public override String ToString() => $"{GetType().Name}({_scale})";
	}
}
