using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logs a message to the engine's logging system.
	/// </summary>
	internal sealed class EngineLogBlock : ScriptActionBlock
	{
		private readonly String _message;

		public static ScriptActionBlock Create(String message) => new EngineLogBlock(message);

		private EngineLogBlock(String message) => _message = message ?? throw new ArgumentNullException(nameof(message));

		protected internal  override void Execute(IScriptRuntimeContext runtimeContext) => LunyLogger.LogInfo(_message, this);
	}
}
