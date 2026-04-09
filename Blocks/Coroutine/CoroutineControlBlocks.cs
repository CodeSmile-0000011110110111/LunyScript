using System;

namespace LunyScript.Blocks
{
	internal abstract class CoroutineControlBlock : ActionBlock
	{
		protected readonly Coroutines.Coroutine _coroutine;

		protected CoroutineControlBlock(Coroutines.Coroutine coroutine) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlock
	{
		public CoroutineStartBlock(Coroutines.Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Start();
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlock
	{
		public CoroutineStopBlock(Coroutines.Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Stop();
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlock
	{
		public CoroutinePauseBlock(Coroutines.Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Pause();
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlock
	{
		public CoroutineResumeBlock(Coroutines.Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext context) => _coroutine.Resume();
	}
}
