using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICounterBuilder {}
	public interface ICounterBuilderStart : ICounterBuilder {}
	public struct CounterBuilderStart : ICounterBuilderStart {}

	/// <summary>
	/// Counter runs blocks either once or repeatedly after a given number of frames/heartbeats.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	///        Counter("name").Every(10).Heartbeats().Do(blocks);
	/// </summary>
	public readonly struct CounterBuilder<T> where T : struct, ICounterBuilder
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

		internal static CounterBuilder<CounterBuilderStart> Create(Script script, String name)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Counter name is null or empty", nameof(name));

			var options = new CounterOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Counter()");
			return new CounterBuilder<CounterBuilderStart>(script, token, in options);
		}
	}

	public interface ICounterBuilderContinuationSet : ICounterBuilder {}
	public struct CounterBuilderContinuationSet : ICounterBuilderContinuationSet {}

	public static class CounterBuilderContinuationExtensions
	{
		/// <summary>Sets the counter to fire once after the specified count.</summary>
		public static CounterBuilder<CounterBuilderContinuationSet> In(this CounterBuilder<CounterBuilderStart> b, Int32 targetCount)
		{
			var options = b.Options;
			options.Amount = targetCount;
			options.Continuation = Coroutine.Continuation.Finite;
			return new CounterBuilder<CounterBuilderContinuationSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the counter to fire repeatedly at the specified interval.</summary>
		public static CounterBuilder<CounterBuilderContinuationSet> Every(this CounterBuilder<CounterBuilderStart> b, Int32 interval)
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutine.Continuation.Repeating;
			return new CounterBuilder<CounterBuilderContinuationSet>(b.Script, b.Token, in options);
		}
	}

	public interface ICounterBuilderUnitSet : ICounterBuilder {}
	public struct CounterBuilderUnitSet : ICounterBuilderUnitSet {}

	public static class CounterBuilderUnitExtensions
	{
		/// <summary>Counts frame updates.</summary>
		public static CounterBuilder<CounterBuilderUnitSet> Frames<T>(this CounterBuilder<T> b)
			where T : struct, ICounterBuilderContinuationSet
		{
			var options = b.Options;
			options.Process = Coroutine.Process.FrameUpdate;
			return new CounterBuilder<CounterBuilderUnitSet>(b.Script, b.Token, in options);
		}

		/// <summary>Counts heartbeat (fixed step) updates.</summary>
		public static CounterBuilder<CounterBuilderUnitSet> Heartbeats<T>(this CounterBuilder<T> b)
			where T : struct, ICounterBuilderContinuationSet
		{
			var options = b.Options;
			options.Process = Coroutine.Process.Heartbeat;
			return new CounterBuilder<CounterBuilderUnitSet>(b.Script, b.Token, in options);
		}
	}

	public static class CounterBuilderFinalExtensions
	{
		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static ICoroutineBlock Do<T>(this CounterBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICounterBuilderUnitSet
		{
			var co = CoroutineOptions.ForCounterCoroutine(b.Options.Name, b.Options.Amount, b.Options.Continuation, b.Options.Process) with
			{
				OnElapsed = blocks,
			};
			return CoroutineBuilder.Finalize(b.Script, b.Token, in co);
		}
	}

	internal struct CounterOptions
	{
		internal String Name;
		internal Int32 Amount;
		internal Coroutine.Continuation Continuation;
		internal Coroutine.Process Process;
	}
}
