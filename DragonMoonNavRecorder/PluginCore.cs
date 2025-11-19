using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;

/*
 * DragonMoonNavRecorder - Records player movement and saves navigation routes
 * 
 * Features:
 * - Records player position (x, y, z, landcell, timestamp) in real-time
 * - Background thread recording to never block the client
 * - Hotkey support (F9) to toggle recording
 * - UI buttons for Start/Stop/Save/Clear
 * - Saves routes to .nav files
 */

namespace DragonMoonNavRecorder
{
	//Attaches events from core
	[WireUpBaseEvents]

	//View (UI) handling
	[MVView("DragonMoonNavRecorder.mainView.xml")]
	[MVWireUpControlEvents]

	// FriendlyName is the name that will show up in the plugins list of the decal agent
	[FriendlyName("DragonMoonNavRecorder")]
	public class PluginCore : PluginBase
	{
		// Windows API for key detection
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		private NavRecorder recorder;
		private System.Timers.Timer statusUpdateTimer;
		private bool f9KeyPressed = false;
		private const int VK_F9 = 0x78; // F9 virtual key code

		// UI Control References
		[MVControlReference("StatusLabel")]
		private IStaticText StatusLabel = null;

		[MVControlReference("PointCountLabel")]
		private IStaticText PointCountLabel = null;

		[MVControlReference("InfoLabel")]
		private IStaticText InfoLabel = null;

		/// <summary>
		/// This is called when the plugin is started up. This happens only once.
		/// </summary>
		protected override void Startup()
		{
			try
			{
				// Initialize globals with references to Host and Core
				Globals.Init("DragonMoonNavRecorder", Host, Core);

				// Initialize the recorder
				recorder = new NavRecorder();

				// Initialize the view
				MVWireupHelper.WireupStart(this, Host);

				// Set up render frame handler for hotkey detection
				Core.RenderFrame += new EventHandler<EventArgs>(Core_RenderFrame);

				// Set up command line handler as alternative method
				Core.CommandLineText += new EventHandler<ChatParserInterceptEventArgs>(Core_CommandLineText);

				// Set up status update timer (updates UI every second)
				statusUpdateTimer = new System.Timers.Timer(1000);
				statusUpdateTimer.Elapsed += new ElapsedEventHandler(StatusUpdateTimer_Elapsed);
				statusUpdateTimer.Start();

				// Update initial UI state
				UpdateUI();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// This is called when the plugin is shut down. This happens only once.
		/// </summary>
		protected override void Shutdown()
		{
			try
			{
				// Stop recording if active
				if (recorder != null && recorder.IsRecording)
				{
					recorder.StopRecording();
				}

				// Stop and dispose timer
				if (statusUpdateTimer != null)
				{
					statusUpdateTimer.Stop();
					statusUpdateTimer.Dispose();
				}

				// Unsubscribe from events
				if (Core != null)
				{
					Core.RenderFrame -= new EventHandler<EventArgs>(Core_RenderFrame);
					Core.CommandLineText -= new EventHandler<ChatParserInterceptEventArgs>(Core_CommandLineText);
				}

				// Destroy the view
				MVWireupHelper.WireupEnd(this);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// Handles render frame events to detect F9 key press
		/// </summary>
		private void Core_RenderFrame(object sender, EventArgs e)
		{
			try
			{
				// Check if F9 is currently pressed using Windows API
				// GetAsyncKeyState returns negative value if key is currently pressed
				bool isF9Pressed = (GetAsyncKeyState(VK_F9) & 0x8000) != 0;

				if (isF9Pressed && !f9KeyPressed)
				{
					f9KeyPressed = true;
					ToggleRecording();
				}
				else if (!isF9Pressed)
				{
					f9KeyPressed = false;
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// Handles command line text for alternative control method
		/// Users can type "/navrecord" or "/navtoggle" to control recording
		/// </summary>
		private void Core_CommandLineText(object sender, ChatParserInterceptEventArgs e)
		{
			try
			{
				string text = e.Text.ToLower().Trim();
				
				if (text == "/navtoggle" || text == "/navrecord")
				{
					e.Eat = true; // Consume the command
					ToggleRecording();
				}
				else if (text == "/navstart")
				{
					e.Eat = true;
					if (!recorder.IsRecording)
					{
						recorder.StartRecording();
						Util.WriteToChat("Recording started via command.");
						UpdateUI();
					}
				}
				else if (text == "/navstop")
				{
					e.Eat = true;
					if (recorder.IsRecording)
					{
						recorder.StopRecording();
						Util.WriteToChat("Recording stopped via command.");
						UpdateUI();
					}
				}
				else if (text == "/navsave")
				{
					e.Eat = true;
					SaveRoute();
				}
				else if (text == "/navclear")
				{
					e.Eat = true;
					if (recorder.IsRecording)
					{
						Util.WriteToChat("Cannot clear while recording. Stop recording first.");
					}
					else
					{
						int pointCount = recorder.PointCount;
						recorder.ClearPoints();
						Util.WriteToChat("Cleared " + pointCount + " recorded points.");
						UpdateUI();
					}
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// Toggles recording on/off
		/// </summary>
		private void ToggleRecording()
		{
			try
			{
				if (recorder.IsRecording)
				{
					recorder.StopRecording();
					Util.WriteToChat("Recording stopped. Points recorded: " + recorder.PointCount);
				}
				else
				{
					recorder.StartRecording();
					Util.WriteToChat("Recording started. Press F9 again to stop.");
				}
				UpdateUI();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// Updates the UI with current status
		/// </summary>
		private void UpdateUI()
		{
			try
			{
				if (StatusLabel != null)
				{
					StatusLabel.Text = "Status: " + (recorder.IsRecording ? "Recording..." : "Stopped");
				}

				if (PointCountLabel != null)
				{
					PointCountLabel.Text = "Points Recorded: " + recorder.PointCount;
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// Timer event that updates UI status periodically
		/// </summary>
		private void StatusUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				UpdateUI();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("LoginComplete", "CharacterFilter")]
		private void CharacterFilter_LoginComplete(object sender, EventArgs e)
		{
			try
			{
				Util.WriteToChat("DragonMoonNavRecorder loaded. Press F9 to toggle recording.");
				UpdateUI();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("Logoff", "CharacterFilter")]
		private void CharacterFilter_Logoff(object sender, Decal.Adapter.Wrappers.LogoffEventArgs e)
		{
			try
			{
				// Stop recording on logoff
				if (recorder != null && recorder.IsRecording)
				{
					recorder.StopRecording();
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		// UI Button Event Handlers

		[MVControlEvent("StartRecording", "Click")]
		void StartRecording_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				if (!recorder.IsRecording)
				{
					recorder.StartRecording();
					Util.WriteToChat("Recording started. Points recorded: " + recorder.PointCount);
					UpdateUI();
				}
				else
				{
					Util.WriteToChat("Recording is already in progress.");
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("StopRecording", "Click")]
		void StopRecording_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				if (recorder.IsRecording)
				{
					recorder.StopRecording();
					Util.WriteToChat("Recording stopped. Total points recorded: " + recorder.PointCount);
					UpdateUI();
				}
				else
				{
					Util.WriteToChat("Recording is not active.");
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("SaveRoute", "Click")]
		void SaveRoute_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				SaveRoute();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// Saves the current route to a file
		/// </summary>
		private void SaveRoute()
		{
			try
			{
				if (recorder.PointCount == 0)
				{
					Util.WriteToChat("No points to save. Start recording first!");
					return;
				}

				// Generate filename with timestamp
				string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
				string fileName = "NavRoute_" + timestamp + ".nav";
				string directory = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.Personal),
					@"Asheron's Call\NavRoutes"
				);
				string filePath = Path.Combine(directory, fileName);

				if (recorder.SaveToFile(filePath))
				{
					Util.WriteToChat("Route saved successfully!");
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("ClearRoute", "Click")]
		void ClearRoute_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				if (recorder.IsRecording)
				{
					Util.WriteToChat("Cannot clear while recording. Stop recording first.");
					return;
				}

				int pointCount = recorder.PointCount;
				recorder.ClearPoints();
				Util.WriteToChat("Cleared " + pointCount + " recorded points.");
				UpdateUI();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}
	}
}
