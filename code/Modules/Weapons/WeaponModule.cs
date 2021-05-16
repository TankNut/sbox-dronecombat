using Sandbox;

namespace DroneCombat.Modules.Weapons
{
	partial class WeaponModule : ActiveModule
	{
		public virtual bool Automatic => false;

		public virtual bool CanAttack() => true;
		public virtual void Attack() { }

		public virtual float TraceSize => 3.0f;

		public virtual Vector3 GetOrigin() => Owner.WorldPos;

		public override void ActiveTick()
		{
			bool wantToAttack = Automatic ? Owner.Input.Down( InputButton.Attack1 ) : Owner.Input.Pressed( InputButton.Attack1 );

			if ( wantToAttack && CanAttack() )
				Attack();

			if ( HasLocalPlayerOwner && UseCrosshair )
				UpdateCrosshair();
		}

		public override void SetInactive() => DestroyCrosshair( Owner );
		protected override void Deactivate() => DestroyCrosshair( Owner );

		public TraceResult DoTrace( Vector3 start )
		{
			bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

			return (Owner as DronePlayer).GetTrace( start )
				.HitLayer( CollisionLayer.Water, !InWater )
				.Size( TraceSize )
				.Run();
		}
	}
}
