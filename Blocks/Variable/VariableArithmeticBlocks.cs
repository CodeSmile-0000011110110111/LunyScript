using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	public abstract class VariableArithmeticBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.VarHandle VarHandle => _left?.VarHandle ?? _right?.VarHandle;

		protected VariableArithmeticBlock(VariableBlock left, VariableBlock right, StackTrace trace)
			: base(trace)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));

#if DEBUG || LUNYSCRIPT_DEBUG
			var leftType = _left.Variable.Type;
			// allow "string.Add(whatever)" for string concat, but everything else should fail
			if (leftType != Luny.Variable.ValueType.String || GetType() != typeof(VariableAddBlock))
			{
				if (leftType != Luny.Variable.ValueType.Number)
					throw new LunyScriptException($"Attempt to perform {ToString()} with: {_left.Variable}");

				var rightType = _right.Variable.Type;
				if (rightType != Luny.Variable.ValueType.Number)
					throw new LunyScriptException($"Attempt to perform {ToString()} with: {_right.Variable}");
			}
#endif
		}
	}

	internal sealed class VariableAddBlock : VariableArithmeticBlock
	{
		internal override Luny.Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var leftVar = _left.Variable;
				if (leftVar.Type == Luny.Variable.ValueType.String)
					return leftVar + _right.Variable.AsString();

				return leftVar + _right.Variable.Value;
			}
		}
		public static VariableAddBlock Create(VariableBlock left, VariableBlock right, StackTrace trace) => new(left, right, trace);

		private VariableAddBlock(VariableBlock left, VariableBlock right, StackTrace trace)
			: base(left, right, trace) {}

		public override String ToString() => $"{_left} + {_right}";
	}

	internal sealed class VariableSubtractBlock : VariableArithmeticBlock
	{
		internal override Luny.Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable - _right.Variable.Value;
		}
		public static VariableSubtractBlock Create(VariableBlock left, VariableBlock right, StackTrace trace) => new(left, right, trace);

		private VariableSubtractBlock(VariableBlock left, VariableBlock right, StackTrace trace)
			: base(left, right, trace) {}

		public override String ToString() => $"{_left} - {_right}";
	}

	internal sealed class VariableMultiplyBlock : VariableArithmeticBlock
	{
		internal override Luny.Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _left.Variable * _right.Variable.Value;
		}
		public static VariableMultiplyBlock Create(VariableBlock left, VariableBlock right, StackTrace trace) => new(left, right, trace);

		private VariableMultiplyBlock(VariableBlock left, VariableBlock right, StackTrace trace)
			: base(left, right, trace) {}

		public override String ToString() => $"{_left} * {_right}";
	}

	internal sealed class VariableDivideBlock : VariableArithmeticBlock
	{
		internal override Luny.Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				// A common "safe" threshold for dividing without provoking "div by zero" or "Infinity" results
				const Double SafeDivideThreshold = 1e-10;

				var denominator = _right.Variable.Value;
				return Math.Abs(denominator) >= SafeDivideThreshold ? _left.Variable / denominator : 0d;
			}
		}
		public static VariableDivideBlock Create(VariableBlock left, VariableBlock right, StackTrace trace) => new(left, right, trace);

		private VariableDivideBlock(VariableBlock left, VariableBlock right, StackTrace trace)
			: base(left, right, trace) {}

		public override String ToString() => $"{_left} / {_right}";
	}
}
