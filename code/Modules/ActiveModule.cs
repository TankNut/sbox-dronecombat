using Sandbox;

namespace DroneCombat.Modules
{
	abstract partial class ActiveModule : Module
	{
		[NetLocal]
		public int InventoryIndex { get; set; } = -1;

		public virtual void SetActive() { }
		public virtual void SetInactive() { }
		public virtual void ActiveTick() { }
	}
}
