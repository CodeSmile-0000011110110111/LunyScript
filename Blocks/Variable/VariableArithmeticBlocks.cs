using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class VariableArithmeticBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.ScalarVarHandle VarHandle => _left?.VarHandle ?? _right?.VarHandle;

		protected VariableArithmeticBlock(VariableBlock left, VariableBlock right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}
	}

	internal sealed class VariableAddBlock : VariableArithmeticBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable + (Double)_right.Variable;
		}
		public static VariableAddBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableAddBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}
	}

	internal sealed class VariableSubtractBlock : VariableArithmeticBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable - (Double)_right.Variable;
		}
		public static VariableSubtractBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableSubtractBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}
	}

	internal sealed class VariableMultiplyBlock : VariableArithmeticBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable * (Double)_right.Variable;
		}
		public static VariableMultiplyBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableMultiplyBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}
	}

	internal sealed class VariableDivideBlock : VariableArithmeticBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable / (Double)_right.Variable;
		}
		public static VariableDivideBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableDivideBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}
	}
}
