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
		/// Move toward the target position each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformMoveTowardsBuilder<TransformBuilderReady> MoveTowards(LunyObjectRef target)
		{
			var options = new TransformMoveTowardsOptions
			{
				Script = _script,
				Token = _script.CreateBuilderToken(nameof(MoveTowards), "Transform." + nameof(MoveTowards)),
				Trace = _trace.Add(nameof(MoveTowards)),
				Target = target,
				Speed = 3.0,
				DeadZone = 0.1,
				LockAxis = LunyVector3.One,
			};
			return new TransformMoveTowardsBuilder<TransformBuilderReady>(options);
		}
	}

	public static class TransformMoveTowardsBuilderExtensions
	{
		/// <summary> Movement speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> Speed<T>(this TransformMoveTowardsBuilder<T> b, VariableBlock speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Minimum distance threshold before movement begins (prevents micro-jitter). </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> DeadZone<T>(this TransformMoveTowardsBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Options with { DeadZone = deadZone });

		/// <summary> Linear interpolation — speed is the lerp factor. </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> Lerp<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Linear });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformMoveTowardsBuilder<TransformBuilderReady> Slerp<T>(this TransformMoveTowardsBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Spherical });

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
	}

	/// <summary>
	/// Fluent builder for Move-Towards blocks.
	/// Usage: Transform.MoveTowards(target).Speed(3).Responsiveness(2).LockY()
	///        Transform.MoveTowards(target).Speed(3).Lerp()
	///        Transform.MoveTowards(target).Speed(3).Slerp()
	/// </summary>
	public readonly struct TransformMoveTowardsBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformMoveTowardsOptions Options;

		internal TransformMoveTowardsBuilder(in TransformMoveTowardsOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformMoveTowardsBuilder<T> b) => Finish(b.Options);

		private static ActionBlock Finish(in TransformMoveTowardsOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformMoveTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockAxis,
				options.Interpolation, options.Trace);
		}
	}

	internal record TransformMoveTowardsOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyObjectRef Target;
		public VariableBlock Speed;
		public Double DeadZone;
		public LunyVector3 LockAxis;
		public LunyInterpolation Interpolation;

		public void LockAxisX() => LockAxis = VectorUtil.ClearX(LockAxis);
		public void LockAxisY() => LockAxis = VectorUtil.ClearY(LockAxis);
		public void LockAxisZ() => LockAxis = VectorUtil.ClearZ(LockAxis);
	}
}
