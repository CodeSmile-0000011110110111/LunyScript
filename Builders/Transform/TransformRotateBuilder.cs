using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct TransformRotateBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformBuilderOptions Options;

		internal static TransformRotateBuilder<T> Create(Script script, VariableBlock amount, LunyAxis axis, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformRotateBuilder<T>), "Transform.Rotate()");
			var options = new TransformBuilderOptions
			{
				Script = script, Token = token, Amount = amount, Axis = axis,
				MinAngle = Double.NegativeInfinity, MaxAngle = Double.PositiveInfinity,
				Space = LunyTransformSpace.Local, Trace = trace,
			};
			return new TransformRotateBuilder<T>(options);
		}

		internal TransformRotateBuilder(in TransformBuilderOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformRotateBuilder<T> b) => Finish(b.Options);

		/// <summary> Clamp the accumulated rotation angle between <paramref name="min"/> and <paramref name="max"/> degrees. </summary>
		public TransformRotateBuilder<TransformBuilderReady> Clamp(Double min, Double max) =>
			new(Options with { MinAngle = min, MaxAngle = max });

		/// <summary> Apply rotation in world space instead of local space. </summary>
		public TransformAddRotationAngleBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal TransformAddRotationAngleBlock Finish() => Finish(Options);

		private static TransformAddRotationAngleBlock Finish(in TransformBuilderOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformAddRotationAngleBlock.Create(options.Amount, options.Axis, options.Space, options.MinAngle, options.MaxAngle,
				options.Trace);
		}
	}
}
