using LunyScript.Blocks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public static class BuilderUtility
	{
		[Conditional("DEBUG")][Conditional("LUNYSCRIPT_DEBUG")]
		public static void ThrowIfUnaryMethodUsedAgain(Script script, ScriptActionBlock[] blocks, [CallerMemberName] String callerName = "")
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (!ScriptActionBlock.IsNullOrEmpty(blocks))
				throw new ArgumentException($"{callerName}() is used multiple times in script: {script}", nameof(blocks));
#endif
		}
	}
}
