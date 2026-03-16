using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Api
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
			var userName = options.UserName;
			var scheduler = options.Script.Scheduler;

			var started = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Started, options.StartedBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Started, started);
			var performed = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Performed, options.PerformedBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Performed, performed);
			var performing = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Performing, options.PerformingBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Performing, performing);
			var canceled = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Canceled, options.CanceledBlocks);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Canceled, canceled);

			options.Script.MarkBuilderTokenFinished(options.Token);
		}
	}

	public static class WhenInputActionBuilderExtensions
	{
		public static WhenInputActionBuilder For(this WhenInputActionBuilder b, String userName)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.UserName);
			return new WhenInputActionBuilder(b.Options with { UserName = userName });
		}

		public static WhenInputActionBuilder Begins(this WhenInputActionBuilder b, params ActionBlock[] startedBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.StartedBlocks);

			var options = b.Options with { StartedBlocks = startedBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}

		public static WhenInputActionBuilder Changes(this WhenInputActionBuilder b, params ActionBlock[] performedBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.PerformedBlocks);

			var options = b.Options with { PerformedBlocks = performedBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}

		public static WhenInputActionBuilder Continues(this WhenInputActionBuilder b, params ActionBlock[] performingBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.PerformingBlocks);

			var options = b.Options with { PerformingBlocks = performingBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}

		public static WhenInputActionBuilder Ends(this WhenInputActionBuilder b, params ActionBlock[] canceledBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.CanceledBlocks);

			var options = b.Options with { CanceledBlocks = canceledBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}
	}

	internal record InputActionOptions
	{
		public Script Script;
		public BuilderToken Token;
		public String ActionName;
		public String UserName;

		public ActionBlock[] StartedBlocks;
		public ActionBlock[] PerformedBlocks;
		public ActionBlock[] PerformingBlocks;
		public ActionBlock[] CanceledBlocks;
	}
}
