using Luny;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public sealed class SequenceBlock : ActionBlock, ISequenceBlock
	{
		public ScriptBlockId Id { get; }
		public IReadOnlyList<ActionBlock> Blocks { get; }

		public static SequenceBlock TryCreate(IReadOnlyList<ActionBlock> blocks, StackTrace trace = null) =>
			blocks?.Count > 0 ? new SequenceBlock(blocks, trace) : null;

		private SequenceBlock(IReadOnlyList<ActionBlock> blocks, StackTrace trace)
			: base(trace)
		{
			Id = ScriptBlockId.Generate();
			Blocks = blocks;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				return;

			foreach (var block in Blocks)
				block?.Execute(runtimeContext);
		}

		public override String ToString()
		{
			var sb = new StringBuilder();
			var trace = Trace;
			if (trace != null && trace.Count > 0)
			{
				for (var i = 0; i < trace.Count; i++)
				{
					if (i > 0)
						sb.Append('.');

					sb.Append(trace[i].Name);
				}

				sb.Append('(');
				sb.Append(Blocks.Count);
				sb.Append(" blocks");
				sb.Append(')');

				sb.Append("    (");
				sb.Append(trace[0].Filename);
				sb.Append(':');
				sb.Append(trace[0].Line);
				sb.Append(')');
			}
			else
				sb.Append(base.ToString());

			return sb.ToString();
		}
	}
}
