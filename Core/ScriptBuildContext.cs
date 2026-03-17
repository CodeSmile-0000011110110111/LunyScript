namespace LunyScript
{
	/// <summary>
	/// Options that change runtime behaviour of the script.
	/// </summary>
	public struct ScriptRuntimeOptions
	{
		//public Boolean Singleton { get; set; }
		//public Boolean PatternMatching { get; set; }
	}

	/// <summary>
	/// Contains design-time data for a script, mainly Inspector-assigned values and references.
	/// </summary>
	public sealed class ScriptBuildData {}

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
		public ScriptBuildData Data { get; internal set; }
	}
}
