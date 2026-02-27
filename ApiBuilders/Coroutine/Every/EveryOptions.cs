using System;

namespace LunyScript.ApiBuilders.Coroutine.Every
{
	internal struct EveryOptions
	{
		internal Int32 Interval;
		internal Int32 Delay;
		internal Coroutines.Coroutine.Process Process;
	}
}
