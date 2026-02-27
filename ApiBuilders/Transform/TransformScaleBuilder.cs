using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Transform
{
	/// <summary>
	/// Fluent builder for Scale-Towards blocks.
	/// Usage: Transform.ScaleTowards(targetScale).Speed(1).Responsiveness(2).LockY()
	///        Transform.ScaleTowards(targetScale).Speed(1).Lerp()
	///        Transform.ScaleTowards(targetScale).Speed(1).Slerp()
	/// </summary>
	public readonly struct TransformScaleBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly Script Script;
		internal readonly TransformTowardsVariableOptions Options;
		internal readonly BuilderToken Token;

		internal TransformScaleBuilder(Script script, TransformTowardsVariableOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => FinalizeBuilder(capturedScript, in capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(TransformScaleBuilder<T> b) =>
			FinalizeBuilder(b.Script, in b.Options, b.Token);

		internal static ScriptActionBlock FinalizeBuilder(Script script, in TransformTowardsVariableOptions options, BuilderToken token)
		{
			var block = TransformScaleTowardsBlock.Create(options.TargetScale, options.Speed, options.DeadZone, options.LockX, options.LockY, options.LockZ, options.Responsiveness);
			script.FinalizeBuilderToken(token);
			return block;
		}

		internal static TransformScaleTowardsLerpBlock FinalizeLerpBuilder(Script script, in TransformTowardsVariableOptions options, BuilderToken token, Boolean slerp)
		{
			var block = TransformScaleTowardsLerpBlock.Create(options.TargetScale, options.Speed, options.DeadZone, options.LockX, options.LockY, options.LockZ, options.Responsiveness, slerp);
			script.FinalizeBuilderToken(token);
			return block;
		}
	}

	public static class TransformScaleBuilderExtensions
	{
		/// <summary> Scale speed in units per second (for linear) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
		public static TransformScaleBuilder<TransformBuilderReady> Speed<T>(this TransformScaleBuilder<T> b, Double speed)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Speed = speed;
			return new TransformScaleBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Minimum scale-distance threshold before scaling begins (prevents micro-jitter). </summary>
		public static TransformScaleBuilder<TransformBuilderReady> DeadZone<T>(this TransformScaleBuilder<T> b, Double deadZone)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.DeadZone = deadZone;
			return new TransformScaleBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Multiplies delta time; larger values produce faster approach. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> Responsiveness<T>(this TransformScaleBuilder<T> b, Double responsiveness)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.Responsiveness = responsiveness;
			return new TransformScaleBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents scaling along the X axis. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> LockX<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockX = true;
			return new TransformScaleBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents scaling along the Y axis. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> LockY<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockY = true;
			return new TransformScaleBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Prevents scaling along the Z axis. </summary>
		public static TransformScaleBuilder<TransformBuilderReady> LockZ<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockZ = true;
			return new TransformScaleBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformScaleTowardsLerpBlock Lerp<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformScaleBuilder<T>.FinalizeLerpBuilder(b.Script, in b.Options, b.Token, slerp: false);

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformScaleTowardsLerpBlock Slerp<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady =>
			TransformScaleBuilder<T>.FinalizeLerpBuilder(b.Script, in b.Options, b.Token, slerp: true);
	}
}
