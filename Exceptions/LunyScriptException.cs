using Luny;
using System;

namespace LunyScript
{
	public class LunyScriptException : LunyException
	{
		public LunyScriptException(String message)
			: base(message) {}

		public LunyScriptException(String message, Exception innerException)
			: base(message, innerException) {}
	}
}
