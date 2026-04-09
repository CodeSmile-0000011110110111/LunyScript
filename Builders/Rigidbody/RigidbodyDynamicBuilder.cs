using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

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
		public RigidbodyDynamicForceTerminalBuilder AddForce(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicForceTerminalBuilder.CreateAxisRelative(_script, amount, axis, false, _trace.Add(nameof(AddForce)));

		/// <summary> Apply a continuous force vector. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicForceTerminalBuilder AddForce(LunyVector3 force) =>
			RigidbodyDynamicForceTerminalBuilder.CreateVector(_script, force, false, _trace.Add(nameof(AddForce)));

		/// <summary> Apply an instant impulse along <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicForceTerminalBuilder AddImpulse(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicForceTerminalBuilder.CreateAxisRelative(_script, amount, axis, true, _trace.Add(nameof(AddImpulse)));

		/// <summary> Apply an instant impulse vector. Chain <c>.IgnoreMass()</c>, <c>.AtPosition()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicForceTerminalBuilder AddImpulse(LunyVector3 impulse) =>
			RigidbodyDynamicForceTerminalBuilder.CreateVector(_script, impulse, true, _trace.Add(nameof(AddImpulse)));

		/// <summary> Apply a continuous torque around <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicAngularForceTerminalBuilder AddAngularForce(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicAngularForceTerminalBuilder.CreateAxisRelative(_script, amount, axis, false, _trace.Add(nameof(AddAngularForce)));

		/// <summary> Apply a continuous torque vector. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicAngularForceTerminalBuilder AddAngularForce(LunyVector3 torque) =>
			RigidbodyDynamicAngularForceTerminalBuilder.CreateVector(_script, torque, false, _trace.Add(nameof(AddAngularForce)));

		/// <summary> Apply an instant angular impulse around <paramref name="axis"/>. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicAngularForceTerminalBuilder AddAngularImpulse(VariableBlock amount, LunyAxis axis) =>
			RigidbodyDynamicAngularForceTerminalBuilder.CreateAxisRelative(_script, amount, axis, true, _trace.Add(nameof(AddAngularImpulse)));

		/// <summary> Apply an instant angular impulse vector. Chain <c>.IgnoreMass()</c>, <c>.InWorldSpace()</c>. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyDynamicAngularForceTerminalBuilder AddAngularImpulse(LunyVector3 torque) =>
			RigidbodyDynamicAngularForceTerminalBuilder.CreateVector(_script, torque, true, _trace.Add(nameof(AddAngularImpulse)));
	}
}
