using UnityEngine;

namespace LunyScript
{
	[CreateAssetMenu(fileName = nameof(LunyScriptMaterialsAsset), menuName = "LunyScript/" + nameof(LunyScriptMaterialsAsset))]
	public class LunyScriptMaterialsAsset : LunyScriptDataAsset
	{
		[field:SerializeField] public MaterialArray Array = new();
	}
}
