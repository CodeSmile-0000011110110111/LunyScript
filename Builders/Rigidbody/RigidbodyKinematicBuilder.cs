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
		public RigidbodyKinematicMoveByBuilder MoveBy(VariableBlock amount, LunyAxis axis) =>
			RigidbodyKinematicMoveByBuilder.CreateAxisRelative(_script, amount, axis, _trace.Add(nameof(MoveBy)));

		/// <summary> Move the kinematic rigidbody by <paramref name="delta"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		public RigidbodyKinematicMoveByBuilder MoveBy(LunyVector3 delta) =>
			RigidbodyKinematicMoveByBuilder.CreateVector(_script, delta, _trace.Add(nameof(MoveBy)));

		/// <summary> Rotate the kinematic rigidbody by <paramref name="amount"/> degrees around <paramref name="axis"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicRotateByBuilder RotateBy(VariableBlock amount, LunyAxis axis) =>
			RigidbodyKinematicRotateByBuilder.CreateAxisRelative(_script, amount, axis, _trace.Add(nameof(RotateBy)));

		/// <summary> Rotate the kinematic rigidbody by <paramref name="eulerDelta"/> degrees per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicRotateByBuilder RotateBy(LunyVector3 eulerDelta) =>
			RigidbodyKinematicRotateByBuilder.CreateVector(_script, eulerDelta, _trace.Add(nameof(RotateBy)));
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
