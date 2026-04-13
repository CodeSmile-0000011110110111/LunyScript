using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that interpolate or step a transform property toward a target over time.
	/// Holds the shared optional parameters: speed, deadZone, lockAxis, responsiveness.
	/// </summary>
	public abstract class TransformInterpolateBlock : ActionBlock
	{
		private readonly Double Speed;

		protected readonly Double DeadZone;
		protected readonly LunyVector3 LockAxis;

		protected TransformInterpolateBlock(Double speed, Double deadZone, LunyVector3 lockAxis, LunyStackTrace trace)
			: base(trace)
		{
			Speed = speed > 0 ? speed : 1;
			DeadZone = deadZone;
			LockAxis = lockAxis;
		}

		/// <summary>Returns Speed * deltaTime * Responsiveness, ready to use as a step or lerp t value.</summary>
		protected Double ComputeStep() => Speed * LunyTime.DeltaTime;

		protected String ParametersToString() => $"Speed({Speed}), DeadZone({DeadZone}), Axis={LockAxis})";
	}
}
