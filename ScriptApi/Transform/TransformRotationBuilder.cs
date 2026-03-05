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
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly TransformBuilderOptions Options;

		internal TransformRotationBuilder(Script script, BuilderToken token, in TransformBuilderOptions options)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedOptions = options;
			token.AutoFinish = () => Finish(script, token, capturedOptions);
		}

		public static implicit operator ScriptActionBlock(TransformRotationBuilder<T> b) => Finish(b.Script, b.Token, b.Options);

		internal static ScriptActionBlock Finish(Script script, BuilderToken token, in TransformBuilderOptions options)
		{
			var block = TransformRotationLinearTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness);
			script.MarkBuilderTokenFinished(token);
			return block;
		}

		internal static TransformRotationLerpTowardsObjectBlock FinishLerpBuilder(Script script, BuilderToken token,
			in TransformBuilderOptions options)
		{
			var block = TransformRotationLerpTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.SphericalLerp);
			script.MarkBuilderTokenFinished(token);
			return block;
		}
	}

	public static class TransformRotateBuilderExtensions
	{
		/// <summary> Rotation speed in degrees per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformRotationBuilder<TransformBuilderReady> Speed<T>(this TransformRotationBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { Speed = speed });

		/// <summary> Minimum angle threshold in degrees before rotation begins (prevents micro-jitter). </summary>
		public static TransformRotationBuilder<TransformBuilderReady> DeadZone<T>(this TransformRotationBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { DeadZone = deadZone });

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> Responsiveness<T>(this TransformRotationBuilder<T> b,
			Double responsiveness)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { Responsiveness = responsiveness });

		/// <summary> Prevents rotation around the X axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockX<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Prevents rotation around the Y axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockY<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Prevents rotation around the Z axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockZ<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotationLerpTowardsObjectBlock Lerp<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotationBuilder<T>.FinishLerpBuilder(b.Script, b.Token, b.Options with { Lerp = true });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotationLerpTowardsObjectBlock Slerp<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotationBuilder<T>.FinishLerpBuilder(b.Script, b.Token, b.Options with { Lerp = true, SphericalLerp = true });
	}
}
