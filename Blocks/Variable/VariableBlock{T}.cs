using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for blocks that evaluate to a typed struct value.
	/// Separate from the scalar <see cref="VariableBlock"/> hierarchy; does not participate
	/// in arithmetic, comparison, or boolean condition operators.
	/// </summary>
	public abstract class VariableBlock<T> where T : struct
	{
		public abstract T Value { get; }

		public static implicit operator VariableBlock<T>(T value) => new ConstantBlock(value);

		public override String ToString() => typeof(T).Name;

		private sealed class ConstantBlock : VariableBlock<T>
		{
			private readonly T _value;

			public override T Value => _value;

			internal ConstantBlock(T value) => _value = value;

			public override String ToString() => _value.ToString();
		}
	}
}
