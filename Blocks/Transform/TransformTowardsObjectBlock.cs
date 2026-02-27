using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for Towards blocks whose target is an <see cref="ILunyObject"/> in the scene
	/// (Move and Rotate towards).
	/// </summary>
	public abstract class TransformTowardsObjectBlock : TransformTowardsBlock
	{
		protected readonly ILunyObject Target;

		protected TransformTowardsObjectBlock(
			ILunyObject target,
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness)
			: base(speed, deadZone, lockX, lockY, lockZ, responsiveness)
		{
			Target = target;
		}

		protected String TowardsObjectToString() => $"{Target}, {TowardsToString()}";
	}
}
