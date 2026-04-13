using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Rotate around <paramref name="axis"/> by <paramref name="degreesPerSecond"/> degrees per second. Chain <c>.Clamp(min, max)</c> and/or <c>.InWorldSpace()</c>. </summary>
		public TransformRotateByBuilder<TransformBuilderReady> RotateBy(VariableBlock degreesPerSecond, LunyAxis axis) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, axis, _trace.Add(nameof(RotateBy)));
	}

	public static class TransformRotateByBuilderExtensions
	{
		/// <summary> Apply rotation in world space instead of local space. </summary>
		public static TransformRotateByBuilder<TransformBuilderReady> InWorldSpace<T>(this TransformRotateByBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Space = LunyTransformSpace.World });

		/// <summary> Clamp the accumulated rotation angle between <paramref name="min"/> and <paramref name="max"/> degrees. </summary>
		public static TransformRotateByBuilder<TransformBuilderReady> Clamp<T>(this TransformRotateByBuilder<T> b, Double min, Double max)
			where T : struct, ITransformBuilderReady => new(b.Options with { MinAngle = min, MaxAngle = max });
	}

	public readonly struct TransformRotateByBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformRotateByOptions Options;

		internal static TransformRotateByBuilder<T> Create(Script script, VariableBlock amount, LunyAxis axis, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformRotateByBuilder<T>), "Transform." + nameof(TransformBuilder.RotateBy));
			var options = new TransformRotateByOptions
			{
				Script = script, Token = token, Amount = amount, Axis = axis,
				MinAngle = Double.NegativeInfinity, MaxAngle = Double.PositiveInfinity,
				Space = LunyTransformSpace.Local, Trace = trace,
			};
			return new TransformRotateByBuilder<T>(options);
		}

		internal TransformRotateByBuilder(in TransformRotateByOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformRotateByBuilder<T> b) => Finish(b.Options);

		private static TransformRotateByBlock Finish(in TransformRotateByOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformRotateByBlock.Create(options.Amount, options.Axis, options.Space, options.MinAngle, options.MaxAngle,
				options.Trace);
		}
	}

	internal record TransformRotateByOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyTransformSpace Space;
		public VariableBlock Amount;
		public LunyAxis Axis;
		public Double MinAngle;
		public Double MaxAngle;
	}
}
