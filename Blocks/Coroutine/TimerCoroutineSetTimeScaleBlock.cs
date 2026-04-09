using Luny;
using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks
{
	internal sealed class TimerCoroutineSetTimeScaleBlock : CoroutineControlBlock
	{
		private readonly Double _timeScale;

		public TimerCoroutineSetTimeScaleBlock(TimerCoroutine coroutine, Double timeScale, LunyStackTrace trace)
			: base(coroutine, trace) => _timeScale = coroutine.TimeScale = timeScale;

		protected internal override void Execute(IScriptRuntimeContext context) => ((TimerCoroutine)_coroutine).TimeScale = _timeScale;
	}
}
