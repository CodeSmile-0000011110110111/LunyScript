using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that interpolate or step a transform property toward a target over time.
	/// Holds the shared optional parameters: speed, deadZone, axisLock, responsiveness.
	/// </summary>
	public abstract class TransformTowardsBlock : ScriptActionBlock
	{
		protected readonly Single Speed;
		protected readonly Single DeadZone;
		protected readonly LunyVector3 AxisLock;
		protected readonly Single Responsiveness;

		protected TransformTowardsBlock(
			Double speed,
			Double deadZone,
			Boolean lockX,
			Boolean lockY,
			Boolean lockZ,
			Double responsiveness)
		{
			Speed = speed > 0f ? (Single)speed : 1f;
			DeadZone = (Single)deadZone;
			AxisLock = new LunyVector3(lockX ? 0d : 1d, lockY ? 0d : 1d, lockZ ? 0d : 1d);
			Responsiveness = responsiveness > 0f ? (Single)responsiveness : 1f;
		}

		protected String TowardsToString() => $"speed={Speed}, deadZone={DeadZone}, axisLock={AxisLock}, responsiveness={Responsiveness}";
	}
}
