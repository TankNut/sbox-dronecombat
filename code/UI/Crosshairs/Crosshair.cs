using DroneCombat.Modules.Weapons;
using Sandbox;
using Sandbox.UI;

namespace DroneCombat.UI.Crosshairs
{
	class Crosshair : Panel
	{
		public Crosshair()
		{
			StyleSheet.Load( "/UI/Crosshairs/Crosshair.scss" );
		}

		public override void Tick()
		{
			DronePawn player = Local.Pawn as DronePawn;

			if ( player.ActiveChild is not WeaponModule module )
				return;

			TraceResult trace = (module.Owner as DronePawn).GetTrace()
				.Size( module.TraceSize )
				.Run();

			PositionAtWorld( trace.EndPos );

			PanelTransform transform = new();

			transform.AddTranslateX( Length.Fraction( -0.5f ) );
			transform.AddTranslateY( Length.Fraction( -0.5f ) );

			float roll = player.Rotation.Angles().roll - CurrentView.Rotation.Angles().roll;

			transform.AddRotation( 0, 0, roll );

			Style.Transform = transform;
			Style.Dirty();
		}

		public void PositionAtWorld( Vector3 pos )
		{
			var screenpos = pos.ToScreen();

			if ( screenpos.z < 0 )
				return;

			Style.Left = Length.Fraction( screenpos.x );
			Style.Top = Length.Fraction( screenpos.y );
			Style.Dirty();
		}
	}
}
