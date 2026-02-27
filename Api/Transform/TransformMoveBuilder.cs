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
	public readonly struct TransformMoveBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly Script Script;
		internal readonly TransformTowardsObjectOptions Options;
		internal readonly BuilderToken Token;

		internal TransformMoveBuilder(Script script, TransformTowardsObjectOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => FinalizeBuilder(capturedScript, in capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(TransformMoveBuilder<T> b) =>
			FinalizeBuilder(b.Script, in b.Options, b.Token);

		internal static ScriptActionBlock FinalizeBuilder(Script script, in TransformTowardsObjectOptions options, BuilderToken token)
		{
			var block = TransformMoveTowardsBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockX, options.LockY, options.LockZ, options.Responsiveness);
			script.FinalizeBuilderToken(token);
			return block;
		}

		internal static TransformMoveTowardsLerpBlock FinalizeLerpBuilder(Script script, in TransformTowardsObjectOptions options, BuilderToken token, Boolean slerp)
		{
			var block = TransformMoveTowardsLerpBlock.Create(options.Target, options.Speed, options.DeadZone, options.LockX, options.LockY, options.LockZ, options.Responsiveness, slerp);
			script.FinalizeBuilderToken(token);
			return block;
		}
	}

	public static class TransformMoveBuilderExtensions
	{
		/// <summary> Movement speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformMoveBuilder<TransformBuilderReady> Speed<T>(this TransformMoveBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Speed = speed;
			return new TransformMoveBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Minimum distance threshold before movement begins (prevents micro-jitter). </summary>
		public static TransformMoveBuilder<TransformBuilderReady> DeadZone<T>(this TransformMoveBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.DeadZone = deadZone;
			return new TransformMoveBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformMoveBuilder<TransformBuilderReady> Responsiveness<T>(this TransformMoveBuilder<T> b, Double responsiveness)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Responsiveness = responsiveness;
			return new TransformMoveBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents movement along the X axis. </summary>
		public static TransformMoveBuilder<TransformBuilderReady> LockX<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockX = true;
			return new TransformMoveBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents movement along the Y axis. </summary>
		public static TransformMoveBuilder<TransformBuilderReady> LockY<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockY = true;
			return new TransformMoveBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents movement along the Z axis. </summary>
		public static TransformMoveBuilder<TransformBuilderReady> LockZ<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockZ = true;
			return new TransformMoveBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformMoveTowardsLerpBlock Lerp<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformMoveBuilder<T>.FinalizeLerpBuilder(b.Script, in b.Options, b.Token, slerp: false);

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformMoveTowardsLerpBlock Slerp<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformMoveBuilder<T>.FinalizeLerpBuilder(b.Script, in b.Options, b.Token, slerp: true);
	}
}
