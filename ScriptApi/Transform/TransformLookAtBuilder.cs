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
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly TransformBuilderOptions Options;

		internal TransformLookAtBuilder(Script script, BuilderToken token, in TransformBuilderOptions options)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedOptions = options;
			token.AutoFinish = () => Finish(script, token, capturedOptions);
		}

		public static implicit operator ScriptActionBlock(TransformLookAtBuilder<T> b) => Finish(b.Script, b.Token, in b.Options);

		internal static ScriptActionBlock Finish(Script script, BuilderToken token, in TransformBuilderOptions options)
		{
			var block = TransformRotationLookAtBlock.Create(options.Target, options.WorldUp, options.AxisLock);
			script.MarkBuilderTokenFinished(token);
			return block;
		}
	}

	public static class TransformLookAtBuilderExtensions
	{
		/// <summary> Overrides the world-up vector used when computing the look rotation. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> WorldUp<T>(this TransformLookAtBuilder<T> b, LunyVector3 worldUp)
			where T : struct, ITransformBuilderReady => new(b.Script, b.Token, b.Options with { WorldUp = worldUp });

		/// <summary> Locks the X axis: prevents the look direction from changing on the X axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockX<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisX();
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Locks the Y axis: prevents the look direction from changing on the Y axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockY<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisY();
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}

		/// <summary> Locks the Z axis: prevents the look direction from changing on the Z axis. </summary>
		public static TransformLookAtBuilder<TransformBuilderReady> LockZ<T>(this TransformLookAtBuilder<T> b)
			where T : struct, ITransformBuilderReady
		{
			var options = b.Options;
			options.LockAxisZ();
			return new TransformLookAtBuilder<TransformBuilderReady>(b.Script, b.Token, options);
		}
	}
}
