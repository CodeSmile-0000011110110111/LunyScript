using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetLocalBlock : ScriptActionBlock
	{
		private readonly VariableBlock<LunyVector3> _scale;

		public static TransformScaleSetLocalBlock Create(VariableBlock<LunyVector3> scale) => new(scale);

		private TransformScaleSetLocalBlock(VariableBlock<LunyVector3> scale) => _scale = scale;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalScale = _scale.Value;

		public override String ToString() => $"{GetType().Name}({_scale})";
	}
}
