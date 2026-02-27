using System;

namespace LunyScript.Blocks.Guards
{
	internal abstract class EventGuard
	{
		public abstract Boolean CanExecute();
		public abstract void WillExecute();
	}
}
