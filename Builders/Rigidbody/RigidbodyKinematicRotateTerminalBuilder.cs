using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly struct RigidbodyKinematicRotateTerminalBuilder
	{
		internal readonly RigidbodyKinematicOptions Options;

		internal static RigidbodyKinematicRotateTerminalBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicRotateTerminalBuilder), "Rigidbody.Kinematic.Rotate(axis)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicRotateTerminalBuilder(options);
		}

		internal static RigidbodyKinematicRotateTerminalBuilder CreateVector(Script script, LunyVector3 eulerDelta, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicRotateTerminalBuilder), "Rigidbody.Kinematic.Rotate(vector)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				EulerDelta = eulerDelta, UseVector = true, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicRotateTerminalBuilder(options);
		}

		internal RigidbodyKinematicRotateTerminalBuilder(in RigidbodyKinematicOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyKinematicRotateTerminalBuilder b) => Finish(b.Options);

		/// <summary> Apply rotation in world space instead of local space. </summary>
		public ActionBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal ActionBlock Finish() => Finish(Options);

		private static ActionBlock Finish(in RigidbodyKinematicOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseVector
				? RigidbodyKinematicRotateBlock.CreateVector(options.EulerDelta, options.Space, options.Trace)
				: RigidbodyKinematicRotateBlock.CreateAxisRelative(options.Amount, options.Axis, options.Space, options.Trace);
		}
	}
}
