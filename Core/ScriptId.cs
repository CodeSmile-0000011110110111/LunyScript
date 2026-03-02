using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a LunyScript definition (type).
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct ScriptId : IEquatable<ScriptId>, IComparable<ScriptId>
	{
		private const Int32 StartId = 1;
		private static Int32 s_NextId = StartId;
		internal static void Reset() => s_NextId = StartId;

		public readonly Int32 Value;

		private ScriptId(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique ScriptID.
		/// </summary>
		public static ScriptId Generate() => new(s_NextId++);

		public Boolean Equals(ScriptId other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is ScriptId other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(ScriptId other) => Value.CompareTo(other.Value);
		public override String ToString() => $"ScriptId:{Value}";

		public static Boolean operator ==(ScriptId left, ScriptId right) => left.Equals(right);
		public static Boolean operator !=(ScriptId left, ScriptId right) => !left.Equals(right);
	}
}
