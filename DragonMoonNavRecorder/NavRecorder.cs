using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace DragonMoonNavRecorder
{
	/// <summary>
	/// Represents a single navigation point with coordinates, landcell, and timestamp
	/// </summary>
	public class NavPoint
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
		public int Landcell { get; set; }
		public DateTime Timestamp { get; set; }

		public NavPoint(double x, double y, double z, int landcell, DateTime timestamp)
		{
			X = x;
			Y = y;
			Z = z;
			Landcell = landcell;
			Timestamp = timestamp;
		}
	}

	/// <summary>
	/// Handles background recording of player movement using a separate thread
	/// </summary>
	public class NavRecorder
	{
		private List<NavPoint> recordedPoints;
		private bool isRecording;
		private Thread recordingThread;
		private readonly object lockObject = new object();
		private int recordingIntervalMs = 100; // Record every 100ms by default

		public NavRecorder()
		{
			recordedPoints = new List<NavPoint>();
			isRecording = false;
		}

		/// <summary>
		/// Gets the current number of recorded points
		/// </summary>
		public int PointCount
		{
			get
			{
				lock (lockObject)
				{
					return recordedPoints.Count;
				}
			}
		}

		/// <summary>
		/// Gets whether recording is currently active
		/// </summary>
		public bool IsRecording
		{
			get
			{
				lock (lockObject)
				{
					return isRecording;
				}
			}
		}

		/// <summary>
		/// Starts recording player movement in a background thread
		/// </summary>
		public void StartRecording()
		{
			lock (lockObject)
			{
				if (isRecording)
					return;

				isRecording = true;
				recordingThread = new Thread(RecordingLoop)
				{
					IsBackground = true,
					Name = "NavRecorderThread"
				};
				recordingThread.Start();
			}
		}

		/// <summary>
		/// Stops recording player movement
		/// </summary>
		public void StopRecording()
		{
			lock (lockObject)
			{
				if (!isRecording)
					return;

				isRecording = false;
			}

			// Wait for thread to finish (with timeout)
			if (recordingThread != null && recordingThread.IsAlive)
			{
				if (!recordingThread.Join(1000))
				{
					// Thread didn't finish in time, but that's okay - it's a background thread
				}
			}
		}

		/// <summary>
		/// Background thread loop that records player position at regular intervals
		/// </summary>
		private void RecordingLoop()
		{
			try
			{
				while (true)
				{
					lock (lockObject)
					{
						if (!isRecording)
							break;
					}

					// Record current position
					try
					{
						if (Globals.Core != null && Globals.Core.CharacterFilter != null)
						{
							WorldObject player = Globals.Core.WorldFilter[Globals.Core.CharacterFilter.Id];
							if (player != null)
							{
								double x = player.Coordinates().X;
								double y = player.Coordinates().Y;
								double z = player.Coordinates().Z;
								int landcell = player.Values(LongValueKey.Landcell);

								NavPoint point = new NavPoint(x, y, z, landcell, DateTime.Now);

								lock (lockObject)
								{
									recordedPoints.Add(point);
								}
							}
						}
					}
					catch (Exception ex)
					{
						Util.LogError(ex);
					}

					// Sleep for the recording interval
					Thread.Sleep(recordingIntervalMs);
				}
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
			}
		}

		/// <summary>
		/// Clears all recorded points
		/// </summary>
		public void ClearPoints()
		{
			lock (lockObject)
			{
				recordedPoints.Clear();
			}
		}

		/// <summary>
		/// Gets a copy of all recorded points
		/// </summary>
		public List<NavPoint> GetPoints()
		{
			lock (lockObject)
			{
				return new List<NavPoint>(recordedPoints);
			}
		}

		/// <summary>
		/// Saves recorded points to a .nav file
		/// </summary>
		/// <param name="filePath">Full path to the .nav file to save</param>
		/// <returns>True if save was successful, false otherwise</returns>
		public bool SaveToFile(string filePath)
		{
			try
			{
				List<NavPoint> points;
				lock (lockObject)
				{
					points = new List<NavPoint>(recordedPoints);
				}

				if (points.Count == 0)
				{
					Util.WriteToChat("No points to save!");
					return false;
				}

				// Ensure directory exists
				string directory = Path.GetDirectoryName(filePath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				// Write points to file
				using (StreamWriter writer = new StreamWriter(filePath))
				{
					writer.WriteLine("# DragonMoonNavRecorder Navigation File");
					writer.WriteLine("# Format: X, Y, Z, Landcell, Timestamp");
					writer.WriteLine("# Total Points: " + points.Count);
					writer.WriteLine();

					foreach (NavPoint point in points)
					{
						writer.WriteLine(string.Format("{0:F6},{1:F6},{2:F6},{3},{4:yyyy-MM-dd HH:mm:ss.fff}",
							point.X, point.Y, point.Z, point.Landcell, point.Timestamp));
					}
				}

				Util.WriteToChat(string.Format("Saved {0} points to {1}", points.Count, filePath));
				return true;
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
				Util.WriteToChat("Error saving file: " + ex.Message);
				return false;
			}
		}
	}
}
