using System;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;

namespace NavRecorder
{
	//Attaches events from core
	[WireUpBaseEvents]

	//View (UI) handling
	[MVView("NavRecorder.mainView.xml")]
	[MVWireUpControlEvents]

	// FriendlyName is the name that will show up in the plugins list of the decal agent
	[FriendlyName("NavRecorder")]
	public class PluginCore : PluginBase
	{
		private NavRecorder recorder = new NavRecorder();

		/// <summary>
		/// This is called when the plugin is started up. This happens only once.
		/// </summary>
		protected override void Startup()
		{
			try
			{
				// This initializes our static Globals class with references to the key objects your plugin will use, Host and Core.
				Globals.Init("NavRecorder", Host, Core);

				//Initialize the view.
				MVWireupHelper.WireupStart(this, Host);
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
				if (recorder.IsRecording)
				{
					recorder.StopRecording();
				}

				// Unsubscribe from events
				UnsubscribeEvents();

				//Destroy the view.
				MVWireupHelper.WireupEnd(this);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("LoginComplete", "CharacterFilter")]
		private void CharacterFilter_LoginComplete(object sender, EventArgs e)
		{
			try
			{
				Util.WriteToChat("NavRecorder plugin loaded.");

				// Subscribe to events
				SubscribeEvents();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("Logoff", "CharacterFilter")]
		private void CharacterFilter_Logoff(object sender, Decal.Adapter.Wrappers.LogoffEventArgs e)
		{
			try
			{
				// Stop recording if active
				if (recorder.IsRecording)
				{
					recorder.StopRecording();
				}

				// Unsubscribe to events
				UnsubscribeEvents();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		private void SubscribeEvents()
		{
			try
			{
				Globals.Core.CharacterFilter.Movement += CharacterFilter_Movement;
				Globals.Core.WorldFilter.ChangeObject += WorldFilter_ChangeObject;
				Globals.Host.Actions.ChatMessageReceived += Actions_ChatMessageReceived;
				Globals.Host.Actions.ItemUsed += Actions_ItemUsed;
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		private void UnsubscribeEvents()
		{
			try
			{
				if (Globals.Core?.CharacterFilter != null)
					Globals.Core.CharacterFilter.Movement -= CharacterFilter_Movement;
				if (Globals.Core?.WorldFilter != null)
					Globals.Core.WorldFilter.ChangeObject -= WorldFilter_ChangeObject;
				if (Globals.Host?.Actions != null)
				{
					Globals.Host.Actions.ChatMessageReceived -= Actions_ChatMessageReceived;
					Globals.Host.Actions.ItemUsed -= Actions_ItemUsed;
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		private void CharacterFilter_Movement(object sender, MovementEventArgs e)
		{
			try
			{
				recorder.OnMovement();
				UpdateStatus();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		private void WorldFilter_ChangeObject(object sender, ChangeObjectEventArgs e)
		{
			try
			{
				// Detect portal usage
				if (e.Change == WorldChangeType.RemoveObject)
				{
					WorldObject obj = e.Changed;
					if (obj != null && obj.ObjectClass == ObjectClass.Portal)
					{
						recorder.OnPortalUse(obj.Name);
						UpdateStatus();
					}
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		private void Actions_ChatMessageReceived(object sender, ChatTextInterceptEventArgs e)
		{
			try
			{
				if (e.Text != null)
				{
					recorder.OnChatMessage(e.Text);
					UpdateStatus();
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		private void Actions_ItemUsed(object sender, UseItemEventArgs e)
		{
			try
			{
				if (e.Target != 0)
				{
					WorldObject obj = Globals.Core.WorldFilter[e.Target];
					if (obj != null)
					{
						recorder.OnClick(e.Target, obj.Name);
						UpdateStatus();
					}
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("StartButton", "Click")]
		void StartButton_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				recorder.StartRecording();
				UpdateStatus();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("StopButton", "Click")]
		void StopButton_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				recorder.StopRecording();
				UpdateStatus();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlReference("StatusText")]
		private IStaticText StatusText = null;

		[MVControlReference("StatsText")]
		private IStaticText StatsText = null;

		private void UpdateStatus()
		{
			try
			{
				if (StatusText != null)
				{
					StatusText.Text = "Status: " + (recorder.IsRecording ? "Recording" : "Stopped");
				}

				if (StatsText != null)
				{
					StatsText.Text = $"Waypoints: {recorder.WaypointCount}\nEvents: {recorder.EventCount}";
				}
			}
			catch (Exception ex) { Util.LogError(ex); }
		}
	}
}
