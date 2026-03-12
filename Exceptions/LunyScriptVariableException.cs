using System;

namespace LunyScript
{
	public sealed class LunyScriptVariableException : LunyScriptException
	{
		public LunyScriptVariableException(String message)
			: base(message) {}

		public LunyScriptVariableException(String message, Exception innerException)
			: base(message, innerException) {}
	}
}
