using Sandbox;

namespace DroneCombat
{
	class DroneCamera : Camera
	{
		public static float Distance => 40.0f;
		public static float LagDistance => 20.0f;
		public static float RollMultiplier => 0.25f;

		public override void Update()
		{
			var pawn = Local.Pawn as DronePawn;
			var controller = pawn.GetActiveController() as DroneController;

			Pos = pawn.Position + Vector3.Up * pawn.CollisionBounds.Maxs.z * pawn.Scale;
			Pos -= pawn.Velocity / controller.BaseSpeed * LagDistance;

			Rot = pawn.EyeRot;
			Rot *= Rotation.FromRoll( pawn.Rotation.Angles().roll * RollMultiplier );

			Vector3 targetPos = Pos + pawn.EyeRot.Forward * -Distance;

			var tr = Trace.Ray( Pos, targetPos )
				.Ignore( pawn )
				.Radius( 8 )
				.Run();

			Pos = tr.EndPos;

			FieldOfView = 70;

			Viewer = null;
		}
	}
}
