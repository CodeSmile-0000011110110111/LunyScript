using LunyScript.Blocks.Transform;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Fluent builder for Move-Towards blocks.
	/// Usage: Transform.MoveTowards(target).Speed(3).Responsiveness(2).LockY().Do()
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
		}
	}

	public static class TransformMoveBuilderExtensions
	{
		/// <summary> Movement speed in units per second (for <c>Do()</c>) or lerp factor (for <c>Lerp()</c>/<c>Slerp()</c>). </summary>
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

		/// <summary> Linear step (MoveTowards) — speed is units per second. </summary>
		public static TransformMoveTowardsBlock Do<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var o = b.Options;
			var block = TransformMoveTowardsBlock.Create(o.Target, o.Speed, o.DeadZone, o.LockX, o.LockY, o.LockZ, o.Responsiveness);
			b.Script.FinalizeToken(b.Token);
			return block;
		}

		/// <summary> Lerp interpolation — speed is the lerp factor. </summary>
		public static TransformMoveTowardsLerpBlock Lerp<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var o = b.Options;
			var block = TransformMoveTowardsLerpBlock.Create(o.Target, o.Speed, o.DeadZone, o.LockX, o.LockY, o.LockZ, o.Responsiveness);
			b.Script.FinalizeToken(b.Token);
			return block;
		}

		/// <summary> Spherical interpolation — speed is the slerp factor. </summary>
		public static TransformMoveTowardsLerpBlock Slerp<T>(this TransformMoveBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var o = b.Options;
			var block = TransformMoveTowardsLerpBlock.Create(o.Target, o.Speed, o.DeadZone, o.LockX, o.LockY, o.LockZ, o.Responsiveness, true);
			b.Script.FinalizeToken(b.Token);
			return block;
		}
	}
}
