using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Shared utilities for LunyScript builder pattern.
	/// </summary>
	internal static class BuilderUtility
	{
		public static ScriptActionBlock[] Append(ScriptActionBlock[] existing, ScriptActionBlock[] additional)
		{
			if (existing == null || existing.Length == 0)
				return additional;
			if (additional == null || additional.Length == 0)
				return existing;

			LunyLogger.LogWarning("Appending multiple Coroutine blocks due to use of two or more same-behaviour block methods. " +
			                      "Please review the Coroutine builder statements to avoid the array copy operations.");

			var result = new ScriptActionBlock[existing.Length + additional.Length];
			Array.Copy(existing, 0, result, 0, existing.Length);
			Array.Copy(additional, 0, result, existing.Length, additional.Length);
			return result;
		}
	}
}
