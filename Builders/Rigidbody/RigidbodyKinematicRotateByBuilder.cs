using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public static class RigidbodyKinematicRotateByBuilderExtensions
	{
		/// <summary> Apply rotation in world space instead of local space. </summary>
		public static RigidbodyKinematicRotateByBuilder InWorldSpace(this RigidbodyKinematicRotateByBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });
	}

	public readonly struct RigidbodyKinematicRotateByBuilder
	{
		internal readonly RigidbodyKinematicOptions Options;

		internal static RigidbodyKinematicRotateByBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicRotateByBuilder), "Rigidbody.Kinematic.Rotate(axis)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicRotateByBuilder(options);
		}

		internal static RigidbodyKinematicRotateByBuilder CreateVector(Script script, LunyVector3 eulerDelta, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicRotateByBuilder), "Rigidbody.Kinematic.Rotate(vector)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				EulerDelta = eulerDelta, UseVector = true, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicRotateByBuilder(options);
		}

		internal RigidbodyKinematicRotateByBuilder(in RigidbodyKinematicOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyKinematicRotateByBuilder b) => Finish(b.Options);

		private static ActionBlock Finish(in RigidbodyKinematicOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseVector
				? RigidbodyKinematicRotateBlock.CreateVector(options.EulerDelta, options.Space, options.Trace)
				: RigidbodyKinematicRotateBlock.CreateAxisRelative(options.Amount, options.Axis, options.Space, options.Trace);
		}
	}
}
