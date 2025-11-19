using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace NavRecorder
{
	public class NavRecorder
	{
		private bool isRecording = false;
		private List<WaypointData> waypoints = new List<WaypointData>();
		private List<EventData> events = new List<EventData>();
		private double lastX = 0;
		private double lastY = 0;
		private double lastZ = 0;

		public bool IsRecording
		{
			get { return isRecording; }
		}

		public int WaypointCount
		{
			get { return waypoints.Count; }
		}

		public int EventCount
		{
			get { return events.Count; }
		}

		public void StartRecording()
		{
			if (isRecording)
				return;

			isRecording = true;
			waypoints.Clear();
			events.Clear();

			// Record initial position
			try
			{
				if (Globals.Core.CharacterFilter != null)
				{
					lastX = Globals.Core.CharacterFilter.X;
					lastY = Globals.Core.CharacterFilter.Y;
					lastZ = Globals.Core.CharacterFilter.Z;

					waypoints.Add(new WaypointData
					{
						x = lastX,
						y = lastY,
						z = lastZ,
						type = "move"
					});
				}
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
			}

			Util.WriteToChat("Recording started");
		}

		public void StopRecording()
		{
			if (!isRecording)
				return;

			isRecording = false;
			SaveToJson();
			Util.WriteToChat("Recording stopped. Data saved.");
		}

		public void OnMovement()
		{
			if (!isRecording)
				return;

			try
			{
				if (Globals.Core.CharacterFilter != null)
				{
					double currentX = Globals.Core.CharacterFilter.X;
					double currentY = Globals.Core.CharacterFilter.Y;
					double currentZ = Globals.Core.CharacterFilter.Z;

					// Only record if position changed significantly (threshold to avoid spam)
					double threshold = 0.1;
					if (Math.Abs(currentX - lastX) > threshold ||
						Math.Abs(currentY - lastY) > threshold ||
						Math.Abs(currentZ - lastZ) > threshold)
					{
						lastX = currentX;
						lastY = currentY;
						lastZ = currentZ;

						waypoints.Add(new WaypointData
						{
							x = currentX,
							y = currentY,
							z = currentZ,
							type = "move"
						});
					}
				}
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
			}
		}

		public void OnClick(int objectId, string objectName = null)
		{
			if (!isRecording)
				return;

			try
			{
				string name = objectName;
				if (string.IsNullOrEmpty(name))
				{
					WorldObject obj = Globals.Core.WorldFilter[objectId];
					if (obj != null)
					{
						name = obj.Name ?? "Unknown";
					}
					else
					{
						name = "Unknown";
					}
				}

				events.Add(new EventData
				{
					type = "click",
					name = name
				});
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
			}
		}

		public void OnPortalUse(string portalName)
		{
			if (!isRecording)
				return;

			try
			{
				events.Add(new EventData
				{
					type = "portal",
					name = portalName ?? "Unknown"
				});
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
			}
		}

		public void OnChatMessage(string message)
		{
			if (!isRecording)
				return;

			try
			{
				events.Add(new EventData
				{
					type = "chat",
					name = message ?? ""
				});
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
			}
		}

		private void SaveToJson()
		{
			try
			{
				// Output to RecorderOutput directory in user's Documents/Asheron's Call folder
				string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Asheron's Call", "RecorderOutput");
				if (!Directory.Exists(outputDir))
				{
					Directory.CreateDirectory(outputDir);
				}

				string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
				string filename = Path.Combine(outputDir, $"route_{timestamp}.json");

				StringBuilder json = new StringBuilder();
				json.AppendLine("{");
				json.AppendLine("  \"waypoints\": [");

				for (int i = 0; i < waypoints.Count; i++)
				{
					WaypointData wp = waypoints[i];
					json.Append("    { \"x\": ");
					json.Append(wp.x.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
					json.Append(", \"y\": ");
					json.Append(wp.y.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
					json.Append(", \"z\": ");
					json.Append(wp.z.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
					json.Append(", \"type\": \"");
					json.Append(wp.type);
					json.Append("\" }");
					if (i < waypoints.Count - 1)
						json.Append(",");
					json.AppendLine();
				}

				json.AppendLine("  ],");
				json.AppendLine("  \"events\": [");

				for (int i = 0; i < events.Count; i++)
				{
					EventData evt = events[i];
					json.Append("    { \"type\": \"");
					json.Append(evt.type);
					json.Append("\", \"name\": \"");
					json.Append(EscapeJsonString(evt.name));
					json.Append("\" }");
					if (i < events.Count - 1)
						json.Append(",");
					json.AppendLine();
				}

				json.AppendLine("  ]");
				json.AppendLine("}");

				File.WriteAllText(filename, json.ToString());
				Util.WriteToChat($"Route saved to: {filename}");
			}
			catch (Exception ex)
			{
				Util.LogError(ex);
				Util.WriteToChat("Error saving route: " + ex.Message);
			}
		}

		private string EscapeJsonString(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";

			return input.Replace("\\", "\\\\")
						.Replace("\"", "\\\"")
						.Replace("\n", "\\n")
						.Replace("\r", "\\r")
						.Replace("\t", "\\t");
		}

		private class WaypointData
		{
			public double x;
			public double y;
			public double z;
			public string type;
		}

		private class EventData
		{
			public string type;
			public string name;
		}
	}
}
