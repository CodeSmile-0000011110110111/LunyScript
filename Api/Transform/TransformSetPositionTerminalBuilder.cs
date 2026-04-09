using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record TransformSetPositionOptions
	{
		public Script Script;
		public BuilderToken Token;
		public StackTrace Trace;
		public VariableBlock<LunyVector3> Position;
		public LunyTransformSpace Space;
	}

	public readonly struct TransformSetPositionTerminalBuilder
	{
		internal readonly TransformSetPositionOptions Options;

		internal static TransformSetPositionTerminalBuilder Create(Script script, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetPositionTerminalBuilder), "Transform.SetPosition()");
			var options = new TransformSetPositionOptions
			{
				Script = script, Token = token, Trace = trace, Position = position, Space = space,
			};
			return new TransformSetPositionTerminalBuilder(options);
		}

		internal TransformSetPositionTerminalBuilder(in TransformSetPositionOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		/// <summary> Override only the X component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetAxisBlock X(Double value) => FinishAxis(LunyAxis.X, value);

		/// <summary> Override only the Y component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetAxisBlock Y(Double value) => FinishAxis(LunyAxis.Y, value);

		/// <summary> Override only the Z component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetAxisBlock Z(Double value) => FinishAxis(LunyAxis.Z, value);

		/// <summary> Apply position set in world space instead of local space. </summary>
		public TransformSetPositionTerminalBuilder InWorldSpace() => new(Options with { Space = LunyTransformSpace.World });

		internal TransformPositionSetBlock Finish() => Finish(Options);

		private TransformPositionSetAxisBlock FinishAxis(LunyAxis axis, Double value)
		{
			Options.Script.MarkBuilderTokenFinished(Options.Token);
			return TransformPositionSetAxisBlock.Create(axis, value, Options.Space, Options.Trace);
		}

		private static TransformPositionSetBlock Finish(in TransformSetPositionOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformPositionSetBlock.Create(options.Position, options.Space, options.Trace);
		}
	}
}
