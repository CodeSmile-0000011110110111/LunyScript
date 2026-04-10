using Luny;
using System;
using StackFrame = System.Diagnostics.StackFrame;

namespace LunyScript
{
	public class ScriptTrace
	{
		internal static LunyStackTrace TryCreateStackTrace(String apiName = null) => TryCreateStackTrace(apiName, 3);

		internal static LunyStackTrace TryCreateStackTrace(String apiName, Int32 skipFrames = 1) =>
			true ? new LunyStackTrace(apiName, new StackFrame(skipFrames, true)) : null;
	}
}
