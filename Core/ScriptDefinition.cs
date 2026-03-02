using System;

namespace LunyScript
{
	/// <summary>
	/// Metadata for a discovered LunyScript type.
	/// Represents the script ID and associated Type, not an instance.
	/// </summary>
	public interface IScriptDefinition
	{
		ScriptId ScriptId { get; }
		Type Type { get; }
		String Name { get; }
	}

	/// <summary>
	/// Metadata for a discovered LunyScript type.
	/// Represents the script ID and associated Type, not an instance.
	/// </summary>
	internal sealed class ScriptDefinition : IScriptDefinition
	{
		/// <summary>
		/// Unique identifier for this script definition.
		/// </summary>
		public ScriptId ScriptId { get; }

		/// <summary>
		/// The C# Type of the LunyScript subclass.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// The name of the script (used for object binding).
		/// Derived from the Type name by default.
		/// </summary>
		public String Name => Type.Name;

		internal ScriptDefinition(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (!typeof(Script).IsAssignableFrom(type))
				throw new ArgumentException($"Type {type.Name} does not inherit from {nameof(Script)}", nameof(type));

			ScriptId = ScriptId.Generate();
			Type = type;
		}

		public override String ToString() => $"{ScriptId} -> {Type.FullName}";
	}
}
