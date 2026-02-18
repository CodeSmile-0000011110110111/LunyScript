using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class VariableComparisonBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.ScalarVarHandle TargetHandle => _left?.TargetHandle ?? _right?.TargetHandle;

		protected VariableComparisonBlock(VariableBlock left, VariableBlock right = null)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right;
		}
	}

	internal sealed class VariableIsEqualToBlock : VariableComparisonBlock
	{
		public static VariableIsEqualToBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsEqualToBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) == _right.GetValue(runtimeContext);

		public override String ToString() => $"{_left} == {_right}";
	}

	internal sealed class VariableIsNotEqualToBlock : VariableComparisonBlock
	{
		public static VariableIsNotEqualToBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsNotEqualToBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) != _right.GetValue(runtimeContext);
	}

	internal sealed class VariableIsGreaterThanBlock : VariableComparisonBlock
	{
		public static VariableIsGreaterThanBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsGreaterThanBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) > (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class VariableIsAtLeastBlock : VariableComparisonBlock
	{
		public static VariableIsAtLeastBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsAtLeastBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) >= (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class VariableIsLessThanBlock : VariableComparisonBlock
	{
		public static VariableIsLessThanBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsLessThanBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) < (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class VariableIsAtMostBlock : VariableComparisonBlock
	{
		public static VariableIsAtMostBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private VariableIsAtMostBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) <= (Double)_right.GetValue(runtimeContext);
	}
}
