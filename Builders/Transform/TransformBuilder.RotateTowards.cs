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
			var options = new TransformRotateTowardsOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(RotateTowards)),
				Target = target,
				Speed = 90.0,
				DeadZone = 0.1,
				LockAxis = LunyVector3.One,
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

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Lerp<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Lerp = true, SphericalLerp = false });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Slerp<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Lerp = true, SphericalLerp = true });

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
	}

	/// <summary>
	/// Fluent builder for Rotate-Towards blocks.
	/// Usage: Transform.RotateTowards(target).Speed(45).Responsiveness(2).LockY()
	///        Transform.RotateTowards(target).Speed(45).Lerp()
	///        Transform.RotateTowards(target).Speed(45).Slerp()
	/// </summary>
	public readonly struct TransformRotateTowardsBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformRotateTowardsOptions Options;

		internal TransformRotateTowardsBuilder(in TransformRotateTowardsOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformRotateTowardsBuilder<T> b) => Finish(b.Options);

		private static ActionBlock Finish(in TransformRotateTowardsOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.Lerp
				? TransformRotateTowardsObjectLerpBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockAxis,
					options.SphericalLerp, options.Trace)
				: TransformRotateTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockAxis,
					options.Trace);
		}
	}

	internal record TransformRotateTowardsOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyObjectRef Target;
		public Double Speed;
		public Double DeadZone;
		public LunyVector3 LockAxis;
		public Boolean Lerp;
		public Boolean SphericalLerp;

		public void LockAxisX() => LockAxis = VectorUtil.ClearX(LockAxis);
		public void LockAxisY() => LockAxis = VectorUtil.ClearY(LockAxis);
		public void LockAxisZ() => LockAxis = VectorUtil.ClearZ(LockAxis);
	}
}
