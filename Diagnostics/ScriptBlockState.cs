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
		public String DisplayString
		{
			get
			{
				var trace = _block.Trace;
				if (trace != null && trace.Count > 1)
				{
					var sb = new StringBuilder();
					for (var i = 0; i < trace.Count; i++)
					{
						if (i > 0)
							sb.Append('.');

						sb.Append(trace[i].Name);
					}

					sb.Append('(');
					sb.Append(_block);
					sb.Append(')');
					return sb.ToString();
				}

				return $"{_block?.GetType().Name}({_block}) <-- FIXME";
			}
		}
		public String FileName => _block.Trace?.Count > 1 ? _block.Trace[0].FileName : null;
		public int Line => _block.Trace?.Count > 1 ? _block.Trace[0].Line : 0;
		public Boolean IsAction => _block is ActionBlock;
		public Boolean IsCondition => _block is ConditionBlock;
		public ActionBlock Action => _block as ActionBlock;
		public ConditionBlock Condition => _block as ConditionBlock;
		public ScriptBlock Block => _block;

		public ScriptBlockState(ScriptBlock block) => _block = block;

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

		public Boolean Contains(String filterText) => DisplayString.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;

		public override String ToString() => DisplayString;
	}
}
