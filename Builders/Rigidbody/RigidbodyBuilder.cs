using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct RigidbodyBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal RigidbodyBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary> Sets whether the rigidbody is kinematic (not affected by physics forces). </summary>
		[NeedsReview] [NeedsSmokeTest]
		public ActionBlock SetKinematic(Boolean enabled) => RigidbodySetKinematicBlock.Create(enabled, _trace.Add(nameof(SetKinematic)));

		/// <summary> Sets whether gravity affects this rigidbody. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public ActionBlock SetGravityEnabled(Boolean enabled) =>
			RigidbodySetGravityEnabledBlock.Create(enabled, _trace.Add(nameof(SetGravityEnabled)));

		/// <summary> Access kinematic movement and rotation operations. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicBuilder Kinematic => new(_script, _trace.Add(nameof(Kinematic)));

		/// <summary> Access dynamic force and impulse operations. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicBuilder Dynamic => new(_script, _trace.Add(nameof(Dynamic)));

		internal static LunyForceMode ToForceMode(Boolean isImpulse, Boolean ignoreMass)
		{
			if (!isImpulse && !ignoreMass)
				return LunyForceMode.Force; // Force (over time) with mass
			if (!isImpulse && ignoreMass)
				return LunyForceMode.Acceleration; // Force ignoring mass
			if (isImpulse && !ignoreMass)
				return LunyForceMode.Impulse; // Impulse (all at once) with mass

			return LunyForceMode.VelocityChange; // Impulse ignoring mass, ie a directly sets velocity
		}
	}
}
