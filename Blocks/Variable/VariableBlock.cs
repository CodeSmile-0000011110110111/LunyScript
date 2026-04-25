using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for variable blocks that evaluate to a runtime Variable.
	/// Extends ScriptConditionBlock because variables are implicitly usable as conditions
	/// (via AsBoolean conversion).
	/// </summary>
	public abstract class VariableBlock : ConditionBlock
	{
		internal virtual Table.VarHandle VarHandle => null;

		public static implicit operator VariableBlock(Variable value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		public static implicit operator VariableBlock(Int32 value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		public static implicit operator VariableBlock(Int64 value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		public static implicit operator VariableBlock(Single value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		public static implicit operator VariableBlock(Double value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		public static implicit operator VariableBlock(Boolean value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		public static implicit operator VariableBlock(String value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		// Arithmetic Operators
		public static VariableArithmeticBlock operator +(VariableBlock left, Variable right) =>
			VariableAddBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Add)));

		public static VariableArithmeticBlock operator +(VariableBlock left, VariableBlock right) =>
			VariableAddBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Add)));

		public static VariableArithmeticBlock operator +(Variable left, VariableBlock right) =>
			VariableAddBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Add)));

		public static VariableArithmeticBlock operator -(VariableBlock left, Variable right) =>
			VariableSubtractBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Sub)));

		public static VariableArithmeticBlock operator -(VariableBlock left, VariableBlock right) =>
			VariableSubtractBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Sub)));

		public static VariableArithmeticBlock operator -(Variable left, VariableBlock right) =>
			VariableSubtractBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Sub)));

		public static VariableArithmeticBlock operator *(VariableBlock left, Variable right) =>
			VariableMultiplyBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Mul)));

		public static VariableArithmeticBlock operator *(VariableBlock left, VariableBlock right) =>
			VariableMultiplyBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Mul)));

		public static VariableArithmeticBlock operator *(Variable left, VariableBlock right) =>
			VariableMultiplyBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Mul)));

		public static VariableArithmeticBlock operator /(VariableBlock left, Variable right) =>
			VariableDivideBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Div)));

		public static VariableArithmeticBlock operator /(VariableBlock left, VariableBlock right) =>
			VariableDivideBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Div)));

		public static VariableArithmeticBlock operator /(Variable left, VariableBlock right) =>
			VariableDivideBlock.Create(left, right, ScriptTrace.TryCreateStackTrace(nameof(Div)));

		public static VariableArithmeticBlock operator ++(VariableBlock left) =>
			VariableAddBlock.Create(left, 1, ScriptTrace.TryCreateStackTrace(nameof(Inc)));

		public static VariableArithmeticBlock operator --(VariableBlock left) =>
			VariableSubtractBlock.Create(left, 1, ScriptTrace.TryCreateStackTrace(nameof(Dec)));

		// Comparison Operators
		public static VariableBlock operator ==(VariableBlock left, Variable right) => left is null
			? right.Object is null
			: VariableIsEqualToBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("Equals"));

		public static VariableBlock operator ==(VariableBlock left, VariableBlock right) => left is null
			? right is null
			: VariableIsEqualToBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("Equals"));

		public static VariableBlock operator !=(VariableBlock left, Variable right) => left is null
			? right.Object is not null
			: VariableIsNotEqualToBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("NotEquals"));

		public static VariableBlock operator !=(VariableBlock left, VariableBlock right) => left is null
			? right is null
			: VariableIsNotEqualToBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("NotEquals"));

		public static VariableBlock operator >(VariableBlock left, Variable right) =>
			VariableIsGreaterThanBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("Greater"));

		public static VariableBlock operator >(VariableBlock left, VariableBlock right) =>
			VariableIsGreaterThanBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("Greater"));

		public static VariableBlock operator >=(VariableBlock left, Variable right) =>
			VariableIsAtLeastBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("GreaterOrEqual"));

		public static VariableBlock operator >=(VariableBlock left, VariableBlock right) =>
			VariableIsAtLeastBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("GreaterOrEqual"));

		public static VariableBlock operator <(VariableBlock left, Variable right) =>
			VariableIsLessThanBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("Less"));

		public static VariableBlock operator <(VariableBlock left, VariableBlock right) =>
			VariableIsLessThanBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("Less"));

		public static VariableBlock operator <=(VariableBlock left, Variable right) =>
			VariableIsAtMostBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("LessOrEqual"));

		public static VariableBlock operator <=(VariableBlock left, VariableBlock right) =>
			VariableIsAtMostBlock.Create(left, right, ScriptTrace.TryCreateStackTrace("LessOrEqual"));

		public static VariableBlock operator !(VariableBlock operand) =>
			NegationOperatorBlock.Create(operand, ScriptTrace.TryCreateStackTrace("Neg"));

		public Double Value => Variable.AsDouble();

		internal abstract Variable Variable { get; }

		protected VariableBlock(LunyStackTrace trace = null)
			: base(trace) {}

		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => Variable.AsBoolean();

		private Boolean Equals(VariableBlock other) => throw new NotImplementedException($"{nameof(VariableBlock)}.{nameof(Equals)}()");

		public override Boolean Equals(Object obj)
		{
			if (obj is null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;

			return Equals((VariableBlock)obj);
		}

		public override Int32 GetHashCode() => HashCode.Combine(Variable != null ? Variable.GetHashCode() : 0, Value);

		private ActionBlock Set(Variable value, LunyStackTrace trace) => VariableSetValueBlock.Create(VarHandle,
			LiteralVariableBlock.Create(value, trace), trace);

		private ActionBlock Set(VariableBlock value, LunyStackTrace trace) => VariableSetValueBlock.Create(VarHandle, value, trace);

		public ActionBlock Set(Variable value) => VariableSetValueBlock.Create(VarHandle,
			LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace(nameof(Set))),
			ScriptTrace.TryCreateStackTrace(nameof(Set)));

		public ActionBlock Set(VariableBlock value) =>
			VariableSetValueBlock.Create(VarHandle, value, ScriptTrace.TryCreateStackTrace(nameof(Set)));

		public ActionBlock Add(Variable value) => Set(this + value, ScriptTrace.TryCreateStackTrace(nameof(Add)));
		public ActionBlock Add(VariableBlock value) => Set(this + value, ScriptTrace.TryCreateStackTrace(nameof(Add)));
		public ActionBlock Sub(Variable value) => Set(this - value, ScriptTrace.TryCreateStackTrace(nameof(Sub)));
		public ActionBlock Sub(VariableBlock value) => Set(this - value, ScriptTrace.TryCreateStackTrace(nameof(Sub)));
		public ActionBlock Mul(Variable value) => Set(this * value, ScriptTrace.TryCreateStackTrace(nameof(Mul)));
		public ActionBlock Mul(VariableBlock value) => Set(this * value, ScriptTrace.TryCreateStackTrace(nameof(Mul)));
		public ActionBlock Div(Variable value) => Set(this / value, ScriptTrace.TryCreateStackTrace(nameof(Div)));
		public ActionBlock Div(VariableBlock value) => Set(this / value, ScriptTrace.TryCreateStackTrace(nameof(Div)));
		public ActionBlock Inc() => Set(this + 1, ScriptTrace.TryCreateStackTrace(nameof(Inc)));
		public ActionBlock Dec() => Set(this - 1, ScriptTrace.TryCreateStackTrace(nameof(Dec)));
		public ActionBlock Toggle() => Set(!this, ScriptTrace.TryCreateStackTrace(nameof(Toggle)));

		// Aliases
		/*
		public ActionBlock Subtract(Variable value) => Sub(value);
		public ActionBlock Subtract(VariableBlock value) => Sub(value);
		public ActionBlock Multiply(Variable value) => Mul(value);
		public ActionBlock Multiply(VariableBlock value) => Mul(value);
		public ActionBlock Divide(Variable value) => Div(value);
		public ActionBlock Divide(VariableBlock value) => Div(value);
		public ActionBlock Increment() => Inc();
		public ActionBlock Decrement() => Dec();
		*/

		public override String ToString()
		{
			if (VarHandle != null)
				return !String.IsNullOrEmpty(VarHandle.Name) ? $"(\"{VarHandle.Name}\": {VarHandle.Variable})" : $"({VarHandle.Variable})";

			return !String.IsNullOrEmpty(Variable.Name) ? $"(\"{Variable.Name}\": {Variable})" : $"({Variable})";
		}
	}
}
