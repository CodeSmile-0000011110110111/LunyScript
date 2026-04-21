using Luny;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that pauses the player (Editor-only).
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	internal sealed class EditorPausePlayerBlock : ActionBlock
	{
		private readonly String _message;

		public static ActionBlock Create(String message, LunyStackTrace trace) =>
			LunyEngine.Instance.Application.IsEditor ? new EditorPausePlayerBlock(message, trace) : null;

		private EditorPausePlayerBlock(String message, LunyStackTrace trace)
			: base(trace) => _message = message;

		protected internal override void Execute(IScriptRuntimeContext context) => DoPausePlayer(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoPausePlayer(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(EditorPausePlayerBlock)}: {_message}", runtimeContext.LunyGameObject);

			LunyEngine.Instance.Editor.PausePlayer();
#endif
		}

		public override String ToString() => $"{nameof(EditorPausePlayerBlock)}({_message ?? String.Empty})";
	}
}
