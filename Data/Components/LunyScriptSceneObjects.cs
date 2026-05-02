using UnityEngine;

namespace LunyScript
{
	public class LunyScriptSceneObjects : LunyScriptDataBehaviour
	{
		[SerializeField] private GameObjectArray _gameObjects = new();

		public GameObjectArray GameObjects { get => _gameObjects; set => _gameObjects = value; }

		public static implicit operator GameObject[](LunyScriptSceneObjects data) => data.GameObjects;
		public static implicit operator GameObjectArray(LunyScriptSceneObjects data) => data.GameObjects;
	}
}
