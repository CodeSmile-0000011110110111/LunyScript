using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionBuilder SetPosition(Double x, Double y, Double z) => TransformSetPositionBuilder.Create(
			_script, new LunyVector3(x, y, z), LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));

		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionBuilder SetPosition(VariableBlock<LunyVector3> position) =>
			TransformSetPositionBuilder.Create(_script, position, LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));
	}

	public readonly struct TransformSetPositionBuilder
	{
		public static implicit operator ActionBlock(TransformSetPositionBuilder b) => Finish(b.Options);

		internal readonly TransformSetPositionOptions Options;

		internal static TransformSetPositionBuilder Create(Script script, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetPositionBuilder), "Transform.SetPosition()");
			var options = new TransformSetPositionOptions
			{
				Script = script, Token = token, Trace = trace, Position = position, Space = space,
			};
			return new TransformSetPositionBuilder(options);
		}

		internal TransformSetPositionBuilder(in TransformSetPositionOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		/// <summary> Override only the X component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetSingleAxisBlock X(Double value) => FinishAxis(LunyAxis.X, value);

		/// <summary> Override only the Y component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetSingleAxisBlock Y(Double value) => FinishAxis(LunyAxis.Y, value);

		/// <summary> Override only the Z component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetSingleAxisBlock Z(Double value) => FinishAxis(LunyAxis.Z, value);

		/// <summary> Apply position set in world space instead of local space. </summary>
		public TransformSetPositionBuilder InWorldSpace() => new(Options with { Space = LunyTransformSpace.World });

		internal TransformPositionSetBlock Finish() => Finish(Options);

		private TransformPositionSetSingleAxisBlock FinishAxis(LunyAxis axis, Double value)
		{
			Options.Script.MarkBuilderTokenFinished(Options.Token);
			return TransformPositionSetSingleAxisBlock.Create(axis, value, Options.Space, Options.Trace);
		}

		private static TransformPositionSetBlock Finish(in TransformSetPositionOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformPositionSetBlock.Create(options.Position, options.Space, options.Trace);
		}
	}

	internal record TransformSetPositionOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;
		public VariableBlock<LunyVector3> Position;
		public LunyTransformSpace Space;
	}
}
