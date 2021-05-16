using Sandbox;
using Sandbox.Rcon;
using Sandbox.UI;
using System.Linq;

namespace DroneCombat.UI
{
	class DroneHud : Hud
	{
		public DroneHud()
		{
			if ( !IsClient )
				return;

			RootPanel.AddChild<TargetOverlay>();
			RootPanel.AddChild<LeadIndicator>();

			RootPanel.AddChild<NameTags>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		}
	}
}
