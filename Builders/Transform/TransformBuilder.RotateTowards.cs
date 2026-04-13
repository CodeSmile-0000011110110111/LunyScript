using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Rotate toward the target orientation each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (degrees/sec), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformRotateTowardsBuilder<TransformBuilderReady> RotateTowards(LunyObjectRef target)
		{
			var token = _script.CreateBuilderToken(nameof(RotateTowards), "Transform." + nameof(RotateTowards));
			var options = new TransformTowardsBuilderOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(RotateTowards)),
				Target = target,
				Speed = 90.0,
				DeadZone = 0.1,
				Responsiveness = 1.0,
				AxisLock = LunyVector3.One,
			};
			return new TransformRotateTowardsBuilder<TransformBuilderReady>(options);
		}
	}
		public static class TransformRotateBuilderExtensions
	{
		/// <summary> Rotation speed in degrees per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Speed<T>(this TransformRotateTowardsBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum angle threshold in degrees before rotation begins (prevents micro-jitter). </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> DeadZone<T>(this TransformRotateTowardsBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Responsiveness<T>(this TransformRotateTowardsBuilder<T> b,
			Double responsiveness)
			where T : struct, ITransformBuilderReady => new(b.Options with { Responsiveness = responsiveness });

		/// <summary> Prevents rotation around the X axis. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> LockX<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformRotateTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents rotation around the Y axis. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> LockY<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformRotateTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Prevents rotation around the Z axis. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> LockZ<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformRotateTowardsBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotateTowardsObjectLerpBlock Lerp<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => TransformRotateTowardsBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotateTowardsObjectLerpBlock Slerp<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotateTowardsBuilder<T>.FinishLerpBuilder(b.Options with { Lerp = true, SphericalLerp = true });
	}
	/// <summary>
	/// Fluent builder for Rotate-Towards blocks.
	/// Usage: Transform.RotateTowards(target).Speed(45).Responsiveness(2).LockY()
	///        Transform.RotateTowards(target).Speed(45).Lerp()
	///        Transform.RotateTowards(target).Speed(45).Slerp()
	/// </summary>
	public readonly struct TransformRotateTowardsBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformTowardsBuilderOptions Options;

		internal TransformRotateTowardsBuilder(in TransformTowardsBuilderOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformRotateTowardsBuilder<T> b) => Finish(b.Options);

		internal static ActionBlock Finish(in TransformTowardsBuilderOptions options)
		{
			var block = TransformRotateTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.Trace);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}

		internal static TransformRotateTowardsObjectLerpBlock FinishLerpBuilder(in TransformTowardsBuilderOptions options)
		{
			var block = TransformRotateTowardsObjectLerpBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.SphericalLerp, options.Trace);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}
	}

}
