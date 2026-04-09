using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class VariableComparisonBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.VarHandle VarHandle => _left?.VarHandle ?? _right?.VarHandle;

		protected VariableComparisonBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(trace)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right;

#if DEBUG || LUNYSCRIPT_DEBUG
			var leftType = _left.Variable.Type;
			var rightType = _right.Variable.Type;
			var block = GetType();

			// only equality/inequality allowed for string and boolean types
			if (block != typeof(VariableIsEqualToBlock) && block != typeof(VariableIsNotEqualToBlock))
			{
				if (leftType == Variable.ValueType.String || leftType == Variable.ValueType.Boolean)
					throw new LunyScriptException($"Attempt to compare {ToString()} with: {_left.Variable}");
				if (rightType == Variable.ValueType.String || rightType == Variable.ValueType.Boolean)
					throw new LunyScriptException($"Attempt to compare {ToString()} with: {_right.Variable}");
			}
#endif
		}
	}

	internal sealed class VariableIsEqualToBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable == _right.Variable;
		}
		public static VariableIsEqualToBlock Create(VariableBlock left, VariableBlock right, LunyStackTrace trace) => new(left, right, trace);

		private VariableIsEqualToBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(left, right, trace) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;

		public override String ToString() => $"{_left} == {_right}";
	}

	internal sealed class VariableIsNotEqualToBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable != _right.Variable;
		}

		public static VariableIsNotEqualToBlock Create(VariableBlock left, VariableBlock right, LunyStackTrace trace) => new(left, right, trace);

		private VariableIsNotEqualToBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(left, right, trace) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;

		public override String ToString() => $"{_left} != {_right}";
	}

	internal sealed class VariableIsGreaterThanBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable > (Double)_right.Variable;
		}

		public static VariableIsGreaterThanBlock Create(VariableBlock left, VariableBlock right, LunyStackTrace trace) => new(left, right, trace);

		private VariableIsGreaterThanBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(left, right, trace) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;

		public override String ToString() => $"{_left} > {_right}";
	}

	internal sealed class VariableIsAtLeastBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable >= (Double)_right.Variable;
		}

		public static VariableIsAtLeastBlock Create(VariableBlock left, VariableBlock right, LunyStackTrace trace) => new(left, right, trace);

		private VariableIsAtLeastBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(left, right, trace) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;

		public override String ToString() => $"{_left} >= {_right}";
	}

	internal sealed class VariableIsLessThanBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable < (Double)_right.Variable;
		}

		public static VariableIsLessThanBlock Create(VariableBlock left, VariableBlock right, LunyStackTrace trace) => new(left, right, trace);

		private VariableIsLessThanBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(left, right, trace) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;

		public override String ToString() => $"{_left} < {_right}";
	}

	internal sealed class VariableIsAtMostBlock : VariableComparisonBlock
	{
		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable <= (Double)_right.Variable;
		}
		public static VariableIsAtMostBlock Create(VariableBlock left, VariableBlock right, LunyStackTrace trace) => new(left, right, trace);

		private VariableIsAtMostBlock(VariableBlock left, VariableBlock right, LunyStackTrace trace)
			: base(left, right, trace) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable;

		public override String ToString() => $"{_left} <= {_right}";
	}
}
