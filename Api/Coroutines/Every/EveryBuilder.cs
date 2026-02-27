using System;

namespace LunyScript
{
	/// <summary>
	/// Fluent builder for time-sliced coroutines.
	/// Usage: Every(3).Frames().Do(blocks);
	///        Every(Even).Heartbeats().DelayBy(1).Do(blocks);
	/// </summary>
	public readonly struct EveryBuilder<T> where T : struct, IEveryBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly EveryOptions Options;

		internal EveryBuilder(Script script, BuilderToken token, in EveryOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static EveryBuilder<EveryBuilderStart> Create(Script script, Int32 interval)
		{
			if (script == null)
				throw new ArgumentNullException(nameof(script));

			var options = new EveryOptions { Interval = interval };
			var token = script.CreateBuilderToken($"Every({interval})", "EveryBuilder");
			return new EveryBuilder<EveryBuilderStart>(script, token, in options);
		}
	}
}
