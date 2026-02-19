using LunyScript.BlockBuilders;
using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	public readonly struct PrefabApi
	{
		private readonly Script _script;
		internal PrefabApi(Script script) => _script = script;

		public ScriptActionBlock Instantiate(String prefabName) => _script.Object.Create(prefabName).From(prefabName).Do();
	}
}
