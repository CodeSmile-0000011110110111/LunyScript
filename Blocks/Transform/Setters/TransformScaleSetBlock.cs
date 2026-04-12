using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector3> _scale;

		public static TransformScaleSetBlock Create(VariableBlock<LunyVector3> scale) => new(scale);

		private TransformScaleSetBlock(VariableBlock<LunyVector3> scale) => _scale = scale;

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyObject.Transform.LocalScale = _scale.Value;

		public override String ToString() => $"{GetType().Name}({_scale})";
	}
}
