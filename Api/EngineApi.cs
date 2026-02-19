using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	public readonly struct EngineApi
	{
		private readonly Script _script;
		internal EngineApi(Script script) => _script = script;

		/// <summary>
		/// Logs a message that appears in both debug and release builds.
		/// Posts to both Luny internal log (if enabled) and engine logging.
		/// </summary>
		public ScriptActionBlock Log(String message) => EngineLogBlock.Create(message);
	}
}
