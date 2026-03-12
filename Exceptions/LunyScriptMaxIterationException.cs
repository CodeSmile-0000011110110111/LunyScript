using System;

namespace LunyScript
{
	/// <summary>
	/// Exception thrown when a script loop exceeds the maximum allowed iterations.
	/// </summary>
	public sealed class LunyScriptMaxIterationException : Exception
	{
		public LunyScriptMaxIterationException(IScriptRuntimeContext runtimeContext, String blockName, Int32 limit)
			: base($"Max loop iterations ({limit}) exceeded in {blockName}! ({runtimeContext})") {}
	}
}
