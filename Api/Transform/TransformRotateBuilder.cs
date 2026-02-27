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
	public readonly struct TransformRotateBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly Script Script;
		internal readonly TransformTowardsObjectOptions Options;
		internal readonly BuilderToken Token;

		internal TransformRotateBuilder(Script script, TransformTowardsObjectOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => FinalizeBuilder(capturedScript, in capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(TransformRotateBuilder<T> b) =>
			FinalizeBuilder(b.Script, in b.Options, b.Token);

		internal static ScriptActionBlock FinalizeBuilder(Script script, in TransformTowardsObjectOptions options, BuilderToken token)
		{
			var block = TransformRotateTowardsBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockX, options.LockY, options.LockZ, options.Responsiveness);
			script.FinalizeBuilderToken(token);
			return block;
		}

		internal static TransformRotateTowardsLerpBlock FinalizeLerpBuilder(Script script, in TransformTowardsObjectOptions options, BuilderToken token, Boolean slerp)
		{
			var block = TransformRotateTowardsLerpBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockX, options.LockY, options.LockZ, options.Responsiveness, slerp);
			script.FinalizeBuilderToken(token);
			return block;
		}
	}

	public static class TransformRotateBuilderExtensions
	{
		/// <summary> Rotation speed in degrees per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformRotateBuilder<TransformBuilderReady> Speed<T>(this TransformRotateBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Speed = speed;
			return new TransformRotateBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Minimum angle threshold in degrees before rotation begins (prevents micro-jitter). </summary>
		public static TransformRotateBuilder<TransformBuilderReady> DeadZone<T>(this TransformRotateBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.DeadZone = deadZone;
			return new TransformRotateBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformRotateBuilder<TransformBuilderReady> Responsiveness<T>(this TransformRotateBuilder<T> b, Double responsiveness)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Responsiveness = responsiveness;
			return new TransformRotateBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents rotation around the X axis. </summary>
		public static TransformRotateBuilder<TransformBuilderReady> LockX<T>(this TransformRotateBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockX = true;
			return new TransformRotateBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents rotation around the Y axis. </summary>
		public static TransformRotateBuilder<TransformBuilderReady> LockY<T>(this TransformRotateBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockY = true;
			return new TransformRotateBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents rotation around the Z axis. </summary>
		public static TransformRotateBuilder<TransformBuilderReady> LockZ<T>(this TransformRotateBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockZ = true;
			return new TransformRotateBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformRotateTowardsLerpBlock Lerp<T>(this TransformRotateBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotateBuilder<T>.FinalizeLerpBuilder(b.Script, in b.Options, b.Token, slerp: false);

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformRotateTowardsLerpBlock Slerp<T>(this TransformRotateBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformRotateBuilder<T>.FinalizeLerpBuilder(b.Script, in b.Options, b.Token, slerp: true);
	}
}
