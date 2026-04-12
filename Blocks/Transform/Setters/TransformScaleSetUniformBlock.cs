using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetUniformBlock : ActionBlock
	{
		private readonly VariableBlock _uniformScale;

		public static TransformScaleSetUniformBlock Create(VariableBlock uniformScale) => new(uniformScale);

		private TransformScaleSetUniformBlock(VariableBlock uniformScale) => _uniformScale = uniformScale;

		protected internal override void Execute(IScriptRuntimeContext context) =>
			context.LunyObject.Transform.LocalScale = LunyVector3.Uniform(_uniformScale.Value);

		public override String ToString() => _uniformScale.ToString();
	}
}
