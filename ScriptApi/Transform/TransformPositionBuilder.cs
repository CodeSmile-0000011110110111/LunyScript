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
		internal readonly BuilderToken Token;
		internal readonly TransformBuilderOptions Options;

		internal TransformPositionBuilder(Script script, BuilderToken token, in TransformBuilderOptions options)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedOptions = options;
			token.AutoFinish = () => Finish(script, token, capturedOptions);
		}

		public static implicit operator ScriptActionBlock(TransformPositionBuilder<T> b) => Finish(b.Script, b.Token, b.Options);

		internal static ScriptActionBlock Finish(Script script, BuilderToken token, in TransformBuilderOptions options)
		{
			var block = TransformPositionLinearTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness);
			script.MarkBuilderTokenFinished(token);
			return block;
		}

		internal static TransformPositionLerpTowardsObjectBlock FinishLerpBuilder(Script script, BuilderToken token,
			in TransformBuilderOptions options)
		{
			var block = TransformPositionLerpTowardsObjectBlock.Create(options.Target, options.Speed, options.DeadZone, options.AxisLock,
				options.Responsiveness, options.SphericalLerp);
			script.MarkBuilderTokenFinished(token);
			return block;
		}
	}

	public static class TransformMoveBuilderExtensions
	{
		/// <summary> Movement speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformPositionBuilder<TransformBuilderReady> Speed<T>(this TransformPositionBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { Speed = speed });

		/// <summary> Minimum distance threshold before movement begins (prevents micro-jitter). </summary>
		public static TransformPositionBuilder<TransformBuilderReady> DeadZone<T>(this TransformPositionBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { DeadZone = deadZone });

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> Responsiveness<T>(this TransformPositionBuilder<T> b,
			Double responsiveness)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { Responsiveness = responsiveness });

		/// <summary> Prevents movement along the X axis. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> LockX<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Prevents movement along the Y axis. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> LockY<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Prevents movement along the Z axis. </summary>
		public static TransformPositionBuilder<TransformBuilderReady> LockZ<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformPositionBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformPositionLerpTowardsObjectBlock Lerp<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformPositionBuilder<T>.FinishLerpBuilder(b.Script, b.Token, b.Options with { Lerp = true });

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformPositionLerpTowardsObjectBlock Slerp<T>(this TransformPositionBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformPositionBuilder<T>.FinishLerpBuilder(b.Script, b.Token, b.Options with { Lerp = true, SphericalLerp = true });
	}
}
