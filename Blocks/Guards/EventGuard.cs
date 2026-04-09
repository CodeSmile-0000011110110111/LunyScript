using System;

namespace LunyScript.Blocks
{
	internal abstract class EventGuard
	{
		public abstract Boolean CanExecute();
		public abstract void WillExecute();
	}
}
