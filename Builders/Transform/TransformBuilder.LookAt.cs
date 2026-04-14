using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary>
		/// Orient to face the target each frame.
		/// Chain <c>.WorldUp(v)</c>, <c>.LockX()</c>, <c>.LockY()</c>, <c>.LockZ()</c>,
		/// <c>.Speed(n)</c>, then call <c>.Lerp()</c> or <c>.Slerp()</c> for smooth interpolation.
		/// </summary>
		public TransformLookAtBuilder<TransformBuilderReady> LookAt(LunyObjectRef target)
		{
			var token = _script.CreateBuilderToken(nameof(LookAt), "Transform.LookAt()");
			var options = new TransformLookAtBuilderOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(LookAt)),
				Target = target,
				WorldUp = LunyVector3.Up,
				LockAxis = LunyVector3.One,
				Speed = 5.0,
			};
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}
	}

	public static class TransformLookAtBuilderExtensions
	{
		/// <summary> Overrides the world-up vector used when computing the look rotation. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> WorldUp<T>(this TransformLookAtBuilder<T> b, LunyVector3 worldUp)
			where T : struct, ITransformBuilderReady => new(b.Options with { WorldUp = worldUp });

		/// <summary> Speed/factor used for <c>Lerp()</c> and <c>Slerp()</c> interpolation. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> Speed<T>(this TransformLookAtBuilder<T> b, VariableBlock speed)
			where T : struct, ITransformBuilderReady => new(b.Options with { Speed = speed });

		/// <summary> Linear interpolation toward the look-at rotation — speed is the lerp factor. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> Lerp<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Linear });

		/// <summary> Spherical interpolation toward the look-at rotation — speed is the slerp factor. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> Slerp<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Interpolation = LunyInterpolation.Spherical });

		/// <summary> Locks the X axis: prevents the look direction from changing on the X axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockX<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Locks the Y axis: prevents the look direction from changing on the Y axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockY<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Locks the Z axis: prevents the look direction from changing on the Z axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockZ<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}
	}

	/// <summary>
	/// Fluent builder for <see cref="TransformLookAtBlock"/>.
	/// Usage: Transform.LookAt(target).WorldUp(v).LockX()
	///        Transform.LookAt(target).Speed(5).Lerp()
	///        Transform.LookAt(target).Speed(5).Slerp()
	/// </summary>
	public readonly struct TransformLookAtBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformLookAtBuilderOptions Options;

		internal TransformLookAtBuilder(in TransformLookAtBuilderOptions options)
		{
			Options = options;
			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformLookAtBuilder<T> b) => Finish(b.Options);

		private static ActionBlock Finish(in TransformLookAtBuilderOptions options)
		{
			var block = TransformLookAtBlock.Create(options.Target, options.WorldUp, options.LockAxis,
				options.Speed, options.Interpolation, options.Trace);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}
	}

	internal record TransformLookAtBuilderOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyObjectRef Target;
		public LunyVector3 WorldUp;
		public LunyVector3 LockAxis;
		public VariableBlock Speed;
		public LunyInterpolation Interpolation;

		public void LockAxisX() => LockAxis = VectorUtil.ClearX(LockAxis);
		public void LockAxisY() => LockAxis = VectorUtil.ClearY(LockAxis);
		public void LockAxisZ() => LockAxis = VectorUtil.ClearZ(LockAxis);
	}
}
