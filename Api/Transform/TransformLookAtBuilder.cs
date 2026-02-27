using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript.Api.Transform
{
	/// <summary>
	/// Fluent builder for <see cref="TransformLookAtBlock"/>.
	/// Usage: Transform.LookAt(target).WorldUp(v).LockX()
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

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => FinalizeBuilder(capturedScript, in capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(TransformLookAtBuilder<T> b) =>
			FinalizeBuilder(b.Script, in b.Options, b.Token);

		internal static ScriptActionBlock FinalizeBuilder(Script script, in TransformLookAtOptions options, BuilderToken token)
		{
			var block = TransformLookAtBlock.Create(options.Target, options.WorldUp, options.AxisLock);
			script.FinalizeBuilderToken(token);
			return block;
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
	}
}
