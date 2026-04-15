using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Scene Events
	/// </summary>
	public readonly struct WhenSceneEventBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal WhenSceneEventBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		private ScriptEventScheduler Scheduler => _script.Scheduler;

		/// <summary>
		/// Runs when a scene has loaded.
		/// </summary>
		/// <param name="blocks"></param>
		/// <returns></returns>
		public ISequenceBlock Loaded(params ActionBlock[] blocks) =>
			Scheduler?.ScheduleSceneEventSequence(blocks, LunySceneEvent.OnSceneLoaded, _trace.Add(nameof(Loaded)));

		/// <summary>
		/// Runs when a scene has loaded.
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="blocks"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public ISequenceBlock Loaded(String sceneName, params ActionBlock[] blocks) => throw new NotImplementedException(nameof(Loaded));

		/// <summary>
		/// Runs when a scene has unloaded.
		/// </summary>
		/// <param name="blocks"></param>
		/// <returns></returns>
		public ISequenceBlock Unloaded(params ActionBlock[] blocks) =>
			Scheduler?.ScheduleSceneEventSequence(blocks, LunySceneEvent.OnSceneUnloaded, _trace.Add(nameof(Unloaded)));

		/// <summary>
		/// Runs when a scene has unloaded.
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="blocks"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public ISequenceBlock Unloaded(String sceneName, params ActionBlock[] blocks) => throw new NotImplementedException(nameof(Unloaded));
	}
}
