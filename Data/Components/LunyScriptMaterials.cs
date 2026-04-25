using UnityEditor;
using UnityEngine;

namespace LunyScript
{
	[CanEditMultipleObjects]
	public class LunyScriptMaterials : LunyScriptDataBehaviour
	{
		[SerializeField] private LunyScriptMaterialsAsset _materialsAsset;
		[SerializeField] private MaterialArray _materials;

		public LunyScriptMaterialsAsset MaterialsAsset { get => _materialsAsset; set => _materialsAsset = value; }
		public MaterialArray Materials
		{
			get => _materialsAsset != null ? _materialsAsset.Materials : _materials;
			set
			{
				if (_materialsAsset != null)
					_materialsAsset.Materials = value;
				else
					_materials = value;
			}
		}


		public static implicit operator Material[](LunyScriptMaterials data) => data.Materials;
		public static implicit operator MaterialArray(LunyScriptMaterials data) => data.Materials;
	}
}
