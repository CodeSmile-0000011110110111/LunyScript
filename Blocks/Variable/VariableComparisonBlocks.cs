using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class VariableComparisonBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.ScalarVarHandle VarHandle => _left?.VarHandle ?? _right?.VarHandle;

		protected VariableComparisonBlock(VariableBlock left, VariableBlock right = null)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right;

			if (left.Variable.IsNull)
				throw new ArgumentException($"Variable {left} is uninitialized");
			if (right is not null && right.Variable.IsNull)
				throw new ArgumentException($"Variable {right} is uninitialized");
		}

		public override String ToString() => $"{GetType().Name}({_left}, {_right})";
	}

	internal sealed class VariableIsEqualToBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable == _right.Variable;
		}
		public static VariableIsEqualToBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsEqualToBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;
	}

	internal sealed class VariableIsNotEqualToBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable != _right.Variable;
		}
		public static VariableIsNotEqualToBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsNotEqualToBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;
	}

	internal sealed class VariableIsGreaterThanBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable > (Double)_right.Variable;
		}
		public static VariableIsGreaterThanBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsGreaterThanBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;
	}

	internal sealed class VariableIsAtLeastBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable >= (Double)_right.Variable;
		}
		public static VariableIsAtLeastBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsAtLeastBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;
	}

	internal sealed class VariableIsLessThanBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable < (Double)_right.Variable;
		}
		public static VariableIsLessThanBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsLessThanBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;
	}

	internal sealed class VariableIsAtMostBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable <= (Double)_right.Variable;
		}
		public static VariableIsAtMostBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsAtMostBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;
	}
}
