using LunyScript.Blocks;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public static class BuilderUtility
	{
		public static void ThrowIfUnaryMethodUsedAgain(ScriptActionBlock[] blocks, [CallerMemberName] String callerName = "")
		{
			if (!ScriptActionBlock.IsNullOrEmpty(blocks))
				throw new ArgumentException($"{callerName}() is used multiple times", nameof(blocks));
		}
	}
}
