using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Provides Editor-only functionality.
	/// In builds these blocks are ignored (no-op).
	/// </summary>
	public readonly struct EditorApi
	{
		private readonly Script _script;
		internal EditorApi(Script script) => _script = script;

		/// <summary>
		/// Pauses playmode.
		/// </summary>
		public ScriptActionBlock PausePlayer(String message = null) => EditorPausePlayerBlock.Create(message);
	}
}
