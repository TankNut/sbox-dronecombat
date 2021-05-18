using Sandbox;

namespace DroneCombat.Projectiles
{
	abstract class Projectile : ModelEntity, IPhysicsUpdate
	{
		public virtual float Speed => 100.0f;
		public virtual float Size => 3.0f;

		public bool HasHit { get; set; } = false;

		public virtual void OnPostPhysicsStep( float delta )
		{
			if ( IsClient )
				return;

			Vector3 velocity = Rotation.Forward * Speed * delta;

			TraceResult trace = Trace.Ray( Position, Position + velocity )
				.WorldAndEntities()
				.HitLayer( CollisionLayer.SKY, true )
				.Ignore( Owner )
				.Ignore( this )
				.Size( Size )
				.Run();

			Process( trace );

			Position = trace.EndPos;

			if ( trace.Hit && !HasHit )
				Hit( trace );
		}

		public virtual void Process( TraceResult trace )
		{
		}

		public virtual void OnHit( TraceResult trace )
		{
		}

		public virtual void Hit( TraceResult trace )
		{
			HasHit = true;

			OnHit( trace );

			Delete();
		}
	}
}
