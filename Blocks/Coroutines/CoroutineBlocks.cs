using LunyScript.Coroutines;
using System;

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
		ScriptActionBlock Start();

		/// <summary>
		/// Stops the coroutine and resets its state.
		/// </summary>
		ScriptActionBlock Stop();

		/// <summary>
		/// Pauses the coroutine, preserving current state.
		/// </summary>
		ScriptActionBlock Pause();

		/// <summary>
		/// Resumes a paused coroutine.
		/// </summary>
		ScriptActionBlock Resume();
	}

	/// <summary>
	/// Represents a coroutine timer block. Timers fire after a duration elapses.
	/// </summary>
	public interface ITimerCoroutineBlock : ICoroutineBlock
	{
		/// <summary>
		/// Sets the time scale. Values >= 0; negative values are clamped to 0.
		/// </summary>
		ScriptActionBlock TimeScale(Double scale);
	}

	/// <summary>
	/// Represents a coroutine counter block. Counters elapse after a specific number of frames/heartbeats have passed.
	/// </summary>
	public interface ICounterCoroutineBlock : ICoroutineBlock {}

	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal class CoroutineBlock : ScriptActionBlock, ICoroutineBlock
	{
		protected readonly Coroutine _coroutine;

		internal static CoroutineBlock Create(Coroutine coroutine) => coroutine switch
		{
			TimerCoroutine timer => new TimerCoroutineBlock(timer),
			CounterCoroutine counter => new CounterCoroutineBlock(counter),
			var _ => new CoroutineBlock(coroutine),
		};

		protected CoroutineBlock(Coroutine coroutine) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public ScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public ScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public ScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public ScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} cannot be used in a block sequence");
	}

	internal sealed class TimerCoroutineBlock : CoroutineBlock, ITimerCoroutineBlock
	{
		internal TimerCoroutineBlock(TimerCoroutine coroutine)
			: base(coroutine) {}

		public ScriptActionBlock TimeScale(Double scale) => new TimerCoroutineSetTimeScaleBlock((TimerCoroutine)_coroutine, scale);
	}

	internal sealed class CounterCoroutineBlock : CoroutineBlock, ICounterCoroutineBlock
	{
		internal CounterCoroutineBlock(CounterCoroutine coroutine)
			: base(coroutine) {}
	}
}
