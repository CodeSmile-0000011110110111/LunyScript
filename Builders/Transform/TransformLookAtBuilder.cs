using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Fluent builder for <see cref="TransformRotationLookAtBlock"/>.
	/// Usage: Transform.LookAt(target).WorldUp(v).LockX()
	/// </summary>
	public readonly struct TransformLookAtBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformBuilderOptions Options;

		internal TransformLookAtBuilder(in TransformBuilderOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformLookAtBuilder<T> b) => Finish(b.Options);

		internal static ActionBlock Finish(in TransformBuilderOptions options)
		{
			var block = TransformRotationLookAtBlock.Create(options.Target, options.WorldUp, options.AxisLock);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}
	}

	public static class TransformLookAtBuilderExtensions
	{
		/// <summary> Overrides the world-up vector used when computing the look rotation. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> WorldUp<T>(this TransformLookAtBuilder<T> b, LunyVector3 worldUp)
			where T : struct, ITransformBuilderReady => new(b.Options with { WorldUp = worldUp });

		/// <summary> Locks the X axis: prevents the look direction from changing on the X axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockX<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Locks the Y axis: prevents the look direction from changing on the Y axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockY<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Locks the Z axis: prevents the look direction from changing on the Z axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockZ<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}
	}
}
