using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Transform
{
	/// <summary>
	/// Fluent builder for Scale-Towards blocks.
	/// Usage: Transform.ScaleTowards(targetScale).Speed(1).Responsiveness(2).LockY().Do()
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
		}
	}

	public static class TransformScaleBuilderExtensions
	{
		/// <summary> Scale speed in units per second (for <c>Do()</c>) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
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

		/// <summary> Linear step (MoveTowards per-component) — speed is units per second. </summary>
		public static TransformScaleTowardsBlock Do<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var o = b.Options;
			var block = TransformScaleTowardsBlock.Create(o.TargetScale, o.Speed, o.DeadZone, o.LockX, o.LockY, o.LockZ, o.Responsiveness);
			b.Script.FinalizeToken(b.Token);
			return block;
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformScaleTowardsLerpBlock Lerp<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var o = b.Options;
			var block = TransformScaleTowardsLerpBlock.Create(o.TargetScale, o.Speed, o.DeadZone, o.LockX, o.LockY, o.LockZ, o.Responsiveness,
				false);
			b.Script.FinalizeToken(b.Token);
			return block;
		}

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformScaleTowardsLerpBlock Slerp<T>(this TransformScaleBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var o = b.Options;
			var block = TransformScaleTowardsLerpBlock.Create(o.TargetScale, o.Speed, o.DeadZone, o.LockX, o.LockY, o.LockZ, o.Responsiveness,
				true);
			b.Script.FinalizeToken(b.Token);
			return block;
		}
	}
}
