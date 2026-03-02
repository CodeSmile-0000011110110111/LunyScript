using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a Sequence/FSM/BT instance.
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct ScriptBlockId : IEquatable<ScriptBlockId>, IComparable<ScriptBlockId>
	{
		private const Int32 StartId = 1;
		private static Int32 s_NextId = StartId;
		internal static void Reset() => s_NextId = StartId;

		public readonly Int32 Value;

		private ScriptBlockId(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique SequenceID.
		/// </summary>
		public static ScriptBlockId Generate() => new(s_NextId++);

		public Boolean Equals(ScriptBlockId other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is ScriptBlockId other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(ScriptBlockId other) => Value.CompareTo(other.Value);
		public override String ToString() => $"BlockId:{Value}";

		public static Boolean operator ==(ScriptBlockId left, ScriptBlockId right) => left.Equals(right);
		public static Boolean operator !=(ScriptBlockId left, ScriptBlockId right) => !left.Equals(right);
	}
}
