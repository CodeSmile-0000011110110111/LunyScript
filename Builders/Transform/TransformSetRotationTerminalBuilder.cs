using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record TransformSetRotationOptions
	{
		public Script Script;
		public BuilderToken Token;
		public StackTrace Trace;
		public VariableBlock<LunyQuaternion> Rotation;
		public LunyTransformSpace Space;
	}

	public readonly struct TransformSetRotationTerminalBuilder
	{
		internal readonly TransformSetRotationOptions Options;

		internal static TransformSetRotationTerminalBuilder Create(Script script, VariableBlock<LunyQuaternion> rotation,
			LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetRotationTerminalBuilder), "Transform.SetRotation()");
			var options = new TransformSetRotationOptions
			{
				Script = script, Token = token, Trace = trace, Rotation = rotation, Space = space,
			};
			return new TransformSetRotationTerminalBuilder(options);
		}

		internal TransformSetRotationTerminalBuilder(in TransformSetRotationOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		/// <summary> Override only the X euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAxisBlock X(Double value) => FinishAxis(LunyAxis.X, value);

		/// <summary> Override only the Y euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAxisBlock Y(Double value) => FinishAxis(LunyAxis.Y, value);

		/// <summary> Override only the Z euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAxisBlock Z(Double value) => FinishAxis(LunyAxis.Z, value);

		/// <summary> Apply rotation set in world space instead of local space. </summary>
		public TransformSetRotationTerminalBuilder InWorldSpace() => new(Options with { Space = LunyTransformSpace.World });

		internal TransformRotationSetBlock Finish() => Finish(Options);

		private TransformRotationSetAxisBlock FinishAxis(LunyAxis axis, Double value)
		{
			Options.Script.MarkBuilderTokenFinished(Options.Token);
			return TransformRotationSetAxisBlock.Create(axis, value, Options.Space, Options.Trace);
		}

		private static TransformRotationSetBlock Finish(in TransformSetRotationOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformRotationSetBlock.Create(options.Rotation, options.Space, options.Trace);
		}
	}
}
