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
			Vector3 distance = pos - Local.Pawn.Position;

			return distance.Length / 4000.0f;
		}

		public override void Tick()
		{
			DronePawn target = (Local.Pawn as DronePawn).Target;

			bool active = target != null;

			SetClass( "active", active );

			if ( active )
			{
				PositionAtWorld( target.Position );

				Vector3 start = target.Position;
				Vector3 lead = target.Position + (target.Velocity * TimeToTarget( target.Position ));

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
