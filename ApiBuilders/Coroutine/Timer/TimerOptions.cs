using System;

namespace LunyScript.ApiBuilders.Coroutine.Timer
{
	internal struct TimerOptions
	{
		internal String Name;
		internal Double Amount;
		internal Coroutines.Coroutine.Continuation Continuation;
	}
}
