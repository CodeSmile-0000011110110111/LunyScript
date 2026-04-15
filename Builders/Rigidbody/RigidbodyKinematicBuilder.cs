using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
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

	internal record RigidbodyKinematicOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public VariableBlock Amount;
		public LunyAxis Axis;
		public Boolean UseVector;
		public LunyVector3 Vector;
		public LunyVector3 EulerDelta;
		public LunyTransformSpace Space;
	}
}
