using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript.ApiBuilders.Transform
{
	/// <summary>
	/// Fluent builder for <see cref="TransformLookAtBlock"/>.
	/// Usage: Transform.LookAt(target).WorldUp(v).LockX().Do()
	/// </summary>
	public readonly struct TransformLookAtBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly Script Script;
		internal readonly TransformLookAtOptions Options;
		internal readonly BuilderToken Token;

		internal TransformLookAtBuilder(Script script, TransformLookAtOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;
		}
	}

	public static class TransformLookAtBuilderExtensions
	{
		/// <summary> Overrides the world-up vector used when computing the look rotation. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> WorldUp<T>(this TransformLookAtBuilder<T> b, LunyVector3 worldUp)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.WorldUp = worldUp;
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Locks the X axis: prevents the look direction from changing on the X axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockX<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.AxisLock = new LunyVector3(0d, options.AxisLock.Y, options.AxisLock.Z);
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Locks the Y axis: prevents the look direction from changing on the Y axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockY<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.AxisLock = new LunyVector3(options.AxisLock.X, 0d, options.AxisLock.Z);
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Locks the Z axis: prevents the look direction from changing on the Z axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockZ<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.AxisLock = new LunyVector3(options.AxisLock.X, options.AxisLock.Y, 0d);
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, options, b.Token);
		}

		/// <summary> Finalizes the builder and returns the executable block. </summary>
		public static TransformLookAtBlock Do<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var block = TransformLookAtBlock.Create(b.Options.Target, b.Options.WorldUp, b.Options.AxisLock);
			b.Script.FinalizeBuilderToken(b.Token);
			return block;
		}
	}
}
