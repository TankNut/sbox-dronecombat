namespace DroneCombat.Projectiles
{
	partial class AutocannonProjectile : BulletProjectile
	{
		public override float Damage => 20.0f;
		public override float Speed => 4000.0f;

		public override void Spawn()
		{
			base.Spawn();

			Scale = 0.1f;

			SetModel( "models/citizen_props/hotdog01.vmdl" );
		}
	}
}
