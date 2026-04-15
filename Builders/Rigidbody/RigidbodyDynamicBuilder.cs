using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
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
		public RigidbodyDynamicForceBuilder AddForce(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicForceBuilder.CreateLocalForce(_script, amount, axis, false, _trace.Add(nameof(AddForce)));

		/// <summary> Apply an instant impulse along <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicForceBuilder AddImpulse(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicForceBuilder.CreateLocalForce(_script, amount, axis, true, _trace.Add(nameof(AddImpulse)));

		/// <summary> Apply a continuous torque around <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicTorqueBuilder AddAngularForce(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicTorqueBuilder.CreateLocalForce(_script, amount, axis, false, _trace.Add(nameof(AddAngularForce)));

		/// <summary> Apply an instant angular impulse around <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicTorqueBuilder AddAngularImpulse(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicTorqueBuilder.CreateLocalForce(_script, amount, axis, true, _trace.Add(nameof(AddAngularImpulse)));

		/// <summary> Apply a continuous force vector. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicForceBuilder AddForce(LunyVector3 force) =>
			RigidbodyDynamicForceBuilder.CreateWorldForce(_script, force, false, _trace.Add(nameof(AddForce)));

		/// <summary> Apply an instant impulse vector. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicForceBuilder AddImpulse(LunyVector3 impulse) =>
			RigidbodyDynamicForceBuilder.CreateWorldForce(_script, impulse, true, _trace.Add(nameof(AddImpulse)));

		/// <summary> Apply a continuous torque vector. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicTorqueBuilder AddAngularForce(LunyVector3 torque) =>
			RigidbodyDynamicTorqueBuilder.CreateWorldForce(_script, torque, false, _trace.Add(nameof(AddAngularForce)));

		/// <summary> Apply an instant angular impulse vector. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicTorqueBuilder AddAngularImpulse(LunyVector3 torque) =>
			RigidbodyDynamicTorqueBuilder.CreateWorldForce(_script, torque, true, _trace.Add(nameof(AddAngularImpulse)));
	}

	internal record RigidbodyAddForceOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public VariableBlock Amount;
		public LunyAxis Axis;
		public LunyVector3 Vector;
		public Boolean UseVector;
		public Boolean IsImpulse;
		public Boolean IgnoreMass;
		public Boolean HasAtPositionOffset;
		public LunyVector3 AtPositionOffset;
		public LunyObjectRef AtPositionChildRef;
		public LunyTransformSpace Space;
	}
}
