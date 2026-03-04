using LunyScript.Blocks;
using LunyScript.Events;
using LunyScript.Exceptions;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public readonly struct WhenInputActionBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly InputActionOptions _options;

		private ScriptEventScheduler Scheduler => _script.Scheduler;

		internal WhenInputActionBuilder(Script script, String actionName, [CallerMemberName] String callerName = "")
		{
			if (String.IsNullOrWhiteSpace(actionName))
				throw new LunyScriptException($"{script.GetType().Name}: When.{callerName}({nameof(actionName)}) cannot be null or empty");

			_script = script;
			_token = _script.CreateBuilderToken(actionName, "When.InputAction");
			_options = new InputActionOptions { ActionName = actionName };
		}

		public void Started(params ScriptActionBlock[] startedBlocks)
		{
			throw new NotImplementedException(nameof(Started));
		}
		public void Performed(params ScriptActionBlock[] startedBlocks)
		{
			throw new NotImplementedException(nameof(Started));
		}
		public void Canceled(params ScriptActionBlock[] startedBlocks)
		{
			throw new NotImplementedException(nameof(Started));
		}

		public void Performing(params ScriptActionBlock[] blocks)
		{
			Scheduler.ScheduleInputActionEventSequence(_options.ActionName, blocks);
			_script.FinalizeBuilderToken(_token);
		}
	}

	internal struct InputActionOptions
	{
		public String ActionName;
	}
}
