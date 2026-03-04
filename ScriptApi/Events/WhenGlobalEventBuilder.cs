using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Events;
using System;

namespace LunyScript
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenGlobalEventBuilder
	{
		private readonly Script _script;
		internal WhenGlobalEventBuilder(Script script) => _script = script;

		// public WhenInputEventBuilder Input => new(_script);
		public WhenInputActionBuilder InputAction(String actionName) => new(_script, actionName);
		public WhenSceneEventBuilder Scene => new(_script);
	}

	/*
	public readonly struct WhenInputEventBuilder
	{
		private readonly Script _script;
		internal WhenInputEventBuilder(Script script) => _script = script;
		private ScriptEventScheduler Scheduler => _script.Scheduler;
	}
	*/

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
		public ISequenceBlock Loads(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSceneEventSequence(blocks, LunySceneEvent.OnSceneLoaded);

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
