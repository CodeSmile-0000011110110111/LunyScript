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
		public StackTrace Trace;
		public LunyTransformSpace Space;
		public VariableBlock<LunyVector2> Direction;
		public VariableBlock Amount;
		public LunyVector3 Axis;
		public VariableBlock Speed;
		public Boolean UseDirection;
	}

	public readonly struct TransformMoveTerminalBuilder
	{
		internal readonly TransformMoveOptions Options;

		internal static TransformMoveTerminalBuilder CreateDirectional(Script script, VariableBlock<LunyVector2> direction,
			VariableBlock speed, LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveTerminalBuilder), "Transform.Move(direction)");
			var options = new TransformMoveOptions
			{
				Script = script, Token = token, Trace = trace, Space = space,
				Direction = direction, Speed = speed, UseDirection = true,
			};
			return new TransformMoveTerminalBuilder(options);
		}

		internal static TransformMoveTerminalBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyVector3 axis,
			VariableBlock speed, LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveTerminalBuilder), "Transform.Move(axis)");
			var options = new TransformMoveOptions
			{
				Script = script, Token = token, Trace = trace, Space = space,
				Amount = amount, Axis = axis, Speed = speed, UseDirection = false,
			};
			return new TransformMoveTerminalBuilder(options);
		}

		internal TransformMoveTerminalBuilder(in TransformMoveOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		/// <summary> Apply movement in world space instead of local space. </summary>
		public TransformPositionMoveBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal TransformPositionMoveBlock Finish() => Finish(Options);

		private static TransformPositionMoveBlock Finish(in TransformMoveOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseDirection
				? TransformPositionMoveBlock.CreateDirectional(options.Direction, options.Speed, options.Space, options.Trace)
				: TransformPositionMoveBlock.CreateAxisRelative(options.Amount, options.Axis, options.Speed, options.Space, options.Trace);
		}
	}
}
