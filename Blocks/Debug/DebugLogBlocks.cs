using Luny;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only logging block base class.
	/// </summary>
	internal class DebugLogBlock : ActionBlock
	{
		private DebugLogBlock() {}

		protected DebugLogBlock(String message, LogLevel logLevel, LunyStackTrace trace)
			: base(trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			_message = message;
			_logLevel = logLevel;
#endif
		}

		protected DebugLogBlock(VariableBlock variableBlock, LogLevel logLevel, LunyStackTrace trace)
			: base(trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			_variableBlock = variableBlock;
			_logLevel = logLevel;
#endif
		}

		protected internal override void Execute(IScriptRuntimeContext context) => DoLog(context);

		[DebuggerHidden] [Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoLog(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_variableBlock is not null)
				_message = _variableBlock.ToString();

			switch (_logLevel)
			{
				case LogLevel.Info:
					LunyLogger.LogInfo(_message, runtimeContext.LunyGameObject.Name);
					break;
				case LogLevel.Warning:
					LunyLogger.LogWarning(_message, runtimeContext.LunyGameObject.Name);
					break;
				case LogLevel.Error:
					LunyLogger.LogError(_message, runtimeContext.LunyGameObject.Name);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_logLevel), _logLevel, runtimeContext?.ToString());
			}
#endif
		}

#if DEBUG || LUNYSCRIPT_DEBUG
		public override String ToString() => _variableBlock is not null ? $"{_variableBlock}={_variableBlock.Variable}" : $"\"{_message}\"";

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
		public static ActionBlock Create(String message, LunyStackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(message, trace);
#else
			return null;
#endif
		}

		public static ActionBlock Create(String[] messages, LunyStackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(String.Join(", ", messages), trace);
#else
			return null;
#endif
		}

		public static ActionBlock Create(VariableBlock variable, LunyStackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(variable, trace);
#else
			return null;
#endif
		}

		private DebugLogInfoBlock(String message, LunyStackTrace trace)
			: base(message, LogLevel.Info, trace) {}

		private DebugLogInfoBlock(VariableBlock variableBlock, LunyStackTrace trace)
			: base(variableBlock, LogLevel.Info, trace) {}
	}

	/// <summary>
	/// Debug-only logging block for "warning" messages (yellow text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogWarningBlock : DebugLogBlock
	{
		public static ActionBlock Create(String message, LunyStackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogWarningBlock(message, trace);
#else
			return null;
#endif
		}

		private DebugLogWarningBlock(String message, LunyStackTrace trace)
			: base(message, LogLevel.Warning, trace) {}
	}

	/// <summary>
	/// Debug-only logging block for "error" messages (red text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogErrorBlock : DebugLogBlock
	{
		public static ActionBlock Create(String message, LunyStackTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogErrorBlock(message, trace);
#else
			return null;
#endif
		}

		private DebugLogErrorBlock(String message, LunyStackTrace trace)
			: base(message, LogLevel.Error, trace) {}
	}
}
