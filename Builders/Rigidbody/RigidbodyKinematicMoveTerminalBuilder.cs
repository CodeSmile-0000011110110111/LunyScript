using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record RigidbodyKinematicOptions
	{
		public Script Script;
		public BuilderToken Token;
		public StackTrace Trace;
		public VariableBlock Amount;
		public LunyAxis Axis;
		public LunyVector3 Vector;
		public LunyVector3 EulerDelta;
		public Boolean UseVector;
		public LunyTransformSpace Space;
	}

	public readonly struct RigidbodyKinematicMoveTerminalBuilder
	{
		internal readonly RigidbodyKinematicOptions Options;

		internal static RigidbodyKinematicMoveTerminalBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicMoveTerminalBuilder), "Rigidbody.Kinematic.Move(axis)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicMoveTerminalBuilder(options);
		}

		internal static RigidbodyKinematicMoveTerminalBuilder CreateVector(Script script, LunyVector3 delta, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyKinematicMoveTerminalBuilder), "Rigidbody.Kinematic.Move(vector)");
			var options = new RigidbodyKinematicOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = delta, UseVector = true, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyKinematicMoveTerminalBuilder(options);
		}

		internal RigidbodyKinematicMoveTerminalBuilder(in RigidbodyKinematicOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyKinematicMoveTerminalBuilder b) => Finish(b.Options);

		/// <summary> Apply movement in world space instead of local space. </summary>
		public ActionBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal ActionBlock Finish() => Finish(Options);

		private static ActionBlock Finish(in RigidbodyKinematicOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseVector
				? RigidbodyKinematicMoveBlock.CreateVector(options.Vector, options.Space, options.Trace)
				: RigidbodyKinematicMoveBlock.CreateAxisRelative(options.Amount, options.Axis, options.Space, options.Trace);
		}
	}
}
