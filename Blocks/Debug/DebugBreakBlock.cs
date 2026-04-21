using Luny;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that triggers a breakpoint when hit.
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	internal sealed class DebugBreakBlock : ActionBlock
	{
		private readonly String _message;

		public static ActionBlock Create(String message, LunyStackTrace trace) => new DebugBreakBlock(message, trace);

		private DebugBreakBlock(String message, LunyStackTrace trace)
			: base(trace) => _message = message;

		protected internal override void Execute(IScriptRuntimeContext context) => DoBreak(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoBreak(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(DebugBreakBlock)}: {_message}", runtimeContext.LunyGameObject);

			Debugger.Break();
#endif
		}

		public override String ToString() => $"{nameof(DebugBreakBlock)}({_message ?? String.Empty})";
	}
}
