using Sandbox;

namespace DroneCombat
{
	[Library( "dronecombat", Title = "Drone Combat" )]
	partial class Game : Sandbox.Game
	{
		public override Player CreatePlayer() => new DronePlayer();

		public Game()
		{
			if ( IsServer )
				_ = new UI.DroneHud();
		}

		public override void PlayerRespawn( Player player )
		{
			base.PlayerRespawn( player );

			player.WorldPos += Vector3.Up * 20;
		}
	}
}
