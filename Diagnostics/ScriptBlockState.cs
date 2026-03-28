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
		private static String s_SymbolTrue = "🟢";//"✅";
		private static String s_SymbolFalse = "🔴";//"❌";
		private static String s_SymbolUndecided = "🤔";
		private readonly ScriptBlock _block;

		public Int32 FrameStamp { get; set; }

		public String FileName => _block.Trace?.Count > 0 ? _block.Trace[0].FileName : null;
		public Int32 Line => _block.Trace?.Count > 0 ? _block.Trace[0].Line : 0;
		public Boolean IsAction => _block is ActionBlock;
		public Boolean IsCondition => _block is ConditionBlock;
		public ActionBlock Action => _block as ActionBlock;
		public ConditionBlock Condition => _block as ConditionBlock;
		public ScriptBlock Block => _block;

		public static String GetTruthSymbol(ScriptRuntimeContext context, ConditionBlock condition) =>
			context != null ? condition.Evaluate(context) ? s_SymbolTrue : s_SymbolFalse : s_SymbolUndecided;

		public ScriptBlockState(ScriptBlock block) => _block = block;

		public String GetDisplayString(ScriptRuntimeContext context)
		{
			if (_block is VariableBlock variableBlock)
				return $"{GetTruthSymbol(context, variableBlock)} {_block}";

			if (_block is IBlockContainer blockContainer)
				return blockContainer.ToString();

			var trace = _block.Trace;
			if (trace != null && trace.Count > 0)
			{
				var sb = new StringBuilder();
				for (var i = 0; i < trace.Count; i++)
				{
					if (i > 0)
						sb.Append('.');

					sb.Append(trace[i].Name);
				}

				if (_block is ConditionBlock conditionBlock)
				{
					sb.Append(GetTruthSymbol(context, conditionBlock));
					sb.Append(' ');
				}
				sb.Append('(');
				sb.Append(_block);
				sb.Append(')');
				return sb.ToString();
			}

			return $"{_block?.GetType().Name}({_block}) <-- FIXME: no trace";
		}

		public Boolean TryGetAction(out ActionBlock action)
		{
			action = Action;
			return IsAction;
		}

		public Boolean TryGetCondition(out ConditionBlock condition)
		{
			condition = Condition;
			return IsCondition;
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
			var truthSymbol = truthValue ? s_SymbolTrue : s_SymbolFalse;
			return $"{truthSymbol} {branchName}";
		}
	}
}
