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
		/// Engine references assigned in the Inspector, accessible via <see cref="Script.Ref"/>.
		/// </summary>
		public IEngineReferences EngineReferences { get; internal set; }
	}

	/// <summary>
	/// Options that change runtime behaviour of the script.
	/// </summary>
	public struct ScriptRuntimeOptions
	{
		//public Boolean Singleton { get; set; }
		//public Boolean PatternMatching { get; set; }
	}
}
