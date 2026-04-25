using UnityEditor;
using UnityEngine;

namespace LunyScript
{
	[CanEditMultipleObjects]
	public class LunyScriptScriptableObjects : LunyScriptData
	{
		[SerializeField] private ScriptableObjectArray _assets = new();

		public ScriptableObjectArray Assets { get => _assets; set => _assets = value; }

		public static implicit operator ScriptableObject[](LunyScriptScriptableObjects data) => data.Assets;
		public static implicit operator ScriptableObjectArray(LunyScriptScriptableObjects data) => data.Assets;
	}
}
