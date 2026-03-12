namespace LunyScript.Api
{
	/*public interface ICoroutineEveryBuilder {}
	public interface ICoroutineEveryBuilderStart : ICoroutineEveryBuilder {}
	public struct CoroutineEveryBuilderStart : ICoroutineEveryBuilderStart {}

	/// <summary>
	/// Fluent builder for time-sliced coroutines.
	/// Usage: Every(3).Frames().Do(blocks);
	///        Every(Even).Heartbeats().DelayBy(1).Do(blocks);
	/// </summary>
	public readonly struct CoroutineEveryBuilder<T> where T : struct, ICoroutineEveryBuilder
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;

		internal CoroutineEveryBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static CoroutineEveryBuilder<CoroutineEveryBuilderStart> Create(Script script, Int32 interval)
		{
			if (interval < 2)
			{
				throw new ArgumentException($"Every({interval}): interval must be at least 2. Interval 1 or 0 would run every " +
				                            "Heartbeat/Update regardless of DelayBy. Use a Counter() if this is intentional.");
			}

			var options = new CoroutineOptions { Duration = interval };
			var token = script.CreateBuilderToken("Every", "Every()");
			return new CoroutineEveryBuilder<CoroutineEveryBuilderStart>(script, token, options);
		}
	}

	public interface ICoroutineEveryUnitSet : ICoroutineEveryBuilder {}
	public struct CoroutineEveryUnitSet : ICoroutineEveryUnitSet {}

	public static class EveryBuilderUnitExtensions
	{
		/// <summary>Selects frame-based execution.</summary>
		public static CoroutineEveryBuilder<CoroutineEveryUnitSet> Frames(this CoroutineEveryBuilder<CoroutineEveryBuilderStart> b) =>
			new(b.Script, b.Token, CoroutineOptions.ForIntervalCoroutine(b.Options.Name, 1, 0, Coroutine.Process.FrameUpdate));

		/// <summary>Selects heartbeat-based execution.</summary>
		public static CoroutineEveryBuilder<CoroutineEveryUnitSet> Heartbeats(this CoroutineEveryBuilder<CoroutineEveryBuilderStart> b) =>
			new(b.Script, b.Token, CoroutineOptions.ForIntervalCoroutine(b.Options.Name, 1, 0, Coroutine.Process.Heartbeat));
	}

	public interface ICoroutineEveryOffsetSet : ICoroutineEveryUnitSet {}
	public struct CoroutineEveryOffsetSet : ICoroutineEveryOffsetSet {}

	public static class EveryBuilderOffsetExtensions
	{
		/// <summary>Sets the phase offset (delay) for time-sliced execution.</summary>
		public static CoroutineEveryBuilder<CoroutineEveryOffsetSet> Offset<T>(this CoroutineEveryBuilder<T> b, Int32 offset)
			where T : struct, ICoroutineEveryUnitSet => new(b.Script, b.Token, b.Options with { TimeSliceOffset = offset });
	}

	public static class EveryBuilderFinalExtensions
	{
		/// <summary>Completes the builder and specifies blocks to run at each interval.</summary>
		public static ICoroutineBlock Do<T>(this CoroutineEveryBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineEveryUnitSet
		{
			var o = b.Options;
			return CoroutineBuilder.Finish(b.Script, b.Token,
				CoroutineOptions.ForIntervalCoroutine(null, o.TimeSliceInterval, o.TimeSliceOffset, o.ProcessMode, blocks));
		}
	}*/
}
