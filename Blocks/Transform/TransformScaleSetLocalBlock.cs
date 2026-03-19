using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetLocalBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector3> _scale;

		public static TransformScaleSetLocalBlock Create(VariableBlock<LunyVector3> scale) => new(scale);

		private TransformScaleSetLocalBlock(VariableBlock<LunyVector3> scale) => _scale = scale;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalScale = _scale.Value;

		public override String ToString() => $"{GetType().Name}({_scale})";
	}

	public sealed class TransformScaleSetLocalUniformBlock : ActionBlock
	{
		private readonly VariableBlock _uniformScale;

		public static TransformScaleSetLocalUniformBlock Create(VariableBlock uniformScale) => new(uniformScale);

		private TransformScaleSetLocalUniformBlock(VariableBlock uniformScale) => _uniformScale = uniformScale;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalScale = LunyVector3.Uniform(_uniformScale.Value);

		public override String ToString() => $"{GetType().Name}({_uniformScale})";
	}
}
