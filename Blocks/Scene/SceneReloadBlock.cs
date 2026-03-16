using Luny;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : ActionBlock
	{
		public static ActionBlock Create() => new SceneReloadBlock();

		private SceneReloadBlock() {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => LunyEngine.Instance.Scene.ReloadScene();
	}
}
