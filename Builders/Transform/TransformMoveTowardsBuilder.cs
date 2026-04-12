using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Fluent builder for Move-Towards blocks.
	/// Usage: Transform.MoveTowards(target).Speed(3).Responsiveness(2).LockY()
	///        Transform.MoveTowards(target).Speed(3).Lerp()
	///        Transform.MoveTowards(target).Speed(3).Slerp()
	/// </summary>
	public readonly struct TransformMoveTowardsBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformBuilderOptions Options;

		internal TransformMoveTowardsBuilder(in TransformBuilderOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformMoveTowardsBuilder<T> b) => Finish(b.Options);

		internal static ActionBlock Finish(in TransformBuilderOptions options)
		{
			var block = TransformMoveTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.Trace);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}

		internal static TransformMoveTowardsObjectLerpBlock FinishLerpBuilder(in TransformBuilderOptions options)
		{
			var block = TransformMoveTowardsObjectLerpBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.SphericalLerp, options.Trace);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}
	}

	public static class TransformMoveBuilderExtensions
	{
		/// <summary> Movement speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> Speed<T>(this TransformMoveTowardsBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum distance threshold before movement begins (prevents micro-jitter). </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> DeadZone<T>(this TransformMoveTowardsBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> Responsiveness<T>(this TransformMoveTowardsBuilder<T> b,
			Double responsiveness)
			where T : struct, ITransformBuilderReady => new(b.Options with { Responsiveness = responsiveness });

		/// <summary> Prevents movement along the X axis. </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> LockX<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformMoveTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents movement along the Y axis. </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> LockY<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformMoveTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents movement along the Z axis. </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> LockZ<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformMoveTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformMoveTowardsObjectLerpBlock Lerp<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => TransformMoveTowardsBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformMoveTowardsObjectLerpBlock Slerp<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformMoveTowardsBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true, SphericalLerp = true });
	}
}
