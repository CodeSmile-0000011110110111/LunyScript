using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionTerminalBuilder SetPosition(Double x, Double y, Double z) => TransformSetPositionTerminalBuilder.Create(
			_script, new LunyVector3(x, y, z), LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));

		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionTerminalBuilder SetPosition(VariableBlock<LunyVector3> position) =>
			TransformSetPositionTerminalBuilder.Create(_script, position, LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));

		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationTerminalBuilder SetRotation(Double xAngle, Double yAngle, Double zAngle) =>
			TransformSetRotationTerminalBuilder.Create(_script, LunyQuaternion.Euler(new LunyVector3(xAngle, yAngle, zAngle)),
				LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationTerminalBuilder SetRotation(LunyVector3 eulerAngles) => TransformSetRotationTerminalBuilder.Create(_script,
			LunyQuaternion.Euler(eulerAngles), LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationTerminalBuilder SetRotation(VariableBlock<LunyQuaternion> rotation) =>
			TransformSetRotationTerminalBuilder.Create(_script, rotation, LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Instantly set the local scale. </summary>
		public TransformScaleSetBlock SetScale(Double uniformScale) => TransformScaleSetBlock.Create(LunyVector3.Uniform(uniformScale));

		[NeedsReview] [NeedsSmokeTest]
		public TransformScaleSetUniformBlock SetScale(VariableBlock uniformScale) => TransformScaleSetUniformBlock.Create(uniformScale);

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Instantly set the local scale. </summary>
		public TransformScaleSetBlock SetScale(VariableBlock<LunyVector3> scale) => TransformScaleSetBlock.Create(scale);

	}
}
