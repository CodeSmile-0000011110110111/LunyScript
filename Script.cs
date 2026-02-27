using Luny;
using LunyScript.Blocks;
using LunyScript.Events;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript
{
	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement Build() to construct their script logic.
	/// </summary>
	/// <remarks>
	/// Example script template (duplicate LunyScript.LunyScript is correct):
	///
	///		public class ExampleLunyScript : LunyScript.LunyScript
	///		{
	///			public override void Build()
	///			{
	///				// define behaviour using LunyScript API here ...
	///				OnUpdate(Debug.Log("Hello, LunyScript!"));
	///			}
	///		}
	/// </remarks>
	public abstract partial class Script
	{
		private IScriptRuntimeContext _runtimeContext;
		private List<BuilderToken> _pendingBuilders;
		private HashSet<BuilderToken> _finalizedBuilders;
		private VarAccessor _globalVariables;
		private VarAccessor _instanceVariables;

		internal ScriptRuntimeContext RuntimeContext => _runtimeContext as ScriptRuntimeContext;
		internal ScriptEventScheduler Scheduler => RuntimeContext.Scheduler;

		internal void Initialize(IScriptRuntimeContext runtimeContext)
		{
			_runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
			_globalVariables = new VarAccessor(_runtimeContext.GlobalVariables);
			_instanceVariables = new VarAccessor(_runtimeContext.LocalVariables);
		}

		~Script() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown()
		{
			FinalizePendingBuilderTokens();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		/// <param name="context"></param>
		public abstract void Build(ScriptContext context);

		internal BuilderToken CreateBuilderToken(String name, String type)
		{
			var frame = new StackFrame(3, true);
			var token = new BuilderToken(name, type, frame.GetFileName(), frame.GetFileLineNumber());

			_pendingBuilders ??= new List<BuilderToken>();
			_pendingBuilders.Add(token);
			return token;
		}

		internal void FinalizeBuilderToken(BuilderToken token)
		{
			token?.MarkFinished();

			_finalizedBuilders ??= new HashSet<BuilderToken>();
			_finalizedBuilders.Add(token);
		}

		private void FinalizePendingBuilderTokens()
		{
			if (_pendingBuilders == null || _pendingBuilders.Count == 0)
				return;

			var unfinishedBuilders = new List<BuilderToken>();
			for (var i = 0; i < _pendingBuilders.Count; i++)
			{
				var token = _pendingBuilders[i];
				if (_finalizedBuilders != null && _finalizedBuilders.Contains(token))
					continue;

				if (!token.FinalizeBuilder())
					unfinishedBuilders.Add(token);
			}

			_pendingBuilders.Clear();
			_finalizedBuilders.Clear();

			foreach (var token in unfinishedBuilders)
			{
				BuilderToken.LogUnfinishedBuilder(token);
				token.MarkFinished();
			}

			if (unfinishedBuilders.Count > 0)
				throw new LunyScriptException($"{GetType().Name} script has unfinished Block Builder(s): see warning message(s) above.");
		}

		public override String ToString() => _runtimeContext != null ? _runtimeContext.ToString() : GetType().FullName;
	}
}
