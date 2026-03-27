using Luny;
using System;
using System.Diagnostics;
using StackTrace = Luny.StackTrace;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only logging block base class.
	/// </summary>
	internal class DebugLogBlock : ActionBlock
	{
		private DebugLogBlock() {}

		protected DebugLogBlock(String message, LogLevel logLevel, StackTrace trace)
			: base(trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			_message = message;
			_logLevel = logLevel;
#endif
		}

		protected DebugLogBlock(VariableBlock variableBlock, LogLevel logLevel, StackTrace trace)
			: base(trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			_variableBlock = variableBlock;
			_logLevel = logLevel;
#endif
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => DoLog(runtimeContext);

		[DebuggerHidden] [Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoLog(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_variableBlock is not null)
				_message = _variableBlock.ToString();

			switch (_logLevel)
			{
				case LogLevel.Info:
					LunyLogger.LogInfo(_message, runtimeContext.LunyObject.Name);
					break;
				case LogLevel.Warning:
					LunyLogger.LogWarning(_message, runtimeContext.LunyObject.Name);
					break;
				case LogLevel.Error:
					LunyLogger.LogError(_message, runtimeContext.LunyObject.Name);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_logLevel), _logLevel, runtimeContext?.ToString());
			}
#endif
		}

		public override String ToString() => _variableBlock is not null ? $"{_variableBlock}={_variableBlock.Variable}" : $"\"{_message}\"";

#if DEBUG || LUNYSCRIPT_DEBUG
		protected String _message;
		protected VariableBlock _variableBlock;
		private LogLevel _logLevel;
#endif
	}

	/// <summary>
	/// Debug-only logging block for "info" messages (gray/white text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogInfoBlock : DebugLogBlock
	{
		public static ActionBlock Create(String message, StackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(message, trace);
#else
			return null;
#endif
		}

		public static ActionBlock Create(String[] messages, StackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(String.Join(", ", messages), trace);
#else
			return null;
#endif
		}

		public static ActionBlock Create(VariableBlock variable, StackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(variable, trace);
#else
			return null;
#endif
		}

		private DebugLogInfoBlock(String message, StackTrace trace)
			: base(message, LogLevel.Info, trace) {}

		private DebugLogInfoBlock(VariableBlock variableBlock, StackTrace trace)
			: base(variableBlock, LogLevel.Info, trace) {}
	}

	/// <summary>
	/// Debug-only logging block for "warning" messages (yellow text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogWarningBlock : DebugLogBlock
	{
		public static ActionBlock Create(String message, StackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogWarningBlock(message, trace);
#else
			return null;
#endif
		}

		private DebugLogWarningBlock(String message, StackTrace trace)
			: base(message, LogLevel.Warning, trace) {}
	}

	/// <summary>
	/// Debug-only logging block for "error" messages (red text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogErrorBlock : DebugLogBlock
	{
		public static ActionBlock Create(String message, StackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogErrorBlock(message, trace);
#else
			return null;
#endif
		}

		private DebugLogErrorBlock(String message, StackTrace trace)
			: base(message, LogLevel.Error, trace) {}
	}
}
