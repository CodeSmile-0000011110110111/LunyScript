using Luny;
using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Provides operations for managing Scenes and accessing the scene hierarchy.
	/// </summary>
	public readonly struct SceneApi
	{
		private readonly Script _script;
		private readonly StackTrace _trace;

		internal SceneApi(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		public ActionBlock Reload() => SceneReloadBlock.Create();
	}
}
