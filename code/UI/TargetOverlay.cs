using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DroneCombat.UI
{
	class TargetOverlay : Panel
	{
		public TargetOverlay()
		{
			StyleSheet.Load( "/UI/TargetOverlay.scss" );
		}

		public override void Tick()
		{
			DronePawn target = (Local.Pawn as DronePawn).Target;

			bool active = target != null;

			SetClass( "active", active );

			if ( active )
			{
				PositionAtWorld( target.Position );

				BBox box = (target.GetActiveController() as DroneController).GetHull();

				List<Vector2> points = new()
				{
					target.Transform.PointToWorld( new Vector3( box.Maxs.x, box.Maxs.y, box.Maxs.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Maxs.x, box.Mins.y, box.Maxs.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Maxs.x, box.Maxs.y, box.Mins.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Maxs.x, box.Mins.y, box.Mins.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Mins.x, box.Maxs.y, box.Maxs.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Mins.x, box.Mins.y, box.Maxs.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Mins.x, box.Maxs.y, box.Mins.z ) ).ToScreen(),
					target.Transform.PointToWorld( new Vector3( box.Mins.x, box.Mins.y, box.Mins.z ) ).ToScreen()
				};

				Vector2 mins = new Vector2( points[1].x, points[1].y );
				Vector2 maxs = new Vector2( points[1].x, points[1].y );

				foreach ( Vector2 point in points )
				{
					mins.x = Math.Min( mins.x, point.x );
					mins.y = Math.Min( mins.y, point.y );

					maxs.x = Math.Max( maxs.x, point.x );
					maxs.y = Math.Max( maxs.y, point.y );
				}

				Style.Width = Length.Fraction( maxs.x - mins.x );
				Style.Height = Length.Fraction( maxs.y - mins.y );
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
