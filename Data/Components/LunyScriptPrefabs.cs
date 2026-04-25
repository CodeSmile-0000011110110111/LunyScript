using Luny;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace LunyScript
{
	[CanEditMultipleObjects]
	public class LunyScriptPrefabs : LunyScriptData
	{
		[SerializeField] private GameObjectArray _prefabs = new();

		public GameObjectArray Prefabs { get => _prefabs; set => _prefabs = value; }

		public static implicit operator GameObject[](LunyScriptPrefabs data) => data.Prefabs;
		public static implicit operator GameObjectArray(LunyScriptPrefabs data) => data.Prefabs;

		private void OnValidate() => SetInSceneReferencesToNull();

		[Conditional("UNITY_EDITOR")]
		private void SetInSceneReferencesToNull()
		{
#if UNITY_EDITOR
			if (_prefabs == null)
				return;

			for (var i = 0; i < _prefabs.Length; i++)
			{
				var go = _prefabs[i];
				if (go == null)
					continue;

				// Check if the object is part of a scene
				if (!EditorUtility.IsPersistent(go) || go.scene.name != null) // latter check is for prefab isolation mode
				{
					LunyLogger.LogWarning($"Unacceptable '{go.name}' ({go.GetEntityId()}): in-scene GameObject (from: {go.scene.name}) " +
					                      "will not persist as reference outside the scene. Assigning 'null' instead.");
					_prefabs[i] = null;
					EditorUtility.SetDirty(this);
				}
			}
#endif
		}
	}
}
