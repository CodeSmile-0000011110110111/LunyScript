using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformScaleSetBlock : ActionBlock
	{
		private readonly VariableBlock<LunyVector3> _scale;

		public static TransformScaleSetBlock Create(VariableBlock<LunyVector3> scale, LunyStackTrace trace) => new(scale, trace);

		private TransformScaleSetBlock(VariableBlock<LunyVector3> scale, LunyStackTrace trace)
			: base(trace) => _scale = scale;

		protected internal override void Execute(IScriptRuntimeContext context) => context.LunyGameObject.Transform.LocalScale = _scale.Value;

		public override String ToString() => $"{GetType().Name}({_scale})";
	}
}
