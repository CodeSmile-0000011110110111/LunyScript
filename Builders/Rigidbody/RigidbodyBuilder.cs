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
	}

	public readonly struct RigidbodyKinematicBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal RigidbodyKinematicBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary> Move the kinematic rigidbody by <paramref name="amount"/> along <paramref name="axis"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicMoveBuilder Move(VariableBlock amount, LunyAxis axis) =>
			RigidbodyKinematicMoveBuilder.CreateAxisRelative(_script, amount, axis, _trace.Add(nameof(Move)));

		/// <summary> Move the kinematic rigidbody by <paramref name="delta"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicMoveBuilder Move(LunyVector3 delta) =>
			RigidbodyKinematicMoveBuilder.CreateVector(_script, delta, _trace.Add(nameof(Move)));

		/// <summary> Rotate the kinematic rigidbody by <paramref name="amount"/> degrees around <paramref name="axis"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicRotateBuilder Rotate(VariableBlock amount, LunyAxis axis) =>
			RigidbodyKinematicRotateBuilder.CreateAxisRelative(_script, amount, axis, _trace.Add(nameof(Rotate)));

		/// <summary> Rotate the kinematic rigidbody by <paramref name="eulerDelta"/> degrees per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicRotateBuilder Rotate(LunyVector3 eulerDelta) =>
			RigidbodyKinematicRotateBuilder.CreateVector(_script, eulerDelta, _trace.Add(nameof(Rotate)));
	}

	public readonly struct RigidbodyDynamicBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal RigidbodyDynamicBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary> Apply a continuous force along <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddLinearForceBuilder AddForce(VariableBlock amount, LunyAxis axis) =>
			RigidbodyAddLinearForceBuilder.CreateLocalForce(_script, amount, axis, false, _trace.Add(nameof(AddForce)));

		/// <summary> Apply an instant impulse along <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddLinearForceBuilder AddImpulse(VariableBlock amount, LunyAxis axis) =>
			RigidbodyAddLinearForceBuilder.CreateLocalForce(_script, amount, axis, true, _trace.Add(nameof(AddImpulse)));

		/// <summary> Apply a continuous torque around <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddAngularForceBuilder AddAngularForce(VariableBlock amount, LunyAxis axis) =>
			RigidbodyAddAngularForceBuilder.CreateLocalForce(_script, amount, axis, false, _trace.Add(nameof(AddAngularForce)));

		/// <summary> Apply an instant angular impulse around <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddAngularForceBuilder AddAngularImpulse(VariableBlock amount, LunyAxis axis) =>
			RigidbodyAddAngularForceBuilder.CreateLocalForce(_script, amount, axis, true, _trace.Add(nameof(AddAngularImpulse)));

		/// <summary> Apply a continuous force vector. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddLinearForceBuilder AddForce(LunyVector3 force) =>
			RigidbodyAddLinearForceBuilder.CreateWorldForce(_script, force, false, _trace.Add(nameof(AddForce)));

		/// <summary> Apply an instant impulse vector. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddLinearForceBuilder AddImpulse(LunyVector3 impulse) =>
			RigidbodyAddLinearForceBuilder.CreateWorldForce(_script, impulse, true, _trace.Add(nameof(AddImpulse)));

		/// <summary> Apply a continuous torque vector. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddAngularForceBuilder AddAngularForce(LunyVector3 torque) =>
			RigidbodyAddAngularForceBuilder.CreateWorldForce(_script, torque, false, _trace.Add(nameof(AddAngularForce)));

		/// <summary> Apply an instant angular impulse vector. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyAddAngularForceBuilder AddAngularImpulse(LunyVector3 torque) =>
			RigidbodyAddAngularForceBuilder.CreateWorldForce(_script, torque, true, _trace.Add(nameof(AddAngularImpulse)));
	}
}
