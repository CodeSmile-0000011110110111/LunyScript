using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Scale uniformly by <paramref name="amount"/> units per second. Chain <c>.BoxClamp()</c> and/or <c>.SphereClamp()</c>. </summary>
		public TransformScaleByBuilder ScaleBy(VariableBlock amount) =>
			TransformScaleByBuilder.CreateUniform(_script, amount, _trace.Add(nameof(ScaleBy)));

		/// <summary> Scale per-axis by <paramref name="scalePerSecond"/> units per second. Chain <c>.BoxClamp()</c> and/or <c>.SphereClamp()</c>. </summary>
		public TransformScaleByBuilder ScaleBy(VariableBlock<LunyVector3> scalePerSecond) =>
			TransformScaleByBuilder.CreateVector3(_script, scalePerSecond, _trace.Add(nameof(ScaleBy)));
	}

	public static class TransformScaleByBuilderExtensions
	{
		/// <summary> Set speed multiplier for scaling. </summary>
		public static TransformScaleByBuilder Speed(this TransformScaleByBuilder b, VariableBlock speed) =>
			new(b.Options with { Speed = speed });

		/// <summary> Clamp the accumulated scale within a box defined by <paramref name="min"/> and <paramref name="max"/>. </summary>
		[NeedsSmokeTest]
		public static TransformScaleByBuilder Clamp(this TransformScaleByBuilder b, LunyVector3 min, LunyVector3 max) =>
			new(b.Options with { UseBoxClamp = true, BoxMin = min, BoxMax = max });

		// Spherical clamping scale is not very meaningful ... fringe use case.
		/*
		/// <summary> Clamp the accumulated scale within a sphere of <paramref name="radius"/>. </summary>
		[NeedsSmokeTest]
		public static TransformScaleByBuilder Clamp(this TransformScaleByBuilder b, Double radius) =>
			new(b.Options with { UseSphereClamp = true, SphereRadius = (Single)radius });
	*/
	}

	public readonly struct TransformScaleByBuilder
	{
		internal readonly TransformScaleByOptions Options;

		internal static TransformScaleByBuilder CreateUniform(Script script, VariableBlock amount, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformScaleByBuilder), "Transform." + nameof(TransformBuilder.ScaleBy));
			var options = new TransformScaleByOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, UseVector3 = false,
			};
			return new TransformScaleByBuilder(options);
		}

		internal static TransformScaleByBuilder CreateVector3(Script script, VariableBlock<LunyVector3> scalePerSecond, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformScaleByBuilder), "Transform." + nameof(TransformBuilder.ScaleBy) + "(Vec3)");
			var options = new TransformScaleByOptions
			{
				Script = script, Token = token, Trace = trace,
				ScalePerSecond = scalePerSecond, UseVector3 = true,
			};
			return new TransformScaleByBuilder(options);
		}

		internal TransformScaleByBuilder(in TransformScaleByOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformScaleByBuilder b) => Finish(b.Options);

		private static TransformScaleByBlock Finish(in TransformScaleByOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			if (options.UseVector3)
				return TransformScaleByBlock.CreateVector3(options.ScalePerSecond, options.Speed,
					options.UseBoxClamp, options.BoxMin, options.BoxMax,
					options.UseSphereClamp, options.SphereRadius, options.Trace);
			return TransformScaleByBlock.CreateUniform(options.Amount, options.Speed,
				options.UseBoxClamp, options.BoxMin, options.BoxMax,
				options.UseSphereClamp, options.SphereRadius, options.Trace);
		}
	}

	internal record TransformScaleByOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public VariableBlock Amount;
		public VariableBlock<LunyVector3> ScalePerSecond;
		public VariableBlock Speed;
		public Boolean UseVector3;
		public Boolean UseBoxClamp;
		public Boolean UseSphereClamp;
		public LunyVector3 BoxMin;
		public LunyVector3 BoxMax;
		public Single SphereRadius;
	}
}
