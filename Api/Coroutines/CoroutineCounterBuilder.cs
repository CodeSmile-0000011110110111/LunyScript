using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineCounterBuilder {}
	public interface ICoroutineCounterBuilderStart : ICoroutineCounterBuilder {}
	public struct CoroutineCounterBuilderStart : ICoroutineCounterBuilderStart {}

	/// <summary>
	/// Counter runs blocks either once or repeatedly after a given number of frames/heartbeats.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	///        Counter("name").Every(10).Heartbeats().Do(blocks);
	/// </summary>
	public readonly struct CoroutineCounterBuilder<T> where T : struct, ICoroutineCounterBuilder
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;

		internal CoroutineCounterBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		internal static CoroutineCounterBuilder<CoroutineCounterBuilderStart> Create(Script script, String name)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Counter name is null or empty", nameof(name));

			var options = new CoroutineOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Counter()");
			return new CoroutineCounterBuilder<CoroutineCounterBuilderStart>(script, token, options);
		}
	}

	public interface ICoroutineCounterBuilderContinuationSet : ICoroutineCounterBuilder {}
	public struct CoroutineCounterBuilderContinuationSet : ICoroutineCounterBuilderContinuationSet {}

	public static class CounterBuilderContinuationExtensions
	{
		/// <summary>Sets the counter to fire once after the specified count.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderContinuationSet>
			In(this CoroutineCounterBuilder<CoroutineCounterBuilderStart> b, Int32 targetCount) => new(b.Script, b.Token,
			b.Options with { CounterTarget = targetCount, ContinuationMode = Coroutine.Continuation.Finite });

		/// <summary>Sets the counter to fire repeatedly at the specified interval.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderContinuationSet>
			Every(this CoroutineCounterBuilder<CoroutineCounterBuilderStart> b, Int32 interval) => new(b.Script, b.Token,
			b.Options with { CounterTarget = interval, ContinuationMode = Coroutine.Continuation.Repeating });
	}

	public interface ICoroutineCounterBuilderUnitSet : ICoroutineCounterBuilder {}
	public struct CoroutineCounterBuilderUnitSet : ICoroutineCounterBuilderUnitSet {}

	public static class CounterBuilderUnitExtensions
	{
		/// <summary>Counts frame updates.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderUnitSet> Frames<T>(this CoroutineCounterBuilder<T> b)
			where T : struct, ICoroutineCounterBuilderContinuationSet =>
			new(b.Script, b.Token, b.Options with { ProcessMode = Coroutine.Process.FrameUpdate });

		/// <summary>Counts heartbeat (fixed step) updates.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderUnitSet> Heartbeats<T>(this CoroutineCounterBuilder<T> b)
			where T : struct, ICoroutineCounterBuilderContinuationSet =>
			new(b.Script, b.Token, b.Options with { ProcessMode = Coroutine.Process.Heartbeat });
	}

	public static class CounterBuilderFinalExtensions
	{
		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static ICoroutineBlock Do<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] elapsedBlocks)
			where T : struct, ICoroutineCounterBuilderUnitSet => CoroutineBuilder.Finalize(b.Script, b.Token,
			CoroutineOptions.ForCounterCoroutine(b.Options.Name, b.Options.CounterTarget, b.Options.ContinuationMode, b.Options.ProcessMode,
				elapsedBlocks:elapsedBlocks));
	}
}
