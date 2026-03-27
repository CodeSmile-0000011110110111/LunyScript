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
				var sb = new StringBuilder();
				var trace = _block.Trace;
				if (trace != null && trace.Count > 1)
				{
					for (var i = 0; i < trace.Count; i++)
					{
						if (i > 0)
							sb.Append('.');

						sb.Append(trace[i].Name);
					}

					sb.Append('(');
					sb.Append(_block);
					sb.Append(')');

					sb.Append(" (at ");
					sb.Append(trace[0].Filename);
					sb.Append(':');
					sb.Append(trace[0].Line);
					sb.Append(')');
				}
				else
					sb.Append(_block);

				return sb.ToString();
			}
		}
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
