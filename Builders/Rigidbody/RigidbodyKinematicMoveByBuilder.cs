using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public static class RigidbodyKinematicMoveByBuilderExtensions
	{
		/// <summary> Apply movement in world space instead of local space. </summary>
		public static RigidbodyKinematicMoveByBuilder InWorldSpace(this RigidbodyKinematicMoveByBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });
	}

	public readonly struct RigidbodyKinematicMoveByBuilder
	{
		internal readonly RigidbodyKinematicOptions Options;

		internal static RigidbodyKinematicMoveByBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicMoveByBuilder), "Rigidbody.Kinematic.Move(axis)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicMoveByBuilder(options);
		}

		internal static RigidbodyKinematicMoveByBuilder CreateVector(Script script, LunyVector3 delta, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicMoveByBuilder), "Rigidbody.Kinematic.Move(vector)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = delta, UseVector = true, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicMoveByBuilder(options);
		}

		internal RigidbodyKinematicMoveByBuilder(in RigidbodyKinematicOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyKinematicMoveByBuilder b) => Finish(b.Options);

		private static ActionBlock Finish(in RigidbodyKinematicOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseVector
				? RigidbodyKinematicMoveByBlock.CreateVector(options.Vector, options.Space, options.Trace)
				: RigidbodyKinematicMoveByBlock.CreateAxisRelative(options.Amount, options.Axis, options.Space, options.Trace);
		}
	}
}
