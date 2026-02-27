using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	internal struct CounterOptions
	{
		internal String Name;
		internal Int32 Amount;
		internal Coroutine.Continuation Continuation;
		internal Coroutine.Process Process;
	}

	/// <summary>
	/// Fluent builder for counter coroutines.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	///        Counter("name").Every(10).Heartbeats().Do(blocks);
	/// </summary>
	public readonly struct CounterBuilder
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CounterOptions Options;

		internal CounterBuilder(Script script, BuilderToken token, in CounterOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static CounterBuilder Create(Script script, String name)
		{
			if (script == null)
				throw new ArgumentNullException(nameof(script));
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Counter name is null or empty", nameof(name));

			var options = new CounterOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Counter");
			return new CounterBuilder(script, token, in options);
		}
	}

	public interface ICounterBuilderState {}
	public interface ICounterBuilderStart : ICounterBuilderState {}
	public struct CounterBuilderStart : ICounterBuilderStart {}
	public readonly struct CounterBuilderState<T> where T : struct, ICounterBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CounterOptions Options;

		internal CounterBuilderState(Script script, BuilderToken token, in CounterOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static CounterBuilderState<CounterBuilderStart> Create(Script script, String name)
		{
			if (script == null)
				throw new ArgumentNullException(nameof(script));
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Counter name is null or empty", nameof(name));

			var options = new CounterOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Counter");
			return new CounterBuilderState<CounterBuilderStart>(script, token, in options);
		}
	}

	public interface ICounterBuilderContinuationSet : ICounterBuilderState {}
	public struct CounterBuilderContinuationSet : ICounterBuilderContinuationSet {}

	public static class CounterBuilderContinuationExtensions
	{
		/// <summary>Sets the counter to fire once after the specified count.</summary>
		public static CounterBuilderState<CounterBuilderContinuationSet> In(this CounterBuilderState<CounterBuilderStart> b, Int32 targetCount)
		{
			var options = b.Options;
			options.Amount = targetCount;
			options.Continuation = Coroutine.Continuation.Finite;
			return new CounterBuilderState<CounterBuilderContinuationSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the counter to fire repeatedly at the specified interval.</summary>
		public static CounterBuilderState<CounterBuilderContinuationSet> Every(this CounterBuilderState<CounterBuilderStart> b, Int32 interval)
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutine.Continuation.Repeating;
			return new CounterBuilderState<CounterBuilderContinuationSet>(b.Script, b.Token, in options);
		}
	}

	public interface ICounterUnitSet : ICounterBuilderState {}
	public struct CounterUnitSet : ICounterUnitSet {}

	public static class CounterBuilderUnitExtensions
	{
		/// <summary>Counts frame updates.</summary>
		public static CounterBuilderState<CounterUnitSet> Frames<T>(this CounterBuilderState<T> b)
			where T : struct, ICounterBuilderContinuationSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {b.Options.Amount}");

			var options = b.Options;
			options.Process = Coroutine.Process.FrameUpdate;
			var capturedScript = b.Script;
			var capturedOptions = options;
			b.Token?.SetAutoFinalizer(() =>
			{
				var co = CoroutineOptions.ForCounter(capturedOptions.Name, capturedOptions.Amount, capturedOptions.Continuation,
					capturedOptions.Process);
				CoroutineBuilder.Finalize(capturedScript, in co, b.Token);
			});
			return new CounterBuilderState<CounterUnitSet>(b.Script, b.Token, in options);
		}

		/// <summary>Counts heartbeat (fixed step) updates.</summary>
		public static CounterBuilderState<CounterUnitSet> Heartbeats<T>(this CounterBuilderState<T> b)
			where T : struct, ICounterBuilderContinuationSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {b.Options.Amount}");

			var options = b.Options;
			options.Process = Coroutine.Process.Heartbeat;
			var capturedScript = b.Script;
			var capturedOptions = options;
			b.Token?.SetAutoFinalizer(() =>
			{
				var co = CoroutineOptions.ForCounter(capturedOptions.Name, capturedOptions.Amount, capturedOptions.Continuation,
					capturedOptions.Process);
				CoroutineBuilder.Finalize(capturedScript, in co, b.Token);
			});
			return new CounterBuilderState<CounterUnitSet>(b.Script, b.Token, in options);
		}
	}

	public static class CounterBuilderFinalExtensions
	{
		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static ICounterCoroutineBlock Do<T>(this CounterBuilderState<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICounterUnitSet
		{
			var co = CoroutineOptions.ForCounter(b.Options.Name, b.Options.Amount, b.Options.Continuation, b.Options.Process) with
			{
				OnElapsed = blocks,
			};
			return (ICounterCoroutineBlock)CoroutineBuilder.Finalize(b.Script, in co, b.Token);
		}
	}
}
