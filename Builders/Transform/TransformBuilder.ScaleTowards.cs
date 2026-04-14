using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Scale toward the target scale each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformScaleTowardsBuilder<TransformBuilderReady> ScaleTowards(VariableBlock<LunyVector3> targetScale)
		{
			var token = _script.CreateBuilderToken(nameof(ScaleTowards), "Transform." + nameof(ScaleTowards));
			var options = new TransformScaleTowardsOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(ScaleTowards)),
				Scale = targetScale,
				Speed = 1.0,
				DeadZone = 0.1,
				LockAxis = LunyVector3.One,
			};
			return new TransformScaleTowardsBuilder<TransformBuilderReady>(options);
		}
	}

	public static class TransformScaleBuilderExtensions
	{
		/// <summary> Scale speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> Speed<T>(this TransformScaleTowardsBuilder<T> b, VariableBlock speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum scale-distance threshold before scaling begins (prevents micro-jitter). </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> DeadZone<T>(this TransformScaleTowardsBuilder<T> b,
			VariableBlock deadZone) where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> Lerp<T>(this TransformScaleTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Linear });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> Slerp<T>(this TransformScaleTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Spherical });

		/// <summary> Prevents scaling along the X axis. </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> LockX<T>(this TransformScaleTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformScaleTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents scaling along the Y axis. </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> LockY<T>(this TransformScaleTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformScaleTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents scaling along the Z axis. </summary>
		public static TransformScaleTowardsBuilder<TransformBuilderReady> LockZ<T>(this TransformScaleTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformScaleTowardsBuilder<TransformBuilderReady>(options);
		}
	}

	/// <summary>
	/// Fluent builder for Scale-Towards blocks.
	/// Usage: Transform.ScaleTowards(targetScale).Speed(1).Responsiveness(2).LockY()
	///        Transform.ScaleTowards(targetScale).Speed(1).Lerp()
	///        Transform.ScaleTowards(targetScale).Speed(1).Slerp()
	/// </summary>
	public readonly struct TransformScaleTowardsBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformScaleTowardsOptions Options;

		internal TransformScaleTowardsBuilder(in TransformScaleTowardsOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformScaleTowardsBuilder<T> b) => Finish(b.Options);

		private static ActionBlock Finish(in TransformScaleTowardsOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformScaleTowardsVariableBlock.Create(options.Scale, options.Speed, options.DeadZone, options.LockAxis,
				options.Interpolation, options.Trace);
		}
	}

	internal record TransformScaleTowardsOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public VariableBlock<LunyVector3> Scale;
		public VariableBlock Speed;
		public VariableBlock DeadZone;
		public LunyVector3 LockAxis;
		public LunyInterpolation Interpolation;

		public void LockAxisX() => LockAxis = VectorUtil.ClearX(LockAxis);
		public void LockAxisY() => LockAxis = VectorUtil.ClearY(LockAxis);
		public void LockAxisZ() => LockAxis = VectorUtil.ClearZ(LockAxis);
	}
}
