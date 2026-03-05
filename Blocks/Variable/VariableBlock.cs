using Luny;
using Luny.Engine.Bridge;
using LunyScript.Exceptions;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for variable blocks that evaluate to a runtime Variable.
	/// Extends ScriptConditionBlock because variables are implicitly usable as conditions
	/// (via AsBoolean conversion).
	/// </summary>
	public abstract class VariableBlock : ScriptConditionBlock
	{
		internal virtual Table.ScalarVarHandle TargetHandle => null;

		public static implicit operator VariableBlock(Variable value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Int32 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Int64 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Single value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Double value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Boolean value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(String value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(LunyVector2 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(LunyVector3 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(LunyQuaternion value) => ConstantVariableBlock.Create(value);

		// Arithmetic Operators
		public static VariableBlock operator +(VariableBlock left, Variable right) =>
			VariableAddBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator +(VariableBlock left, VariableBlock right) => VariableAddBlock.Create(left, right);

		public static VariableBlock operator +(Variable left, VariableBlock right) =>
			VariableAddBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator -(VariableBlock left, Variable right) =>
			VariableSubtractBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator -(VariableBlock left, VariableBlock right) => VariableSubtractBlock.Create(left, right);

		public static VariableBlock operator -(Variable left, VariableBlock right) =>
			VariableSubtractBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator *(VariableBlock left, Variable right) =>
			VariableMultiplyBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator *(VariableBlock left, VariableBlock right) => VariableMultiplyBlock.Create(left, right);

		public static VariableBlock operator *(Variable left, VariableBlock right) =>
			VariableMultiplyBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator /(VariableBlock left, Variable right) =>
			VariableDivideBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator /(VariableBlock left, VariableBlock right) => VariableDivideBlock.Create(left, right);

		public static VariableBlock operator /(Variable left, VariableBlock right) =>
			VariableDivideBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator ++(VariableBlock a) => a + 1;
		public static VariableBlock operator --(VariableBlock a) => a - 1;

		// Comparison Operators
		public static VariableBlock operator ==(VariableBlock left, Variable right)
		{
			if (left is null) return right.Object is null;
			return VariableIsEqualToBlock.Create(left, ConstantVariableBlock.Create(right));
		}

		public static VariableBlock operator ==(VariableBlock left, VariableBlock right)
		{
			if (left is null) return right is null;
			return VariableIsEqualToBlock.Create(left, right);
		}

		public static VariableBlock operator !=(VariableBlock left, Variable right)
		{
			if (left is null) return right.Object is not null;
			return VariableIsNotEqualToBlock.Create(left, ConstantVariableBlock.Create(right));
		}

		public static VariableBlock operator !=(VariableBlock left, VariableBlock right)
		{
			if (left is null) return right is null;
			return VariableIsNotEqualToBlock.Create(left, right);
		}

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

		public static VariableBlock operator !(VariableBlock operand) => NotBlock.Create(operand);

		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => GetValue().AsBoolean();

		internal abstract Variable GetValue();

		/// <summary>
		/// Returns the variable value as a specific struct type. Subclasses can override to avoid boxing.
		/// Default implementation uses GetValue() and converts via Unsafe.As (JIT-eliminated typeof checks).
		/// </summary>
		internal virtual T GetValue<T>() where T : struct
		{
			var v = GetValue();
			if (typeof(T) == typeof(Double))
			{
				var d = v.AsDouble();
				return Unsafe.As<Double, T>(ref d);
			}
			if (typeof(T) == typeof(Single))
			{
				var f = v.AsSingle();
				return Unsafe.As<Single, T>(ref f);
			}
			if (typeof(T) == typeof(Boolean))
			{
				var b = v.AsBoolean();
				return Unsafe.As<Boolean, T>(ref b);
			}
			if (typeof(T) == typeof(Int32))
			{
				var i = v.AsInt32();
				return Unsafe.As<Int32, T>(ref i);
			}
			if (typeof(T) == typeof(LunyVector2))
			{
				var vec2 = v.AsVector2();
				return Unsafe.As<LunyVector2, T>(ref vec2);
			}
			if (typeof(T) == typeof(LunyVector3))
			{
				var vec3 = v.AsVector3();
				return Unsafe.As<LunyVector3, T>(ref vec3);
			}
			if (typeof(T) == typeof(LunyQuaternion))
			{
				var q = v.AsQuaternion();
				return Unsafe.As<LunyQuaternion, T>(ref q);
			}

			throw new LunyScriptVariableException($"Cannot convert {v.Type} to {typeof(T).Name}");
		}

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

		public override Int32 GetHashCode() => throw new NotImplementedException($"{nameof(VariableBlock)}.{nameof(GetHashCode)}()");

		// Actions
		private Table.ScalarVarHandle GetHandleOrThrow()
		{
			var handle = TargetHandle;
			if (handle == null)
				throw new LunyScriptVariableException($"Cannot modify read-only variable: {GetType().Name}");
			if (handle.IsConstant)
				throw new LunyScriptVariableException($"Cannot modify constant variable: {handle.Name}");

			return handle;
		}

		public ScriptActionBlock Set(Variable value) => VariableSetValueBlock.Create(GetHandleOrThrow(), ConstantVariableBlock.Create(value));

		public ScriptActionBlock Set(VariableBlock value) => VariableSetValueBlock.Create(GetHandleOrThrow(), value);

		public ScriptActionBlock Inc() => Add(1);
		public ScriptActionBlock Dec() => Sub(1);

		public ScriptActionBlock Add(Variable value) => Set(this + value);
		public ScriptActionBlock Add(VariableBlock value) => Set(this + value);

		public ScriptActionBlock Sub(Variable value) => Set(this - value);
		public ScriptActionBlock Sub(VariableBlock value) => Set(this - value);

		public ScriptActionBlock Mul(Variable value) => Set(this * value);
		public ScriptActionBlock Mul(VariableBlock value) => Set(this * value);

		public ScriptActionBlock Div(Variable value) => Set(this / value);
		public ScriptActionBlock Div(VariableBlock value) => Set(this / value);

		public ScriptActionBlock Toggle() => Set(!this);
	}
}
