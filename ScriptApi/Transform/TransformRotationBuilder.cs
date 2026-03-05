using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Fluent builder for Rotate-Towards blocks.
	/// Usage: Transform.RotateTowards(target).Speed(45).Responsiveness(2).LockY()
	///        Transform.RotateTowards(target).Speed(45).Lerp()
	///        Transform.RotateTowards(target).Speed(45).Slerp()
	/// </summary>
	public readonly struct TransformRotationBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformBuilderOptions Options;

		internal TransformRotationBuilder(in TransformBuilderOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ScriptActionBlock(TransformRotationBuilder<T> b) => Finish(b.Options);

		internal static ScriptActionBlock Finish(in TransformBuilderOptions options)
		{
			var block = TransformRotationLinearTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}

		internal static TransformRotationLerpTowardsObjectBlock FinishLerpBuilder(in TransformBuilderOptions options)
		{
			var block = TransformRotationLerpTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.SphericalLerp);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}
	}

	public static class TransformRotateBuilderExtensions
	{
		/// <summary> Rotation speed in degrees per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformRotationBuilder<TransformBuilderReady> Speed<T>(this TransformRotationBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum angle threshold in degrees before rotation begins (prevents micro-jitter). </summary>
		public static TransformRotationBuilder<TransformBuilderReady> DeadZone<T>(this TransformRotationBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> Responsiveness<T>(this TransformRotationBuilder<T> b,
			Double responsiveness)
			where T : struct, ITransformBuilderReady => new(b.Options with { Responsiveness = responsiveness });

		/// <summary> Prevents rotation around the X axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockX<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformRotationBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents rotation around the Y axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockY<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformRotationBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents rotation around the Z axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockZ<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformRotationBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotationLerpTowardsObjectBlock Lerp<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady => TransformRotationBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotationLerpTowardsObjectBlock Slerp<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotationBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true, SphericalLerp = true });
	}
}
