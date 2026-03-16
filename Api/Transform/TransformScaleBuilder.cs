using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Fluent builder for Scale-Towards blocks.
	/// Usage: Transform.ScaleTowards(targetScale).Speed(1).Responsiveness(2).LockY()
	///        Transform.ScaleTowards(targetScale).Speed(1).Lerp()
	///        Transform.ScaleTowards(targetScale).Speed(1).Slerp()
	/// </summary>
	public readonly struct TransformScaleBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformBuilderOptions Options;

		internal TransformScaleBuilder(in TransformBuilderOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformScaleBuilder<T> b) => Finish(b.Options);

		internal static ActionBlock Finish(in TransformBuilderOptions options)
		{
			var block = TransformScaleTowardsBlock.Create(options.TargetScale, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}

		internal static TransformScaleTowardsLerpBlock FinishLerpBuilder(in TransformBuilderOptions options)
		{
			var block = TransformScaleTowardsLerpBlock.Create(options.TargetScale, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.SphericalLerp);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}
	}

	public static class TransformScaleBuilderExtensions
	{
		/// <summary> Scale speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformScaleBuilder<TransformBuilderReady> Speed<T>(this TransformScaleBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum scale-distance threshold before scaling begins (prevents micro-jitter). </summary>
		public static TransformScaleBuilder<TransformBuilderReady> DeadZone<T>(this TransformScaleBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> Responsiveness<T>(this TransformScaleBuilder<T> b, Double responsiveness)
			where T : struct, ITransformBuilderReady => new(b.Options with { Responsiveness = responsiveness });

		/// <summary> Prevents scaling along the X axis. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> LockX<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformScaleBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents scaling along the Y axis. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> LockY<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformScaleBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents scaling along the Z axis. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> LockZ<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformScaleBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformScaleTowardsLerpBlock Lerp<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady => TransformScaleBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformScaleTowardsLerpBlock Slerp<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformScaleBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true, SphericalLerp = true });
	}
}
