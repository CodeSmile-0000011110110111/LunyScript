using Luny;
using System;
using StackFrame = System.Diagnostics.StackFrame;

namespace LunyScript
{
	public class Trace
	{
		internal static StackTrace TryCreateStackTrace(String apiName) =>
			true ? new StackTrace(apiName, new StackFrame(2, true)) : null;
	}
}
