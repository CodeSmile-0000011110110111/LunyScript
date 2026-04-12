using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record TransformMoveOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;
		public LunyTransformSpace Space;
		public VariableBlock<LunyVector2> Direction;
		public VariableBlock Amount;
		public LunyVector3 Axis;
		public VariableBlock Speed;
		public Boolean UseDirection;
	}

	public readonly struct TransformMoveBuilder
	{
		public static implicit operator ActionBlock(TransformMoveBuilder b) => Finish(b.Options);

		internal readonly TransformMoveOptions Options;

		internal static TransformMoveBuilder CreateDirectional(Script script, VariableBlock<LunyVector2> direction,
			VariableBlock speed, LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveBuilder), "Transform.Move(direction)");
			var options = new TransformMoveOptions
			{
				Script = script, Token = token, Trace = trace, Space = space,
				Direction = direction, Speed = speed, UseDirection = true,
			};
			return new TransformMoveBuilder(options);
		}

		internal static TransformMoveBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyVector3 axis,
			VariableBlock speed, LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveBuilder), "Transform.Move(axis)");
			var options = new TransformMoveOptions
			{
				Script = script, Token = token, Trace = trace, Space = space,
				Amount = amount, Axis = axis, Speed = speed, UseDirection = false,
			};
			return new TransformMoveBuilder(options);
		}

		internal TransformMoveBuilder(in TransformMoveOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		/// <summary> Apply movement in world space instead of local space. </summary>
		public TransformMoveBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal TransformMoveBlock Finish() => Finish(Options);

		private static TransformMoveBlock Finish(in TransformMoveOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseDirection
				? TransformMoveBlock.CreatePlaneMove(options.Direction, options.Speed, options.Space, options.Trace)
				: TransformMoveBlock.CreateAxisMove(options.Amount, options.Axis, options.Speed, options.Space, options.Trace);
		}
	}
}
