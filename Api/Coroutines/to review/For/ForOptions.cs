namespace LunyScript
{
	public interface IForBuilderState {}
	/// <summary>Initial state — next: call <c>Seconds()</c>, <c>Milliseconds()</c>, <c>Minutes()</c>, <c>Frames()</c>, or <c>Heartbeats()</c>.</summary>
	public interface IForAmountSet : IForBuilderState {}
	/// <summary>Shared base for both frame and heartbeat unit states — enables shared lifecycle extension methods.</summary>
	public interface IForReadyUnit : IForBuilderState {}
	/// <summary>Frame-update unit chosen — <c>OnFrameUpdate()</c> available.</summary>
	public interface IForFrameUnit : IForReadyUnit {}
	/// <summary>Heartbeat unit chosen — <c>OnHeartbeat()</c> available.</summary>
	public interface IForHeartbeatUnit : IForReadyUnit {}

	public struct ForAmountSet : IForAmountSet {}
	public struct ForFrameUnit : IForFrameUnit {}
	public struct ForHeartbeatUnit : IForHeartbeatUnit {}
}
