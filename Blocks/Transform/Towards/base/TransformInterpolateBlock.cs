using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that interpolate or step a transform property toward a target over time.
	/// Holds the shared optional parameters: speed, deadZone, axisLock, responsiveness.
	/// </summary>
	public abstract class TransformInterpolateBlock : ActionBlock
	{
		private readonly Double Speed;

		protected readonly Double DeadZone;
		protected readonly LunyVector3 AxisLock;

		protected TransformInterpolateBlock(Double speed, Double deadZone, LunyVector3 axisLock, LunyStackTrace trace)
			: base(trace)
		{
			Speed = speed > 0 ? speed : 1;
			DeadZone = deadZone;
			AxisLock = axisLock;
		}

		/// <summary>Returns Speed * deltaTime * Responsiveness, ready to use as a step or lerp t value.</summary>
		protected Double ComputeStep() => Speed * LunyTime.DeltaTime;

		protected String ParametersToString() => $"Speed({Speed}), DeadZone({DeadZone}), Axis={AxisLock})";
	}
}
