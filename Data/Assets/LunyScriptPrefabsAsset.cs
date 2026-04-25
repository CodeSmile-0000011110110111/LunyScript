using UnityEngine;

namespace LunyScript
{
	[CreateAssetMenu(fileName = nameof(LunyScriptPrefabsAsset), menuName = nameof(LunyScript) + "/" + nameof(LunyScriptPrefabsAsset))]
	public class LunyScriptPrefabsAsset : LunyScriptDataSO
	{
		[SerializeField] private GameObjectArray _prefabs = new();

		public GameObjectArray Prefabs { get => _prefabs; set => _prefabs = value; }

		public static implicit operator GameObject[](LunyScriptPrefabsAsset data) => data.Prefabs;
		public static implicit operator GameObjectArray(LunyScriptPrefabsAsset data) => data.Prefabs;
	}
}
