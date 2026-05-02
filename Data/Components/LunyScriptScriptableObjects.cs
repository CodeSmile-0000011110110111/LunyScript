using UnityEngine;

namespace LunyScript
{
	public class LunyScriptScriptableObjects : LunyScriptDataBehaviour
	{
		[SerializeField] private ScriptableObjectArray _assets = new();

		public ScriptableObjectArray Assets { get => _assets; set => _assets = value; }

		public static implicit operator ScriptableObject[](LunyScriptScriptableObjects data) => data.Assets;
		public static implicit operator ScriptableObjectArray(LunyScriptScriptableObjects data) => data.Assets;
	}
}
