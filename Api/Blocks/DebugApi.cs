using Luny;
using LunyScript.Blocks;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LunyScript.Api
{
	/// <summary>
	/// Provides diagnostics blocks which are omitted from release builds,
	/// unless the scripting symbol LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	public readonly struct DebugApi
	{
		private readonly Script _script;
		private readonly StackTrace _trace;

		internal DebugApi(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary>
		/// Logs a message. Alias for LogInfo().
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ActionBlock Log(String message)
		{
			_trace.Add(nameof(Log));
			return DebugLogInfoBlock.Create(message, _trace);
		}

		/// <summary>
		/// Logs a variable. Alias for LogInfo().
		/// </summary>
		/// <param name="variableBlock"></param>
		/// <returns></returns>
		public ActionBlock Log(VariableBlock variableBlock)
		{
			_trace.Add(nameof(Log));
			return DebugLogInfoBlock.Create(variableBlock, _trace);
		}

		/// <summary>
		/// Logs multiple comma-separated messages.
		/// </summary>
		/// <param name="messages"></param>
		/// <returns></returns>
		public ActionBlock Log(params String[] messages)
		{
			_trace.Add(nameof(Log));
			return DebugLogInfoBlock.Create(messages, _trace);
		}

		/// <summary>
		/// Logs a variable.
		/// </summary>
		/// <param name="variableBlock"></param>
		/// <returns></returns>
		public ActionBlock LogInfo(VariableBlock variableBlock)
		{
			_trace.Add(nameof(LogInfo));
			return DebugLogInfoBlock.Create(variableBlock, _trace);
		}

		/// <summary>
		/// Logs a debug message that is completely stripped in release builds.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		public ActionBlock LogInfo(String message)
		{
			_trace.Add(nameof(LogInfo));
			return DebugLogInfoBlock.Create(message, _trace);
		}

		private ActionBlock LogInfo(params String[] messages)
		{
			_trace.Add(nameof(LogInfo));
			return DebugLogInfoBlock.Create(messages, _trace);
		}

		/// <summary>
		/// Logs a debug "warning" (yellow text) message.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined, stripped in release builds.
		/// </summary>
		public ActionBlock LogWarning(String message)
		{
			_trace.Add(nameof(LogWarning));
			return DebugLogWarningBlock.Create(message, _trace);
		}

		/// <summary>
		/// Logs a debug "error" (red text) message.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined, stripped in release builds.
		/// </summary>
		public ActionBlock LogError(String message)
		{
			_trace.Add(nameof(LogError));
			return DebugLogErrorBlock.Create(message, _trace);
		}

		/// <summary>
		/// Triggers a debugger breakpoint if debugger is attached by calling System.Diagnostics.Debugger.Break().
		/// Completely stripped in release builds.
		/// Only breaks when DEBUG or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		public ActionBlock Break(String message = null) => DebugBreakBlock.Create(message);
	}
}
