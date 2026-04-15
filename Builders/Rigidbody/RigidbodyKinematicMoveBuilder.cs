using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public static class RigidbodyKinematicMoveBuilderExtensions
	{
		/// <summary> Apply movement in world space instead of local space. </summary>
		public static RigidbodyKinematicMoveBuilder InWorldSpace(this RigidbodyKinematicMoveBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });
	}

	public readonly struct RigidbodyKinematicMoveBuilder
	{
		internal readonly RigidbodyKinematicOptions Options;

		internal static RigidbodyKinematicMoveBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicMoveBuilder), "Rigidbody.Kinematic.Move(axis)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicMoveBuilder(options);
		}

		internal static RigidbodyKinematicMoveBuilder CreateVector(Script script, LunyVector3 delta, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicMoveBuilder), "Rigidbody.Kinematic.Move(vector)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = delta, UseVector = true, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicMoveBuilder(options);
		}

		internal RigidbodyKinematicMoveBuilder(in RigidbodyKinematicOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyKinematicMoveBuilder b) => Finish(b.Options);

		private static ActionBlock Finish(in RigidbodyKinematicOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseVector
				? RigidbodyKinematicMoveBlock.CreateVector(options.Vector, options.Space, options.Trace)
				: RigidbodyKinematicMoveBlock.CreateAxisRelative(options.Amount, options.Axis, options.Space, options.Trace);
		}
	}
}
