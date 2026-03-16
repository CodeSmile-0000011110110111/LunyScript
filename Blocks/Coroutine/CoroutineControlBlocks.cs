using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks
{
	internal abstract class CoroutineControlBlock : ActionBlock
	{
		protected readonly Coroutine _coroutine;

		protected CoroutineControlBlock(Coroutine coroutine) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlock
	{
		public CoroutineStartBlock(Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Start();
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlock
	{
		public CoroutineStopBlock(Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Stop();
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlock
	{
		public CoroutinePauseBlock(Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Pause();
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlock
	{
		public CoroutineResumeBlock(Coroutine coroutine)
			: base(coroutine) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Resume();
	}
}
