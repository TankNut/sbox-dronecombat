using Sandbox;
using Sandbox.UI;
using System;

namespace DroneCombat.UI
{
	class LeadIndicator : Panel
	{
		public LeadIndicator()
		{
			StyleSheet.Load( "/UI/LeadIndicator.scss" );
		}

		public float TimeToTarget( Vector3 pos )
		{
			Vector3 distance = pos - Player.Local.WorldPos;

			return distance.Length / 4000.0f;
		}

		public override void Tick()
		{
			DronePlayer target = (Player.Local as DronePlayer).Target;

			bool active = target != null;

			SetClass( "active", active );

			if ( active )
			{
				PositionAtWorld( target.WorldPos );

				Vector3 start = target.WorldPos;
				Vector3 lead = target.WorldPos + (target.Velocity * TimeToTarget( target.WorldPos ));

				Vector2 screen = lead.ToScreen() - start.ToScreen();

				Style.Width = Length.Fraction( screen.Length );

				float rot = MathF.Atan2( screen.y, screen.x ).RadianToDegree();

				PanelTransform transform = new PanelTransform();

				transform.AddRotation( 0, 0, rot );

				Style.Transform = transform;
				Style.Dirty();
			}
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
