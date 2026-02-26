using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Events;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenApi
	{
		private readonly Script _script;
		internal WhenApi(Script script) => _script = script;

		public InputApi Input => new(_script);
		public SceneApi Scene => new(_script);

		/// <summary>
		/// Scene Events
		/// </summary>
		public readonly struct SceneApi
		{
			private readonly Script _script;
			internal SceneApi(Script script) => _script = script;
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

		public readonly struct InputApi
		{
			private readonly Script _script;
			internal InputApi(Script script) => _script = script;
			private ScriptEventScheduler Scheduler => _script.Scheduler;
		}
	}
}
