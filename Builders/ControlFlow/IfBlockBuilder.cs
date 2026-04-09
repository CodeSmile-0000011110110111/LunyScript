using Luny;
using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Builder for constructing 'If' blocks with 'ElseIf' and 'Else' branches.
	/// </summary>
	public readonly struct IfBlockBuilder
	{
		private readonly IfBlockOptions _options;

		internal IfBlockBuilder(Script script, ConditionBlock[] conditions, LunyStackTrace trace)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("If() conditions cannot be null or empty");

			_options = new IfBlockOptions { Script = script, Token = script.CreateBuilderToken(nameof(IfBlock), "If"), Trace = trace };
			_options.BranchesBuilder.Add((conditions, Array.Empty<ActionBlock>()));

			var capturedOptions = _options;
			_options.Token.AutoFinish = () => BuildIfNeeded(capturedOptions);
		}

		public IfBlockBuilder Then(params ActionBlock[] actions)
		{
			if (actions == null || actions.Length == 0)
				throw new LunyScriptException("Then() blocks cannot be null or empty");

			var last = _options.BranchesBuilder.Count - 1;
			_options.BranchesBuilder[last] = (_options.BranchesBuilder[last].conditions, actions);
			_options.Trace.Add(nameof(Then));
			return this;
		}

		public IfBlockBuilder ElseIf(params ConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("ElseIf() conditions cannot be null or empty");

			_options.BranchesBuilder.Add((conditions, Array.Empty<ActionBlock>()));
			_options.Trace.Add(nameof(ElseIf));
			return this;
		}

		public ActionBlock Else(params ActionBlock[] actions)
		{
			_options.ElseBlocks = actions?.Length > 0 ? actions : null;
			_options.Trace.Add(nameof(Else));

			Build(_options);
			return _options.Block;
		}

		public static implicit operator ActionBlock(IfBlockBuilder builder)
		{
			BuildIfNeeded(builder._options);
			return builder._options.Block;
		}

		private static void BuildIfNeeded(IfBlockOptions options)
		{
			if (options.Block != null)
				return;

			Build(options);
		}

		private static void Build(IfBlockOptions options)
		{
			if (options.BranchesBuilder.Count == 0)
				throw new LunyScriptException($"{nameof(IfBlock)} has no branches");

			options.Block = IfBlock.Create(options.BranchesBuilder.ToArray(), options.ElseBlocks, options.Trace);
			options.Script.MarkBuilderTokenFinished(options.Token);
		}
	}

	/// <summary>
	/// Options DTO for the If block builder.
	/// Holds mutable builder-phase state accumulated across chained calls.
	/// </summary>
	internal record IfBlockOptions
	{
		internal Script Script;
		internal BuilderToken Token;
		internal LunyStackTrace Trace;
		internal IfBlock Block;
		internal List<(ConditionBlock[] conditions, ActionBlock[] actions)> BranchesBuilder = new();
		internal ActionBlock[] ElseBlocks;
	}
}
