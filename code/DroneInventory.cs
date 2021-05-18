using System.Collections.Generic;
using DroneCombat.Modules;

namespace DroneCombat
{
	class DroneInventory
	{
		public readonly List<Module> Modules = new();
		public readonly List<ActiveModule> ActiveModules = new();

		public DronePawn Owner { get; set; }

		public ActiveModule ActiveModule { get; private set; }

		public DroneInventory( DronePawn player )
		{
			Owner = player;
		}

		public bool Has( Module module ) => Modules.Contains( module );
		public int Count() => Modules.Count;

		public void Add( Module module, bool active = false )
		{
			module.AddToInventory( this );

			Modules.Add( module );

			if ( module is ActiveModule activeModule )
			{
				ActiveModules.Add( activeModule );
				activeModule.InventoryIndex = ActiveModules.IndexOf( activeModule );

				if ( active )
					SetActiveModule( activeModule );
			}
		}

		public void Remove( Module module )
		{
			Modules.Remove( module );

			if ( module is ActiveModule activeModule )
			{
				ActiveModules.Remove( activeModule );

				if ( activeModule == ActiveModule )
					SetActiveModule( null );
			}
		}

		public void Clear()
		{
			Modules.ForEach( ( Module module ) => module.Delete() );

			Modules.Clear();
			ActiveModules.Clear();

			ActiveModule = null;
		}

		public void SetActiveModule( ActiveModule module )
		{
			ActiveModule?.SetInactive();
			ActiveModule = module;

			Owner.ActiveChild = module;

			ActiveModule?.SetActive();
		}
	}
}
