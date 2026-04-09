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
		private readonly String _name;
		private readonly LunyStackTrace _trace;

		internal CoroutineBuilder(Script script, String name, LunyStackTrace trace)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
			_trace = trace;
		}
		// TODO:
		// add .OnUpdate(blocks) that run every time the coroutine updates (frames, or heartbeats)
		// add .TimeSliceOffset() to delay the start of the coroutine (could do manually though)

		/// <summary>
		/// A finite timer coroutine which fires after the specified duration. The Do(blocks) execute when elapsed.
		/// </summary>
		public CoroutineTimerBuilder<CoroutineTimerAmountSet> In(Double duration) => new(_script,
			_script.CreateBuilderToken(_name, "Coroutine.In"), _name, duration, false);

		/// <summary>
		/// A repeating timer coroutine which fires at the specified interval. The Do(blocks) execute on every interval elapse.
		/// </summary>
		public CoroutineTimerBuilder<CoroutineTimerAmountSet> Every(Double interval) => new(_script,
			_script.CreateBuilderToken(_name, "Coroutine.Every"), _name, interval, true);

		internal static ICoroutineBlock Finish(Script script, BuilderToken token, in CoroutineOptions options)
		{
			ThrowIfAllSequencesEmpty(script, token, options);

			var block = CoroutineBlock.Create(options);
			var objectEvent = options.ProcessMode == Coroutine.UpdateMode.Heartbeat
				? LunyObjectEvent.Heartbeat
				: LunyObjectEvent.FrameUpdate;

			script.Scheduler.ScheduleObjectEventSequence(block, objectEvent);
			script.MarkBuilderTokenFinished(token);
			return block;
		}

		private static void ThrowIfAllSequencesEmpty(Script script, BuilderToken token, in CoroutineOptions options)
		{
			if (options.OnProcess == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
				throw new LunyScriptException($"{token.Type} '{options.Name}' has no blocks. Add blocks or remove coroutine. Script: {script}");
		}
	}
}
