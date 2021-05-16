using Sandbox;

namespace DroneCombat.Projectiles
{
	abstract partial class BulletProjectile : Projectile
	{
		public virtual float Damage => 5.0f;

		public override void OnHit( TraceResult trace )
		{
			if ( trace.Entity.IsValid() )
			{
				DamageInfo info = DamageInfo.FromBullet( trace.EndPos, trace.Direction * 200, Damage )
					.UsingTraceResult( trace )
					.WithAttacker( Owner )
					.WithWeapon( this );

				trace.Entity.TakeDamage( info );
			}

			trace.Surface.DoBulletImpact( trace );
		}
	}
}
