using LunyScript.Blocks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public static class BuilderUtility
	{
		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		public static void ThrowIfUnaryMethodUsedAgain(Script script, ScriptActionBlock[] blocks, [CallerMemberName] String callerName = "")
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (!ScriptActionBlock.IsNullOrEmpty(blocks))
				throw new ArgumentException($"{callerName}() is used multiple times in script: {script}", nameof(blocks));
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		public static void ThrowIfUnaryMethodUsedAgain(Script script, Object[] array, [CallerMemberName] String callerName = "")
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (array != null && array.Length != 0)
				throw new ArgumentException($"{callerName}() is used multiple times in script: {script}", nameof(array));
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		public static void ThrowIfUnaryMethodUsedAgain(Script script, Int32? option, [CallerMemberName] String callerName = "")
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (option.HasValue)
				throw new ArgumentException($"{callerName}() is used multiple times in script: {script}", nameof(option));
#endif
		}
	}
}
