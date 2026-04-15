using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public readonly struct WhenInputActionBuilder
	{
		internal readonly InputActionOptions Options;

		internal WhenInputActionBuilder(Script script, BuilderToken token, String actionName, LunyStackTrace trace,
			[CallerMemberName] String callerName = "")
		{
			if (String.IsNullOrWhiteSpace(actionName))
				throw new LunyScriptException($"{script.GetType().Name}: When.{callerName}({nameof(actionName)}) cannot be null or empty");

			Options = new InputActionOptions { Script = script, Token = token, Trace = trace, ActionName = actionName };
		}

		internal WhenInputActionBuilder(in InputActionOptions options) => Options = options;

		internal void Finish(in InputActionOptions options)
		{
			var actionName = options.ActionName;
			var userName = options.UserName;
			var scheduler = options.Script.Scheduler;

			var started = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Started, options.StartedBlocks,
				options.Trace);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Started, started);
			var performed = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Performed, options.PerformedBlocks,
				options.Trace);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Performed, performed);
			var performing = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Performing, options.ContinuingBlocks,
				options.Trace);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Performing, performing);
			var canceled = InputEventSequenceBlock.Create(actionName, userName, LunyInputActionPhase.Canceled, options.EndedBlocks,
				options.Trace);
			scheduler.ScheduleInputActionEventSequence(actionName, LunyInputActionPhase.Canceled, canceled);

			options.Script.MarkBuilderTokenFinished(options.Token);
		}
	}

	public static class WhenInputActionBuilderExtensions
	{
		/// <summary>
		/// Specifies that event should only run when sent from a device paired with the named input user.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static WhenInputActionBuilder ForUser(this WhenInputActionBuilder b, String userName)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.UserName);
			return new WhenInputActionBuilder(b.Options with { UserName = userName });
		}

		/// <summary>
		/// Runs when the input action has started processing. All input actions run Start event.
		/// For unconditional actions it is the same as Performed event.
		/// </summary>
		/// <remarks>
		/// For "hold" or "slow tap" interactions the Started event runs with the beginning of the hold or tap.
		/// The Performed even will be delayed accordingly.
		/// </remarks>
		/// <param name="b"></param>
		/// <param name="startedBlocks"></param>
		/// <returns></returns>
		public static WhenInputActionBuilder Started(this WhenInputActionBuilder b, params ActionBlock[] startedBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.StartedBlocks);

			var options = b.Options with { StartedBlocks = startedBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}

		/// <summary>
		/// Runs when the input action is performed. When interactions are used, it runs when the interactions (hold, slow tap) are satisfied.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="performedBlocks"></param>
		/// <returns></returns>
		public static WhenInputActionBuilder Performed(this WhenInputActionBuilder b, params ActionBlock[] performedBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.PerformedBlocks);

			var options = b.Options with { PerformedBlocks = performedBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}

		/// <summary>
		/// Runs repeatedly every frame, before frame processing begins (Heartbeat, FixedUpdate), until the input action ended.
		/// Use this for continuous processing of axis and analog input values, or button holds.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="performingBlocks"></param>
		/// <returns></returns>
		public static WhenInputActionBuilder Continuing(this WhenInputActionBuilder b, params ActionBlock[] performingBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.ContinuingBlocks);

			var options = b.Options with { ContinuingBlocks = performingBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}

		/// <summary>
		/// Runs when an input action ended, even when it merely Started but never Performed.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="canceledBlocks"></param>
		/// <returns></returns>
		public static WhenInputActionBuilder Ended(this WhenInputActionBuilder b, params ActionBlock[] canceledBlocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.EndedBlocks);

			var options = b.Options with { EndedBlocks = canceledBlocks };
			b.Options.Token.AutoFinish = () => b.Finish(options);
			return new WhenInputActionBuilder(options);
		}
	}

	internal record InputActionOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;
		public String ActionName;
		public String UserName;

		public ActionBlock[] StartedBlocks;
		public ActionBlock[] PerformedBlocks;
		public ActionBlock[] ContinuingBlocks;
		public ActionBlock[] EndedBlocks;
	}
}
