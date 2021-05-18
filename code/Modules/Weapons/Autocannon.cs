using DroneCombat.Projectiles;
using Sandbox;

namespace DroneCombat.Modules.Weapons
{
	partial class Autocannon : WeaponModule
	{
		public override bool Automatic => true;
		public override bool UseCrosshair => true;

		public static float RoundsPerMinute => 550.0f;

		[NetPredicted]
		protected bool Side { get; set; }

		[NetPredicted]
		public TimeSince TimeSinceLastAttack { get; set; }

		public override void SetActive()
		{
			base.SetActive();

			TimeSinceLastAttack = 0;
		}

		public override bool CanAttack()
		{
			return TimeSinceLastAttack > (60 / RoundsPerMinute);
		}

		public override void Attack()
		{
			TimeSinceLastAttack = 0;

			PlaySound( "dronecombat.weapon.thudsoft" );

			if ( IsServer )
			{
				using ( Prediction.Off() )
				{
					AutocannonProjectile bullet = new();

					bullet.Position = GetOrigin( Side );
					bullet.Rotation = Owner.EyeRot;
					bullet.Owner = Owner;
				}
			}

			Side = !Side;
		}

		[ClientRpc]
		public void ShootEffects()
		{
			if ( Local.Pawn == Owner )
				_ = new Sandbox.ScreenShake.Perlin( 0.5f, 2.0f, 0.2f );
		}

		public Vector3 GetOrigin( bool side = false )
		{
			var offset = 2.0f;

			return Owner.Transform.PointToWorld( Vector3.Right * (side ? -offset : offset) );
		}
	}
}
