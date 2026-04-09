using Luny;
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
		private readonly StackTrace _trace;

		internal EditorApi(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary>
		/// Pauses playmode.
		/// </summary>
		public ActionBlock PausePlayer(String message = null) => EditorPausePlayerBlock.Create(message);
	}
}
