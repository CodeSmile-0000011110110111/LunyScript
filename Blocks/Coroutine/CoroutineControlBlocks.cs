using Luny;
using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks
{
	internal abstract class CoroutineControlBlock : ActionBlock
	{
		protected readonly Coroutine _coroutine;

		protected CoroutineControlBlock(Coroutine coroutine, LunyStackTrace trace)
			: base(trace) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public override String ToString() => $"\"{_coroutine.Name}\"";
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlock
	{
		public CoroutineStartBlock(Coroutine coroutine, LunyStackTrace trace)
			: base(coroutine, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Start();
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlock
	{
		public CoroutineStopBlock(Coroutine coroutine, LunyStackTrace trace)
			: base(coroutine, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Stop();
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlock
	{
		public CoroutinePauseBlock(Coroutine coroutine, LunyStackTrace trace)
			: base(coroutine, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Pause();
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlock
	{
		public CoroutineResumeBlock(Coroutine coroutine, LunyStackTrace trace)
			: base(coroutine, trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Resume();
	}
}
