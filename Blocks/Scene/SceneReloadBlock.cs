using Luny;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : ActionBlock
	{
		public static ActionBlock Create(LunyStackTrace trace) => new SceneReloadBlock(trace);

		private SceneReloadBlock(LunyStackTrace trace)
			: base(trace) {}

		protected internal override void Execute(IScriptRuntimeContext context) => LunyEngine.Instance.Scene.ReloadScene();
	}
}
