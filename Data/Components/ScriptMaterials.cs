using UnityEditor;
using UnityEngine;

namespace LunyScript
{
	[CanEditMultipleObjects]
	public class ScriptMaterials : LunyScriptData
	{
		[SerializeField] private LunyScriptMaterialsAsset _asset;
		[SerializeField] private MaterialArray _array;

		public LunyScriptMaterialsAsset Asset { get => _asset; set => _asset = value; }
		public MaterialArray Array
		{
			get => _asset != null ? _asset.Array : _array;
			set
			{
				if (_asset != null)
					_asset.Array = value;
				else
					_array = value;
			}
		}
	}
}
