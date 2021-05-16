using Crosshairs = DroneCombat.UI.Crosshairs;
using Sandbox;

namespace DroneCombat.Modules.Weapons
{
	partial class WeaponModule
	{
		public virtual bool UseCrosshair => false;

		public Crosshairs.Crosshair Crosshair { get; set; }

		public virtual Crosshairs.Crosshair CreateCrosshair()
		{
			return Hud.Current.RootPanel.AddChild<Crosshairs.Crosshair>();
		}

		public void UpdateCrosshair()
		{
			if ( Crosshair == null )
				Crosshair = CreateCrosshair();
		}

		[ClientRpc]
		public virtual void DestroyCrosshair()
		{
			Crosshair?.Delete();
		}
	}
}
