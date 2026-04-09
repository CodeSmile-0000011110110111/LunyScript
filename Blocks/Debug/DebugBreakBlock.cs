using Luny;
using System;
using System.Diagnostics;
using StackTrace = Luny.StackTrace;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that triggers a breakpoint when hit.
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	internal sealed class DebugBreakBlock : ActionBlock
	{
		private readonly String _message;

		public static ActionBlock Create(String message, StackTrace trace) => new DebugBreakBlock(message, trace);

		private DebugBreakBlock(String message, StackTrace trace)
			: base(trace) => _message = message;

		protected internal override void Execute(IScriptRuntimeContext context) => DoBreak(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoBreak(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(DebugBreakBlock)}: {_message}", runtimeContext.LunyObject);

			Debugger.Break();
#endif
		}

		public override String ToString() => $"{nameof(DebugBreakBlock)}({_message ?? String.Empty})";
	}
}
