using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class VariableArithmeticBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.ScalarVarHandle TargetHandle => _left?.TargetHandle ?? _right?.TargetHandle;

		protected VariableArithmeticBlock(VariableBlock left, VariableBlock right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}
	}

	internal sealed class VariableAddBlock : VariableArithmeticBlock
	{
		public static VariableAddBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableAddBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() =>
			_left.GetValue() + (Double)_right.GetValue();
	}

	internal sealed class VariableSubtractBlock : VariableArithmeticBlock
	{
		public static VariableSubtractBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableSubtractBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() =>
			_left.GetValue() - (Double)_right.GetValue();
	}

	internal sealed class VariableMultiplyBlock : VariableArithmeticBlock
	{
		public static VariableMultiplyBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableMultiplyBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() =>
			_left.GetValue() * (Double)_right.GetValue();
	}

	internal sealed class VariableDivideBlock : VariableArithmeticBlock
	{
		public static VariableDivideBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableDivideBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() =>
			_left.GetValue() / (Double)_right.GetValue();
	}
}
