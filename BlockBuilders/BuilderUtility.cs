using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript.BlockBuilders
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

		public static ICoroutineBlock Finalize(Script script, in Coroutine.Options options, BuilderToken token)
		{
			if (options.OnFrameUpdate == null && options.OnHeartbeat == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
			{
				LunyLogger.LogWarning($"{nameof(Coroutine)} '{options.Name}' was finalized without any action blocks. " +
				                      "It will run but perform no actions.", script);
			}

			var scriptInternal = script;
			var block = scriptInternal.RuntimeContext.Coroutines.Register(in options);
			scriptInternal.FinalizeToken(token);
			return block;
		}


	}
}
