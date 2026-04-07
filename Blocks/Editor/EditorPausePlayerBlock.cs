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

		public static ActionBlock Create(String message) =>
			LunyEngine.Instance.Application.IsEditor ? new EditorPausePlayerBlock(message) : null;

		private EditorPausePlayerBlock() {}
		private EditorPausePlayerBlock(String message = null) => _message = message;

		protected internal override void Execute(IScriptRuntimeContext context) => DoPausePlayer(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoPausePlayer(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(EditorPausePlayerBlock)}: {_message}", runtimeContext.LunyObject);

			LunyEngine.Instance.Editor.PausePlayer();
#endif
		}

		public override String ToString() => $"{nameof(EditorPausePlayerBlock)}({_message ?? String.Empty})";
	}
}
