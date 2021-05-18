using Sandbox;

namespace DroneCombat
{
	[Library( "dronecombat", Title = "Drone Combat" )]
	partial class Game : Sandbox.Game
	{

		public Game()
		{
			if ( IsServer )
				_ = new UI.DroneHud();
		}

		public override void ClientJoined( Client cl )
		{
			DronePawn pawn = new();

			pawn.Respawn();

			cl.Pawn = pawn;
		}

		public override void MoveToSpawnpoint( Entity pawn )
		{
			base.MoveToSpawnpoint( pawn );

			pawn.Position += Vector3.Up * 20;
		}
	}
}
