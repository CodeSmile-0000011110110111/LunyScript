using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Instantly set the local scale uniformly: value applied to XYZ. </summary>
		public TransformScaleSetUniformBlock SetScale(Double uniformScale) =>
			TransformScaleSetUniformBlock.Create(LiteralVariableBlock.Create(uniformScale, _trace), _trace.Add(nameof(SetScale)));

		/// <summary> Instantly set the local scale uniformly: value applied to XYZ. </summary>
		public TransformScaleSetUniformBlock SetScale(VariableBlock uniformScale) =>
			TransformScaleSetUniformBlock.Create(uniformScale, _trace.Add(nameof(SetScale)));

		/// <summary> Instantly set the local scale. </summary>
		public TransformScaleSetBlock SetScale(VariableBlock<LunyVector3> scale) =>
			TransformScaleSetBlock.Create(scale, _trace.Add(nameof(SetScale)));
	}
}
