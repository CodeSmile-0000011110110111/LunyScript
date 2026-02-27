using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Coroutines
{
	internal static class CoroutineEventsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean Has(this Coroutine.Events events, Coroutine.Events flag) => (events & flag) != 0;
	}
}
