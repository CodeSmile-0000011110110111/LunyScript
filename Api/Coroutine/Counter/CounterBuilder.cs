using System;

namespace LunyScript.Api.Coroutine.Counter
{
	/// <summary>
	/// Fluent builder for counter coroutines.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	///        Counter("name").Every(10).Heartbeats().Do(blocks);
	/// </summary>
	public readonly struct CounterBuilder<T> where T : struct, ICounterBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CounterOptions Options;

		internal CounterBuilder(Script script, BuilderToken token, in CounterOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static CounterBuilder<CounterBuilderStart> Create(Script script, String name)
		{
			if (script == null)
				throw new ArgumentNullException(nameof(script));
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Counter name is null or empty", nameof(name));

			var options = new CounterOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Counter");
			return new CounterBuilder<CounterBuilderStart>(script, token, in options);
		}
	}
}
