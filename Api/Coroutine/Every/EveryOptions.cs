namespace LunyScript.Api.Coroutine.Every
{
	public interface IEveryBuilderState {}
	/// <summary>Initial state — next: call <c>Frames()</c> or <c>Heartbeats()</c>.</summary>
	public interface IEveryBuilderStart : IEveryBuilderState {}
	/// <summary>Unit chosen — ready to finalize via <c>Do()</c> or chain with <c>DelayBy()</c>.</summary>
	public interface IEveryUnitSet : IEveryBuilderState {}

	public struct EveryBuilderStart : IEveryBuilderStart {}
	public struct EveryUnitSet : IEveryUnitSet {}

	internal struct EveryOptions
	{
		internal System.Int32 Interval;
		internal System.Int32 Delay;
		internal Coroutines.Coroutine.Process Process;
	}
}
