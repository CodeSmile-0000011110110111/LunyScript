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
		internal readonly TransformTowardsObjectOptions Options;
		internal readonly BuilderToken Token;

		internal TransformRotationBuilder(Script script, TransformTowardsObjectOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinish(() => Finish(capturedScript, in capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(TransformRotationBuilder<T> b) =>
			Finish(b.Script, in b.Options, b.Token);

		internal static ScriptActionBlock Finish(Script script, in TransformTowardsObjectOptions options, BuilderToken token)
		{
			var block = TransformRotationLinearTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock, options.Responsiveness);
			script.MarkBuilderTokenFinished(token);
			return block;
		}

		internal static TransformRotationLerpTowardsObjectBlock FinishLerpBuilder(Script script, in TransformTowardsObjectOptions options, BuilderToken token, Boolean slerp)
		{
			var block = TransformRotationLerpTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock, options.Responsiveness, slerp);
			script.MarkBuilderTokenFinished(token);
			return block;
		}
	}

	public static class TransformRotateBuilderExtensions
	{
		/// <summary> Rotation speed in degrees per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformRotationBuilder<TransformBuilderReady> Speed<T>(this TransformRotationBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Speed = speed;
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Minimum angle threshold in degrees before rotation begins (prevents micro-jitter). </summary>
		public static TransformRotationBuilder<TransformBuilderReady> DeadZone<T>(this TransformRotationBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.DeadZone = deadZone;
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> Responsiveness<T>(this TransformRotationBuilder<T> b, Double responsiveness)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Responsiveness = responsiveness;
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents rotation around the X axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockX<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents rotation around the Y axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockY<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents rotation around the Z axis. </summary>
		public static TransformRotationBuilder<TransformBuilderReady> LockZ<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformRotationBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotationLerpTowardsObjectBlock Lerp<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotationBuilder<T>.FinishLerpBuilder(b.Script, in b.Options, b.Token, slerp: false);

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotationLerpTowardsObjectBlock Slerp<T>(this TransformRotationBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotationBuilder<T>.FinishLerpBuilder(b.Script, in b.Options, b.Token, slerp: true);
	}
}
