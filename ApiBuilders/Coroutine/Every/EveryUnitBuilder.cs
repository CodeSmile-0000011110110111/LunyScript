using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Coroutine.Every
{
	/// <summary>
	/// Builder step after unit (Frames/Heartbeats) is selected.
	/// </summary>
	public readonly struct EveryUnitBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly EveryOptions _options;

		internal EveryUnitBuilder(Script script, BuilderToken token, EveryOptions options)
		{
			_script = script;
			_token = token;
			_options = options;

			if (options.Interval < 0)
				throw new ArgumentException($"Every duration must be 0 or greater, got: {options.Interval}");

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() =>
			{
				var coroutineOptions = CoroutineOptions.ForEveryInterval(null, capturedOptions.Interval, capturedOptions.Delay, capturedOptions.Process, null);
				CoroutineBuilder.Finalize(capturedScript, in coroutineOptions, token);
			});
		}

		/// <summary>
		/// Sets the phase offset (delay) for time-sliced execution.
		/// </summary>
		public EveryUnitBuilder DelayBy(Int32 delay)
		{
			if (_options.Delay != 0)
				throw new ArgumentException($"{nameof(DelayBy)}() can't be used twice");

			var options = _options;
			options.Delay = delay;
			return new EveryUnitBuilder(_script, _token, options);
		}

		/// <summary>
		/// Completes the builder and specifies blocks to run.
		/// </summary>
		public ICounterCoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			// name = null => generates a unique name for a time-sliced coroutine
			var coroutineOptions = CoroutineOptions.ForEveryInterval(null, _options.Interval, _options.Delay, _options.Process, blocks);
			return (ICounterCoroutineBlock)CoroutineBuilder.Finalize(_script, in coroutineOptions, _token);
		}
	}
}
