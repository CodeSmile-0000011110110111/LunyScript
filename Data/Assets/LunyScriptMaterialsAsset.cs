using UnityEngine;

namespace LunyScript
{
	[CreateAssetMenu(fileName = nameof(LunyScriptMaterialsAsset), menuName = nameof(LunyScript) + "/" + nameof(LunyScriptMaterialsAsset))]
	public class LunyScriptMaterialsAsset : LunyScriptDataAsset
	{
		[SerializeField] private MaterialArray _materials = new();

		public MaterialArray Materials { get => _materials; set => _materials = value; }

		public static implicit operator Material[](LunyScriptMaterialsAsset data) => data.Materials;
		public static implicit operator MaterialArray(LunyScriptMaterialsAsset data) => data.Materials;
	}
}
