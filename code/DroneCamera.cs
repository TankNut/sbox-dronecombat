using Sandbox;

namespace DroneCombat
{
	class DroneCamera : BaseCamera
	{
		public static float Distance => 40.0f;
		public static float LagDistance => 20.0f;
		public static float RollMultiplier => 0.25f;

		public override void Update()
		{
			var player = Player.Local;
			var controller = player.GetActiveController() as DroneController;

			Pos = player.WorldPos + Vector3.Up * player.CollisionBounds.Maxs.z;
			Pos -= player.Velocity / controller.BaseSpeed * LagDistance;

			Rot = player.EyeRot;
			Rot *= Rotation.FromRoll( player.WorldRot.Angles().roll * RollMultiplier );

			Vector3 targetPos = Pos + player.EyeRot.Forward * -Distance;

			var tr = Trace.Ray( Pos, targetPos )
				.Ignore( player )
				.Radius( 8 )
				.Run();

			Pos = tr.EndPos;

			FieldOfView = 70;

			Viewer = null;
		}
	}
}
