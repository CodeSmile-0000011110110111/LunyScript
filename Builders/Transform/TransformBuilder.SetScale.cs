using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Instantly set the local scale uniformly: value applied to XYZ. </summary>
		public TransformScaleSetBlock SetScale(Double uniformScale) => TransformScaleSetBlock.Create(LunyVector3.Uniform(uniformScale));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Instantly set the local scale uniformly: value applied to XYZ. </summary>
		public TransformScaleSetUniformBlock SetScale(VariableBlock uniformScale) => TransformScaleSetUniformBlock.Create(uniformScale);

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Instantly set the local scale. </summary>
		public TransformScaleSetBlock SetScale(VariableBlock<LunyVector3> scale) => TransformScaleSetBlock.Create(scale);
	}
}
