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
	public readonly struct TransformPositionBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly Script Script;
		internal readonly TransformTowardsObjectOptions Options;
		internal readonly BuilderToken Token;

		internal TransformPositionBuilder(Script script, TransformTowardsObjectOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinish(() => Finish(capturedScript, in capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(TransformPositionBuilder<T> b) =>
			Finish(b.Script, in b.Options, b.Token);

		internal static ScriptActionBlock Finish(Script script, in TransformTowardsObjectOptions options, BuilderToken token)
		{
			var block = TransformPositionLinearTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock, options.Responsiveness);
			script.MarkBuilderTokenFinished(token);
			return block;
		}

		internal static TransformPositionLerpTowardsObjectBlock FinishLerpBuilder(Script script, in TransformTowardsObjectOptions options, BuilderToken token, Boolean slerp)
		{
			var block = TransformPositionLerpTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock, options.Responsiveness, slerp);
			script.MarkBuilderTokenFinished(token);
			return block;
		}
	}

	public static class TransformMoveBuilderExtensions
	{
		/// <summary> Movement speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformPositionBuilder<TransformBuilderReady> Speed<T>(this TransformPositionBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Speed = speed;
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Minimum distance threshold before movement begins (prevents micro-jitter). </summary>
		public static TransformPositionBuilder<TransformBuilderReady> DeadZone<T>(this TransformPositionBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.DeadZone = deadZone;
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> Responsiveness<T>(this TransformPositionBuilder<T> b, Double responsiveness)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Responsiveness = responsiveness;
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents movement along the X axis. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> LockX<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents movement along the Y axis. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> LockY<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents movement along the Z axis. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> LockZ<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformPositionLerpTowardsObjectBlock Lerp<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformPositionBuilder<T>.FinishLerpBuilder(b.Script, in b.Options, b.Token, slerp: false);

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformPositionLerpTowardsObjectBlock Slerp<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformPositionBuilder<T>.FinishLerpBuilder(b.Script, in b.Options, b.Token, slerp: true);
	}
}
