using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Exceptions;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public readonly struct WhenInputActionBuilder
	{
		internal readonly InputActionOptions Options;

		internal WhenInputActionBuilder(Script script, BuilderToken token, String actionName, [CallerMemberName] String callerName = "")
		{
			if (String.IsNullOrWhiteSpace(actionName))
				throw new LunyScriptException($"{script.GetType().Name}: When.{callerName}({nameof(actionName)}) cannot be null or empty");

			Options = new InputActionOptions { Script = script, Token = token, ActionName = actionName };
		}

		internal WhenInputActionBuilder(in InputActionOptions options) => Options = options;

		internal void Finish(in InputActionOptions options)
		{
			var actionName = options.ActionName;
			var scheduler = options.Script.Scheduler;
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Started, options.StartedBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Performed, options.PerformedBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Performing, options.PerformingBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Canceled, options.CanceledBlocks);
			options.Script.MarkBuilderTokenFinished(options.Token);
		}
	}

	public static class WhenInputActionBuilderExtensions
	{
		public static WhenInputActionBuilder Started(this WhenInputActionBuilder b, params ScriptActionBlock[] startedBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.StartedBlocks);

			var options = b.Options with { StartedBlocks = startedBlocks };
			b.Options.Token.SetAutoFinish(() => b.Finish(options));
			return new WhenInputActionBuilder(options);
		}

		public static WhenInputActionBuilder Performed(this WhenInputActionBuilder b, params ScriptActionBlock[] performedBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.PerformedBlocks);

			var options = b.Options with { PerformedBlocks = performedBlocks };
			b.Options.Token.SetAutoFinish(() => b.Finish(options));
			return new WhenInputActionBuilder(options);
		}

		public static WhenInputActionBuilder Performing(this WhenInputActionBuilder b, params ScriptActionBlock[] performingBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.PerformingBlocks);

			var options = b.Options with { PerformingBlocks = performingBlocks };
			b.Options.Token.SetAutoFinish(() => b.Finish(options));
			return new WhenInputActionBuilder(options);
		}

		public static WhenInputActionBuilder Canceled(this WhenInputActionBuilder b, params ScriptActionBlock[] canceledBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.CanceledBlocks);

			var options = b.Options with { CanceledBlocks = canceledBlocks };
			b.Options.Token.SetAutoFinish(() => b.Finish(options));
			return new WhenInputActionBuilder(options);
		}
	}

	internal record InputActionOptions
	{
		public Script Script;
		public BuilderToken Token;
		public String ActionName;

		public ScriptActionBlock[] StartedBlocks;
		public ScriptActionBlock[] PerformedBlocks;
		public ScriptActionBlock[] PerformingBlocks;
		public ScriptActionBlock[] CanceledBlocks;
	}
}
