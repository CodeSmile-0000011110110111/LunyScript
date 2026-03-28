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

		protected VariableBlock(StackTrace trace = null) : base(trace) {}

		public static implicit operator VariableBlock(Variable value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Int32 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Int64 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Single value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Double value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Boolean value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(String value) => ConstantVariableBlock.Create(value);

		// Arithmetic Operators
		public static VariableArithmeticBlock operator +(VariableBlock left, Variable right) =>
			VariableAddBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableArithmeticBlock operator +(VariableBlock left, VariableBlock right) => VariableAddBlock.Create(left, right);

		public static VariableArithmeticBlock operator +(Variable left, VariableBlock right) =>
			VariableAddBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableArithmeticBlock operator -(VariableBlock left, Variable right) =>
			VariableSubtractBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableArithmeticBlock operator -(VariableBlock left, VariableBlock right) => VariableSubtractBlock.Create(left, right);

		public static VariableArithmeticBlock operator -(Variable left, VariableBlock right) =>
			VariableSubtractBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableArithmeticBlock operator *(VariableBlock left, Variable right) =>
			VariableMultiplyBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableArithmeticBlock operator *(VariableBlock left, VariableBlock right) => VariableMultiplyBlock.Create(left, right);

		public static VariableArithmeticBlock operator *(Variable left, VariableBlock right) =>
			VariableMultiplyBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableArithmeticBlock operator /(VariableBlock left, Variable right) =>
			VariableDivideBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableArithmeticBlock operator /(VariableBlock left, VariableBlock right) => VariableDivideBlock.Create(left, right);

		public static VariableArithmeticBlock operator /(Variable left, VariableBlock right) =>
			VariableDivideBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableArithmeticBlock operator ++(VariableBlock a) => VariableAddBlock.Create(a, ConstantVariableBlock.Create(1));
		public static VariableArithmeticBlock operator --(VariableBlock a) => VariableSubtractBlock.Create(a, ConstantVariableBlock.Create(1));

		// Comparison Operators
		public static VariableBlock operator ==(VariableBlock left, Variable right) => left is null
			? right.Object is null
			: VariableIsEqualToBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator ==(VariableBlock left, VariableBlock right) =>
			left is null ? right is null : VariableIsEqualToBlock.Create(left, right);

		public static VariableBlock operator !=(VariableBlock left, Variable right) => left is null
			? right.Object is not null
			: VariableIsNotEqualToBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator !=(VariableBlock left, VariableBlock right) =>
			left is null ? right is null : VariableIsNotEqualToBlock.Create(left, right);

		public static VariableBlock operator >(VariableBlock left, Variable right) =>
			VariableIsGreaterThanBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator >(VariableBlock left, VariableBlock right) => VariableIsGreaterThanBlock.Create(left, right);

		public static VariableBlock operator >=(VariableBlock left, Variable right) =>
			VariableIsAtLeastBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator >=(VariableBlock left, VariableBlock right) => VariableIsAtLeastBlock.Create(left, right);

		public static VariableBlock operator <(VariableBlock left, Variable right) =>
			VariableIsLessThanBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator <(VariableBlock left, VariableBlock right) => VariableIsLessThanBlock.Create(left, right);

		public static VariableBlock operator <=(VariableBlock left, Variable right) =>
			VariableIsAtMostBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator <=(VariableBlock left, VariableBlock right) => VariableIsAtMostBlock.Create(left, right);

		public static VariableBlock operator !(VariableBlock operand) => NegationOperatorBlock.Create(operand);

		internal Double Value => Variable.AsDouble();

		internal abstract Variable Variable { get; }

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

		public ActionBlock Set(Variable value) => VariableSetValueBlock.Create(VarHandle, ConstantVariableBlock.Create(value));
		public ActionBlock Set(VariableBlock value) => VariableSetValueBlock.Create(VarHandle, value);
		public ActionBlock Add(Variable value) => Set(this + value);
		public ActionBlock Add(VariableBlock value) => Set(this + value);
		public ActionBlock Sub(Variable value) => Set(this - value);
		public ActionBlock Sub(VariableBlock value) => Set(this - value);
		public ActionBlock Mul(Variable value) => Set(this * value);
		public ActionBlock Mul(VariableBlock value) => Set(this * value);
		public ActionBlock Div(Variable value) => Set(this / value);
		public ActionBlock Div(VariableBlock value) => Set(this / value);
		public ActionBlock Inc() => Set(this + 1);
		public ActionBlock Dec() => Set(this - 1);
		public ActionBlock Toggle() => Set(!this);

		// Aliases
		public ActionBlock Subtract(Variable value) => Sub(value);
		public ActionBlock Subtract(VariableBlock value) => Sub(value);
		public ActionBlock Multiply(Variable value) => Mul(value);
		public ActionBlock Multiply(VariableBlock value) => Mul(value);
		public ActionBlock Divide(Variable value) => Div(value);
		public ActionBlock Divide(VariableBlock value) => Div(value);
		public ActionBlock Increment() => Inc();
		public ActionBlock Decrement() => Dec();

		public override String ToString() =>
			VarHandle != null ? $"\"{VarHandle.Name}\" ({VarHandle.Variable.Value})" : $"\"{Variable.Name}\" ({Variable.Value})";
	}
}
