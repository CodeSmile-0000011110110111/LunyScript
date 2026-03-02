using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Concrete implementation of block-level profiling for LunyScript execution.
	/// Tracks execution time for each sequence/block with configurable rolling average.
	/// Public methods use [Conditional] attributes - completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	public sealed class ScriptBlockProfiler
	{
		private readonly Dictionary<ScriptBlockId, ScriptBlockMetrics> _metrics = new();
		private readonly Dictionary<ScriptBlockId, Stopwatch> _activeBlocks = new();
		private Int32 _rollingAverageWindow = 60;

		public Int32 RollingAverageWindow
		{
			get => _rollingAverageWindow;
			set => _rollingAverageWindow = Math.Max(1, value); // Clamp to minimum 1
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		public void BeginBlock(ScriptBlockId scriptBlockId)
		{
#if DEBUG || LUNYSCRIPT_DEBUG || LUNYSCRIPT_PROFILE
			if (!_activeBlocks.TryGetValue(scriptBlockId, out var sw))
			{
				sw = new Stopwatch();
				_activeBlocks[scriptBlockId] = sw;
			}
			sw.Restart();
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")] [Conditional("LUNYSCRIPT_PROFILE")]
		public void EndBlock(ScriptBlockId scriptBlockId, Type blockType)
		{
#if DEBUG || LUNYSCRIPT_DEBUG || LUNYSCRIPT_PROFILE
			if (!_activeBlocks.TryGetValue(scriptBlockId, out var sw))
				return;

			sw.Stop();
			var elapsed = sw.Elapsed.TotalMilliseconds;

			if (!_metrics.TryGetValue(scriptBlockId, out var metrics))
			{
				metrics = new ScriptBlockMetrics
				{
					ScriptBlockId = scriptBlockId,
					BlockType = blockType,
				};
				_metrics[scriptBlockId] = metrics;
			}

			UpdateMetrics(metrics, elapsed);
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")] [Conditional("LUNYSCRIPT_PROFILE")]
		public void RecordError(ScriptBlockId scriptBlockId, Exception ex)
		{
#if DEBUG || LUNYSCRIPT_DEBUG || LUNYSCRIPT_PROFILE
			if (_metrics.TryGetValue(scriptBlockId, out var metrics))
				metrics.ErrorCount++;
#endif
		}

		public ScriptBlockProfilerSnapshot TakeSnapshot()
		{
#if DEBUG || LUNYSCRIPT_DEBUG || LUNYSCRIPT_PROFILE
			return new ScriptBlockProfilerSnapshot
			{
				BlockMetrics = _metrics.Values.ToList(),
				Timestamp = DateTime.UtcNow,
			};
#else
			return default;
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")] [Conditional("LUNYSCRIPT_PROFILE")]
		public void Reset()
		{
#if DEBUG || LUNYSCRIPT_DEBUG || LUNYSCRIPT_PROFILE
			_metrics.Clear();
			_activeBlocks.Clear();
#endif
		}

		private void UpdateMetrics(ScriptBlockMetrics metrics, Double newSample)
		{
			metrics.CallCount++;
			metrics.TotalMs += newSample;

			// Rolling average: disabled if window <= 1
			if (_rollingAverageWindow <= 1)
				metrics.AverageMs = newSample; // No averaging, just use current sample
			else
			{
				// Simple rolling average calculation
				var window = Math.Min(_rollingAverageWindow, metrics.CallCount);
				metrics.AverageMs = (metrics.AverageMs * (window - 1) + newSample) / window;
			}

			// Update min/max
			if (metrics.CallCount == 1)
			{
				metrics.MinMs = newSample;
				metrics.MaxMs = newSample;
			}
			else
			{
				metrics.MinMs = Math.Min(metrics.MinMs, newSample);
				metrics.MaxMs = Math.Max(metrics.MaxMs, newSample);
			}
		}
	}
}
