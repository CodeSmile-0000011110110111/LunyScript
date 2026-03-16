using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Provides diagnostics blocks which are omitted from release builds,
	/// unless the scripting symbol LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	public readonly struct DebugApi
	{
		private readonly Script _script;
		internal DebugApi(Script script) => _script = script;

		/// <summary>
		/// Logs a variable. Alias for LogInfo().
		/// </summary>
		/// <param name="variableBlock"></param>
		/// <returns></returns>
		public ActionBlock Log(VariableBlock variableBlock) => LogInfo(variableBlock);

		/// <summary>
		/// Logs a message. Alias for LogInfo().
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ActionBlock Log(String message) => LogInfo(message);

		/// <summary>
		/// Logs a variable.
		/// </summary>
		/// <param name="variableBlock"></param>
		/// <returns></returns>
		public ActionBlock LogInfo(VariableBlock variableBlock) => DebugLogInfoBlock.Create(variableBlock);

		/// <summary>
		/// Logs a debug message that is completely stripped in release builds.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		public ActionBlock LogInfo(String message) => DebugLogInfoBlock.Create(message);

		/// <summary>
		/// Logs a debug "warning" (yellow text) message.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined, stripped in release builds.
		/// </summary>
		public ActionBlock LogWarning(String message) => DebugLogWarningBlock.Create(message);

		/// <summary>
		/// Logs a debug "error" (red text) message.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined, stripped in release builds.
		/// </summary>
		public ActionBlock LogError(String message) => DebugLogErrorBlock.Create(message);

		/// <summary>
		/// Triggers a debugger breakpoint if debugger is attached by calling System.Diagnostics.Debugger.Break().
		/// Completely stripped in release builds.
		/// Only breaks when DEBUG or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		public ActionBlock Break(String message = null) => DebugBreakBlock.Create(message);
	}
}
