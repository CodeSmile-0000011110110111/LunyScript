using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that interpolate or step a transform property toward a target over time.
	/// Holds the shared optional parameters: speed, deadZone, lockAxis, responsiveness.
	/// </summary>
	public abstract class TransformTowardsBlock : ActionBlock
	{
		private readonly VariableBlock Speed;

		protected readonly VariableBlock DeadZone;
		protected readonly LunyVector3 LockAxis;

		protected TransformTowardsBlock(VariableBlock speed, VariableBlock deadZone, LunyVector3 lockAxis, LunyStackTrace trace)
			: base(trace)
		{
			Speed = speed;
			DeadZone = deadZone;
			LockAxis = lockAxis;
		}

		/// <summary>Returns Speed * deltaTime * Responsiveness, ready to use as a step or lerp t value.</summary>
		protected Double ComputeStep() => Speed.Value * LunyTime.DeltaTime;

		protected String ParametersToString() => $"Speed{Speed}, DeadZone{DeadZone}, Axis={LockAxis})";
	}
}
