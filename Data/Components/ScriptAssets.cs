using UnityEditor;
using UnityEngine;

namespace LunyScript
{
	[CanEditMultipleObjects]
	public class ScriptAssets : LunyScriptData
	{
		[field:SerializeField] public ScriptableObjectArray Array = new();
	}
}
