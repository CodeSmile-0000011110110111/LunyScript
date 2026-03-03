using Luny;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
	///        Coroutine("name").OnFrameUpdate(blocks).WhenStopped(blocks).Do(blocks);
	///        Coroutine("name").OnHeartbeat(blocks).Do(blocks);
	/// </summary>
	public readonly struct CoroutineBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;

		internal CoroutineBuilder(Script script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
			_token = script.CreateBuilderToken(_name, "Coroutine()");
		}

		/// <summary>Sets the coroutine duration. Returns a builder to specify the time unit.</summary>
		public CoroutineForBuilder<CoroutineForBuilderStart> For(Double duration) => new(_script, _token, _name, duration);

		/*
		/// <summary>Creates an open-ended coroutine (runs until stopped) which runs the blocks every frame.</summary>
		public CoroutineBuilder<CoroutineFrameUpdate> OnFrameUpdate(params ScriptActionBlock[] blocks) => new(_script, _token,
			CoroutineOptions.ForOpenEndedCoroutine(_name, Coroutine.Process.FrameUpdate) with { OnFrameUpdate = blocks });

		/// <summary>Creates an open-ended coroutine (runs until stopped) which runs the blocks every heartbeat (fixed step).</summary>
		public CoroutineBuilder<CoroutineHeartbeat> OnHeartbeat(params ScriptActionBlock[] blocks) => new(_script, _token,
			CoroutineOptions.ForOpenEndedCoroutine(_name, Coroutine.Process.Heartbeat) with { OnHeartbeat = blocks });
			*/

		internal static ICoroutineBlock Finalize(Script script, BuilderToken token, in CoroutineOptions options)
		{
			WarnIfAllSequencesEmpty(script, token, options);
			var block = script.RuntimeContext.Coroutines.Register(options);
			script.FinalizeBuilderToken(token);
			return block;
		}

		private static void WarnIfAllSequencesEmpty(Script script, BuilderToken token, in CoroutineOptions options)
		{
			if (options.OnFrameUpdate == null && options.OnHeartbeat == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
				LunyLogger.LogWarning($"{token.Type} '{options.Name}' has no blocks. Add blocks or remove it.", script);
		}
	}

	/*
	public interface ICoroutineBuilderState {}
	public interface ICoroutineFrameUpdate : ICoroutineBuilderState {}
	public struct CoroutineFrameUpdate : ICoroutineFrameUpdate, ICoroutineFinal {}
	public interface ICoroutineHeartbeat : ICoroutineBuilderState {}
	public struct CoroutineHeartbeat : ICoroutineHeartbeat, ICoroutineFinal {}

	/// <summary>
	/// Generic step-builder for open-ended coroutines (frame-update or heartbeat).
	/// </summary>
	public readonly struct CoroutineBuilder<T> where T : struct, ICoroutineBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;

		internal CoroutineBuilder(Script script, BuilderToken token, CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(script, token, options));
		}
	}

	public interface ICoroutineFinal : ICoroutineBuilderState {}

	public static class CoroutineBuilderExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineBuilder<T> WhenStarted<T>(this CoroutineBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFinal => new(b.Script, b.Token, b.Options);

		// var options = b.Options with { OnStarted = blocks };
		// b.Token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(b.Script, b.Token, options));
		// return new CoroutineBuilder<T>(b.Script, b.Token, options);
		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineBuilder<T> WhenStopped<T>(this CoroutineBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFinal => new(b.Script, b.Token, b.Options with { OnStopped = blocks });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineBuilder<T> WhenPaused<T>(this CoroutineBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFinal => new(b.Script, b.Token, b.Options with { OnPaused = blocks });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineBuilder<T> WhenResumed<T>(this CoroutineBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFinal => new(b.Script, b.Token, b.Options with { OnResumed = blocks });
	}

	public static class CoroutineBuilderFinalExtensions
	{
		/// <summary>Additional update blocks. Finalizes the builder.</summary>
		public static ICoroutineBlock Do<T>(this CoroutineBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFinal
		{
			if (b.Options.ProcessMode == Coroutine.Process.Heartbeat)
				return CoroutineBuilder.Finalize(b.Script, b.Token, b.Options with { OnHeartbeat = blocks });

			if (b.Options.ProcessMode == Coroutine.Process.FrameUpdate)
				return CoroutineBuilder.Finalize(b.Script, b.Token, b.Options with { OnFrameUpdate = blocks });

			throw new ArgumentOutOfRangeException(nameof(b.Options.ProcessMode), b.Options.ProcessMode.ToString());
		}
	}
*/
}
