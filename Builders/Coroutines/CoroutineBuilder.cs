using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

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
		private readonly LunyStackTrace _trace;
		private readonly String _name;

		internal CoroutineBuilder(Script script, String name, LunyStackTrace trace)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_trace = trace;
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
		}

		/// <summary>
		/// A finite timer coroutine which fires after the specified duration. The Do(blocks) execute when elapsed.
		/// </summary>
		public CoroutineTimerBuilder<CoroutineTimerAmountSet> In(Double duration) => new(_script,
			_script.CreateBuilderToken(_name, "Coroutine." + nameof(In)), _trace, _name, duration, false);

		/// <summary>
		/// A repeating timer coroutine which fires at the specified interval. The Do(blocks) execute on every interval elapse.
		/// </summary>
		public CoroutineTimerBuilder<CoroutineTimerAmountSet> Every(Double interval) => new(_script,
			_script.CreateBuilderToken(_name, "Coroutine." + nameof(Every)), _trace, _name, interval, true);

		internal static ICoroutineBlock Finish(in CoroutineOptions options)
		{
			ThrowIfAllSequencesEmpty(options);

			var block = CoroutineBlock.Create(options);
			var objectEvent = options.ProcessMode == Coroutine.UpdateMode.Heartbeat
				? LunyObjectEvent.Heartbeat
				: LunyObjectEvent.FrameUpdate;

			options.Script.Scheduler.ScheduleObjectEventSequence(block, objectEvent);
			options.Script.MarkBuilderTokenFinished(options.Token);
			return block;
		}

		private static void ThrowIfAllSequencesEmpty(in CoroutineOptions options)
		{
			if (options.WhenElapsed == null && options.WhenStarted == null && options.WhenStopped == null &&
			    options.WhenPaused == null && options.WhenResumed == null && options.WhenProcessing == null)
				throw new LunyScriptException($"{options.Token.Type} '{options.Name}' without any blocks: {options.Script}");
		}
	}
}
