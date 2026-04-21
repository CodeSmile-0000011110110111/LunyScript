using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetUniformBlock : ActionBlock
	{
		private readonly VariableBlock _uniformScale;

		public static TransformScaleSetUniformBlock Create(VariableBlock uniformScale, LunyStackTrace trace) => new(uniformScale, trace);

		private TransformScaleSetUniformBlock(VariableBlock uniformScale, LunyStackTrace trace)
			: base(trace) => _uniformScale = uniformScale;

		protected internal override void Execute(IScriptRuntimeContext context) =>
			context.LunyGameObject.Transform.LocalScale = LunyVector3.Uniform(_uniformScale.Value);

		public override String ToString() => _uniformScale.ToString();
	}
}
