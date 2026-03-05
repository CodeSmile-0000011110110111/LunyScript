using Luny;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for variable blocks that compute their value on-the-fly from runtime context
	/// rather than reading from a Table entry. TargetHandle is permanently null, so Set/Add/etc.
	/// will throw LunyScriptVariableException ("Cannot modify read-only variable").
	/// </summary>
	public abstract class ComputedVariableBlock : VariableBlock
	{
		internal sealed override Table.ScalarVarHandle VarHandle => null;
	}
}
