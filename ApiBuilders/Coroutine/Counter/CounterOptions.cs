using System;

namespace LunyScript.ApiBuilders.Coroutine.Counter
{
	internal struct CounterOptions
	{
		internal String Name;
		internal Int32 Amount;
		internal Coroutines.Coroutine.Continuation Continuation;
	}
}
