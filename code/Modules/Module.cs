using Sandbox;

namespace DroneCombat.Modules
{
	abstract class Module : ModelEntity
	{
		public virtual void AddToInventory( DroneInventory inventory )
		{
			SetParent( inventory.Owner, true );

			Owner = inventory.Owner;
			LocalPosition = Vector3.Zero;
			LocalRotation = Rotation.Identity;

			Activate();
		}

		protected virtual void Activate() { }
		protected virtual void Deactivate() { }
		protected override void OnDestroy() => Deactivate();
		public virtual void Tick() { }
	}
}
