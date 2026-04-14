using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Rotate toward the target orientation each frame.
		/// Chain <c>.Speed(n)</c>, <c>.DeadZone(n)</c>, <c>.WorldUp(v)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Instant()</c>, <c>.Lerp()</c> or <c>.Slerp()</c>.
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
				Interpolation = LunyInterpolation.Towards,
				Speed = 45.0,
				DeadZone = 0.1,
				LockAxis = LunyVector3.One,
				WorldUp = LunyVector3.Up,
			};
			return new TransformRotateTowardsBuilder<TransformBuilderReady>(options);
		}
	}

	public static class TransformRotateBuilderExtensions
	{
		/// <summary> Rotation speed in degrees per second (for constant speed) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Speed<T>(this TransformRotateTowardsBuilder<T> b,
			VariableBlock speed) where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum angle threshold in degrees before rotation begins (prevents micro-jitter). </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> DeadZone<T>(this TransformRotateTowardsBuilder<T> b,
			VariableBlock deadZone) where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Overrides the world-up vector used when computing the look rotation. Defaults to <c>Vector3.Up</c>. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> WorldUp<T>(this TransformRotateTowardsBuilder<T> b, LunyVector3 worldUp)
			where T : struct, ITransformBuilderReady => new(b.Options with { WorldUp = worldUp });

		/// <summary> Instantly snaps to face the target in a single frame. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Instant<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Instant });

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Linear<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Linear });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotateTowardsBuilder<TransformBuilderReady> Spherical<T>(this TransformRotateTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Spherical });

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
	/// Usage: Transform.RotateTowards(target).Speed(45).LockY()
	///        Transform.RotateTowards(target).Instant()
	///        Transform.RotateTowards(target).Speed(45).Lerp()
	///        Transform.RotateTowards(target).Speed(45).Slerp()
	///        Transform.RotateTowards(target).WorldUp(Vector3.Forward).Speed(45).Slerp()
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
			return TransformRotateTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockAxis,
				options.Interpolation, options.WorldUp, options.Trace);
		}
	}

	internal record TransformRotateTowardsOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyObjectRef Target;
		public VariableBlock Speed;
		public VariableBlock DeadZone;
		public LunyVector3 LockAxis;
		public LunyVector3 WorldUp;
		public LunyInterpolation Interpolation;

		public void LockAxisX() => LockAxis = VectorUtil.ClearX(LockAxis);
		public void LockAxisY() => LockAxis = VectorUtil.ClearY(LockAxis);
		public void LockAxisZ() => LockAxis = VectorUtil.ClearZ(LockAxis);
	}
}
