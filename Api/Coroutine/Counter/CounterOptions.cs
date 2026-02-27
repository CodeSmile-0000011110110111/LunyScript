using System;

namespace LunyScript.Api.Coroutine.Counter
{
	public interface ICounterBuilderState {}
	/// <summary>Initial state — next: call <c>In()</c> or <c>Every()</c>.</summary>
	public interface ICounterBuilderStart : ICounterBuilderState {}
	/// <summary>Amount set — next: call <c>Frames()</c> or <c>Heartbeats()</c>.</summary>
	public interface ICounterAmountSet : ICounterBuilderState {}
	/// <summary>Unit chosen — ready to finalize via <c>Do()</c>.</summary>
	public interface ICounterUnitSet : ICounterBuilderState {}

	public struct CounterBuilderStart : ICounterBuilderStart {}
	public struct CounterAmountSet : ICounterAmountSet {}
	public struct CounterUnitSet : ICounterUnitSet {}

	internal struct CounterOptions
	{
		internal String Name;
		internal Int32 Amount;
		internal Coroutines.Coroutine.Continuation Continuation;
		internal Coroutines.Coroutine.Process Process;
	}
}
