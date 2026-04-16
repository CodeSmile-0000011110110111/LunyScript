using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Contains design-time data for a script, mainly Inspector-assigned variable values.
	/// </summary>
	[Serializable]
	public sealed class ScriptInspectorData
	{
		public List<InspectorVariable> Variables = new();
	}
}
