using Sandbox;

namespace DroneCombat.Modules
{
	abstract class Module : ModelEntity
	{
		public virtual void AddToInventory( DroneInventory inventory )
		{
			SetParent( inventory.Owner, true );

			Owner = inventory.Owner;
			LocalPos = Vector3.Zero;
			LocalRot = Rotation.Identity;

			Activate();
		}

		protected virtual void Activate() { }
		protected virtual void Deactivate() { }
		protected override void OnDestroy() => Deactivate();
		public virtual void Tick() { }
	}
}
