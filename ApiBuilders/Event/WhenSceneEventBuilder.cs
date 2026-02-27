using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Events;
using System;

namespace LunyScript.ApiBuilders.Event
{
	/// <summary>
	/// Scene Events
	/// </summary>
	public readonly struct WhenSceneEventBuilder
	{
		private readonly Script _script;
		internal WhenSceneEventBuilder(Script script) => _script = script;
		private ScriptEventScheduler Scheduler => _script.Scheduler;

		/// <summary>
		/// Runs when a scene has loaded.
		/// </summary>
		/// <param name="blocks"></param>
		/// <returns></returns>
		public ISequenceBlock Loads(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSceneEventSequence(blocks, LunySceneEvent.OnSceneLoaded);

		/// <summary>
		/// Runs when a scene has loaded.
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="blocks"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public ISequenceBlock Loads(String sceneName, params ScriptActionBlock[] blocks) => throw new NotImplementedException(nameof(Loads));

		/// <summary>
		/// Runs when a scene has unloaded.
		/// </summary>
		/// <param name="blocks"></param>
		/// <returns></returns>
		public ISequenceBlock Unloads(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSceneEventSequence(blocks, LunySceneEvent.OnSceneUnloaded);

		/// <summary>
		/// Runs when a scene has unloaded.
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="blocks"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public ISequenceBlock Unloads(String sceneName, params ScriptActionBlock[] blocks) =>
			throw new NotImplementedException(nameof(Unloads));
	}
}
