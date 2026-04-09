using Luny;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that represents a literal value.
	/// </summary>
	internal sealed class LiteralVariableBlock : VariableBlock
	{
		private readonly Luny.Variable _value;

		internal override Luny.Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _value;
		}

		public static LiteralVariableBlock Create(Luny.Variable value, StackTrace trace) => new(value, trace);

		private LiteralVariableBlock(Luny.Variable value, StackTrace trace)
			: base(trace) => _value = value;
	}
}
