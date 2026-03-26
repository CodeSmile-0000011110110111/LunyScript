using Luny;
using Luny.Engine.Services;
using LunyScript.Api;
using LunyScript.Coroutines;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Represents a coroutine block that runs perpetually (indefinitely).
	/// Coroutines can be started, stopped, paused, resumed.
	/// </summary>
	public interface ICoroutineBlock : IScriptBlock
	{
		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		ActionBlock Start();

		/// <summary>
		/// Stops the coroutine and resets its state.
		/// </summary>
		ActionBlock Stop();

		/// <summary>
		/// Pauses the coroutine, preserving current state.
		/// </summary>
		ActionBlock Pause();

		/// <summary>
		/// Resumes a paused coroutine.
		/// </summary>
		ActionBlock Resume();
	}

	/// <summary>
	/// Represents a coroutine timer block. Timers fire after a duration elapses.
	/// </summary>
	public interface ITimerCoroutineBlock : ICoroutineBlock
	{
		/// <summary>
		/// Sets the time scale. Negative values are clamped to 0.
		/// </summary>
		ActionBlock TimeScale(Double scale);
	}

	/// <summary>
	/// Represents a coroutine counter block. Counters elapse after a specific number of frames/heartbeats have passed.
	/// </summary>
	public interface ICounterCoroutineBlock : ICoroutineBlock {}

	/// <summary>
	/// Wraps a Coroutine as a schedulable sequence block.
	/// Owns the coroutine's sequences, advances the coroutine each frame/heartbeat via Execute(),
	/// and exposes its sequences to diagnostics via IBlockContainer.
	/// </summary>
	internal class CoroutineBlock : ActionBlock, ICoroutineBlock, IBlockContainer, ISequenceBlock
	{
		private const Int32 SeqOnStarted = 0;
		private const Int32 SeqOnResumed = 1;
		private const Int32 SeqOnProcess = 2;
		private const Int32 SeqOnPaused  = 3;
		private const Int32 SeqOnStopped = 4;
		private const Int32 SeqOnElapsed = 5;

		protected readonly Coroutine _coroutine;
		private readonly SequenceBlock[] _sequences;
		private readonly ILunyTimeService _time;
		private readonly Int32 _timeSliceInterval;
		private readonly Int32 _timeSliceOffset;
		private readonly Coroutine.UpdateMode _updateMode;

		// ── ISequenceBlock ────────────────────────────────────────────────
		public ScriptBlockId Id { get; }
		IReadOnlyList<ActionBlock> ISequenceBlock.Blocks => Array.Empty<ActionBlock>();
		Boolean ISequenceBlock.IsEmpty => false;

		// ── IBlockContainer ───────────────────────────────────────────────
		Int32 IBlockContainer.ActionSequenceCount => 6;

		String IBlockContainer.GetActionSequenceName(Int32 index) => index switch
		{
			SeqOnStarted => "OnStarted",
			SeqOnResumed => "OnResumed",
			SeqOnProcess => "OnProcess",
			SeqOnPaused  => "OnPaused",
			SeqOnStopped => "OnStopped",
			SeqOnElapsed => "OnElapsed",
			_ => String.Empty,
		};

		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) =>
			index >= 0 && index < _sequences.Length ? _sequences[index]?.Blocks : null;

		// ── Internal accessors ────────────────────────────────────────────
		internal Coroutine Coroutine => _coroutine;

		// ── Factory ───────────────────────────────────────────────────────
		internal static CoroutineBlock Create(in CoroutineOptions options)
		{
			var coroutine = Coroutine.Create(options);
			return coroutine switch
			{
				TimerCoroutine timer => new TimerCoroutineBlock(timer, options),
				CounterCoroutine counter => new CounterCoroutineBlock(counter, options),
				var _ => new CoroutineBlock(coroutine, options),
			};
		}

		protected CoroutineBlock(Coroutine coroutine, in CoroutineOptions options)
		{
			_coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));
			Id = ScriptBlockId.Generate();
			_updateMode = options.ProcessMode;
			_timeSliceInterval = options.TimeSliceInterval;
			_timeSliceOffset = options.TimeSliceOffset;
			_time = options.TimeSliceInterval > 0 ? LunyEngine.Instance.Time : null;

			_sequences = new SequenceBlock[6];
			_sequences[SeqOnStarted] = SequenceBlock.TryCreate(options.OnStarted);
			_sequences[SeqOnResumed] = SequenceBlock.TryCreate(options.OnResumed);
			_sequences[SeqOnProcess] = SequenceBlock.TryCreate(options.OnProcess);
			_sequences[SeqOnPaused]  = SequenceBlock.TryCreate(options.OnPaused);
			_sequences[SeqOnStopped] = SequenceBlock.TryCreate(options.OnStopped);
			_sequences[SeqOnElapsed] = SequenceBlock.TryCreate(options.OnElapsed);
		}

		// ── ICoroutineBlock ───────────────────────────────────────────────
		public ActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public ActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public ActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public ActionBlock Resume() => new CoroutineResumeBlock(_coroutine);

		// ── Execute ───────────────────────────────────────────────────────
		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (_time != null)
			{
				var count = _updateMode == Coroutine.UpdateMode.Heartbeat
					? _time.HeartbeatCount : _time.FrameCount;
				if ((count - _timeSliceOffset) % _timeSliceInterval != 0)
					return;
			}

			var events = _coroutine.Process();
			if (events == Coroutine.Events.None)
				return;

			var context = (ScriptRuntimeContext)runtimeContext;
			if (events.Has(Coroutine.Events.Started)) LunyScriptRunner.Run(_sequences[SeqOnStarted], context);
			if (events.Has(Coroutine.Events.Resumed)) LunyScriptRunner.Run(_sequences[SeqOnResumed], context);
			if (events.Has(Coroutine.Events.Process)) LunyScriptRunner.Run(_sequences[SeqOnProcess], context);
			if (events.Has(Coroutine.Events.Elapsed)) LunyScriptRunner.Run(_sequences[SeqOnElapsed], context);
			if (events.Has(Coroutine.Events.Paused))  LunyScriptRunner.Run(_sequences[SeqOnPaused],  context);
			if (events.Has(Coroutine.Events.Stopped)) LunyScriptRunner.Run(_sequences[SeqOnStopped], context);
		}
	}

	internal sealed class TimerCoroutineBlock : CoroutineBlock, ITimerCoroutineBlock
	{
		internal TimerCoroutineBlock(TimerCoroutine coroutine, in CoroutineOptions options)
			: base(coroutine, options) {}

		public ActionBlock TimeScale(Double scale) => new TimerCoroutineSetTimeScaleBlock((TimerCoroutine)_coroutine, scale);
	}

	internal sealed class CounterCoroutineBlock : CoroutineBlock, ICounterCoroutineBlock
	{
		internal CounterCoroutineBlock(CounterCoroutine coroutine, in CoroutineOptions options)
			: base(coroutine, options) {}
	}
}
