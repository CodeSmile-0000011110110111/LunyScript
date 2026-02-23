using System;

namespace LunyScript.Blocks.Transform
{
	/// <summary>
	/// Abstract base for Towards blocks whose target is a <see cref="VariableBlock"/> value
	/// (Scale towards).
	/// </summary>
	public abstract class TransformTowardsVariableBlock : TransformTowardsBlock
	{
		protected readonly VariableBlock TargetScale;

		protected TransformTowardsVariableBlock(
			VariableBlock targetScale,
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness)
			: base(speed, deadZone, lockX, lockY, lockZ, responsiveness)
		{
			TargetScale = targetScale;
		}

		protected String TowardsVariableToString() => $"{TargetScale}, {TowardsToString()}";
	}
}
