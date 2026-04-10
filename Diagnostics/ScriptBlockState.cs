using Luny;
using LunyScript.Blocks;
using System;
using System.Text;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Pairs a ScriptBlock with diagnostics-only state for use in the Script Blocks window.
	/// </summary>
	internal sealed class ScriptBlockState
	{
		private readonly ScriptBlock _block;

		public Int32 FrameStamp { get; set; }

		public String FileName => _block.Trace?.Count > 0 ? _block.Trace[0].FileName : null;
		public Int32 Line => _block.Trace?.Count > 0 ? _block.Trace[0].Line : 0;
		public ScriptBlock Block => _block;

		// public Boolean IsAction => _block is ActionBlock;
		// public Boolean IsCondition => _block is ConditionBlock;
		// public ActionBlock Action => _block as ActionBlock;
		// public ConditionBlock Condition => _block as ConditionBlock;

		public static String GetTruthSymbol(ScriptRuntimeContext context, ConditionBlock condition) =>
			context != null ? Emoji.IsSatisfied(condition.Evaluate(context)) : Emoji.IsKnown(false);

		public ScriptBlockState(ScriptBlock block) => _block = block;

		public String GetDisplayString(ScriptRuntimeContext context)
		{
			if (_block is VariableBlock variableBlock)
				return $"{GetTruthSymbol(context, variableBlock)}{_block}";

			if (_block is IBlockContainer blockContainer)
				return blockContainer.ToString();

			var trace = _block.Trace;
			if (trace != null && trace.Count > 0)
			{
				var sb = new StringBuilder();

				if (_block is ConditionBlock conditionBlock)
					sb.Append(GetTruthSymbol(context, conditionBlock));

				if (_block is not CheckBlock checkBlock || checkBlock.ToString() == Emoji.NotFound)
				{
					for (var i = 0; i < trace.Count; i++)
					{
						if (i > 0)
							sb.Append('.');

						sb.Append(trace[i].Name);
					}
				}

				sb.Append('(');
				sb.Append(_block);
				sb.Append(')');
				return sb.ToString();
			}

			return $"{_block?.GetType().Name}({_block}) <-- FIXME: no trace";
		}

		public Boolean Contains(String filterText) => GetDisplayString(null).IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;

		public override String ToString() => GetDisplayString(null);

		public String GetBranchLabel(ScriptRuntimeContext scriptContext, Int32 branchIndex)
		{
			if (scriptContext == null || branchIndex < 0 || _block is not IBlockContainer container)
				return null;

			var condCount = container.ConditionSequenceCount;
			if (branchIndex >= condCount)
				return null;

			var sequence = container.GetConditionSequence(branchIndex);
			if (sequence == null)
				return null;

			var truthValue = true;
			foreach (var block in sequence)
			{
				if (block is ConditionBlock condition && !condition.Evaluate(scriptContext))
				{
					truthValue = false;
					break;
				}
			}

			var branchName = container.GetConditionSequenceName(branchIndex);
			return $"{Emoji.IsSatisfied(truthValue)}{branchName}";
		}

		public String GetIfBlockElseLabel(ScriptRuntimeContext scriptContext)
		{
			if (_block is not IfBlock ifBlock)
				return null;

			var allConditionsNotSatisfied = true;
			var container = (IBlockContainer)ifBlock;
			var condCount = container.ConditionSequenceCount;
			for (var i = 0; i < condCount; i++)
			{
				var sequence = container.GetConditionSequence(i);
				var branchSatisfied = true;
				foreach (var block in sequence)
				{
					if (block is ConditionBlock condition && !condition.Evaluate(scriptContext))
					{
						branchSatisfied = false;
						break;
					}
				}

				if (branchSatisfied)
				{
					allConditionsNotSatisfied = false;
					break;
				}
			}

			return $"{Emoji.IsSatisfied(allConditionsNotSatisfied)}Else";
		}
	}
}
