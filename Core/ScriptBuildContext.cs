using Luny;

namespace LunyScript
{
	/// <summary>
	/// Contains design-time [Data](xref:LunyScript.ScriptContext.Data) and runtime behaviour [Options](xref:LunyScript.ScriptContext.Options).
	/// </summary>
	public sealed class ScriptBuildContext
	{
		/// <summary>
		/// Runtime behaviour options.
		/// </summary>
		public ScriptRuntimeOptions Options;
		/// <summary>
		/// Input values and references, for instance: layers, groups, names, design-time values, asset references.
		/// </summary>
		public ScriptInspectorData Data { get; internal set; }
	}
}
