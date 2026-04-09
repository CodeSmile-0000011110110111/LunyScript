using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly struct RigidbodyKinematicBuilder
	{
		private readonly Script _script;
		private readonly StackTrace _trace;

		internal RigidbodyKinematicBuilder(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary> Move the kinematic rigidbody by <paramref name="amount"/> along <paramref name="axis"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicMoveTerminalBuilder Move(VariableBlock amount, LunyAxis axis) =>
			RigidbodyKinematicMoveTerminalBuilder.CreateAxisRelative(_script, amount, axis, _trace.Add(nameof(Move)));

		/// <summary> Move the kinematic rigidbody by <paramref name="delta"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicMoveTerminalBuilder Move(LunyVector3 delta) =>
			RigidbodyKinematicMoveTerminalBuilder.CreateVector(_script, delta, _trace.Add(nameof(Move)));

		/// <summary> Rotate the kinematic rigidbody by <paramref name="amount"/> degrees around <paramref name="axis"/> per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicRotateTerminalBuilder Rotate(VariableBlock amount, LunyAxis axis) =>
			RigidbodyKinematicRotateTerminalBuilder.CreateAxisRelative(_script, amount, axis, _trace.Add(nameof(Rotate)));

		/// <summary> Rotate the kinematic rigidbody by <paramref name="eulerDelta"/> degrees per second. Append <c>.InWorldSpace()</c> for world space. </summary>
		[NeedsReview] [NeedsSmokeTest]
		public RigidbodyKinematicRotateTerminalBuilder Rotate(LunyVector3 eulerDelta) =>
			RigidbodyKinematicRotateTerminalBuilder.CreateVector(_script, eulerDelta, _trace.Add(nameof(Rotate)));
	}
}
