using Luny;
using System;
using UnityEngine;

namespace LunyScript
{
	public abstract partial class Script
	{
		// TODO: FindChildren

		public T GetData<T>() where T : LunyScriptDataBehaviour
		{
			var go = _runtimeContext.LunyGameObject.NativeObject as GameObject;
			var data = go.GetComponent<T>();
			if (data == null)
				LunyLogger.LogWarning($"{typeof(T).Name} not found", _runtimeContext.LunyGameObject.ToString());

			return data;
		}

		public T GetData<T>(String key) where T : LunyScriptDataBehaviour
		{
			var go = _runtimeContext.LunyGameObject.NativeObject as GameObject;
			var data = go.GetComponents<T>();
			foreach (var dataComponent in data)
			{
				if (dataComponent.Key == key)
					return dataComponent;
			}
			LunyLogger.LogWarning($"{typeof(T).Name} with key '{key}' not found", _runtimeContext.LunyGameObject.ToString());

			return null;
		}

		public LunyScriptSceneObjects GetGameObjects() => GetData<LunyScriptSceneObjects>();
		public LunyScriptSceneObjects GetGameObjects(String key) => GetData<LunyScriptSceneObjects>(key);
		public LunyScriptMaterials GetMaterials() => GetData<LunyScriptMaterials>();
		public LunyScriptMaterials GetMaterials(String key) => GetData<LunyScriptMaterials>(key);
	}
}
