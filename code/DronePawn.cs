using Sandbox;

namespace DroneCombat
{
	partial class DronePawn : Player
	{
		public new DroneInventory Inventory { get; protected set; }

		[Net]
		public DronePawn Target { get; set; }

		public DronePawn()
		{
			Inventory = new( this );
		}

		public override void Spawn()
		{
			Scale = 0.2f;

			SetModel( "entities/drone/drone.vmdl" );
		}

		public override void Respawn()
		{
			LifeState = LifeState.Alive;
			Health = 100;

			Velocity = Vector3.Zero;

			Controller = new DroneController();
			Camera = new DroneCamera();

			CreateHull();

			Game.Current?.MoveToSpawnpoint( this );

			ResetInterpolation();

			EnableAllCollisions = true;
			EnableDrawing = true;

			Inventory.Add( new Modules.Weapons.Autocannon(), true );
		}

		public override void OnKilled()
		{
			base.OnKilled();

			Controller = null;
			Camera = new SpectateRagdollCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;

			Inventory.Clear();

			Target = null;
		}

		public override void Simulate( Client client )
		{
			base.Simulate( client );

			UpdateTarget();

			TickModules();
			TickActiveChild();
		}

		protected void UpdateTarget()
		{
			if ( Input.Pressed( InputButton.Reload ) )
			{
				TraceResult trace = Trace.Ray( EyePos, EyePos + EyeRot.Forward * 5000f )
					.HitLayer( CollisionLayer.Player )
					.Ignore( this )
					.Size( 10.0f )
					.Run();

				Target = trace.Hit ? trace.Entity as DronePawn : null;
			}

			if ( Target != null && Target.LifeState != LifeState.Alive )
				Target = null;
		}

		protected void TickModules()
		{
			foreach ( Entity child in Children )
			{
				if ( child is Modules.Module module )
					module.Tick();
			}
		}

		public Trace GetTrace( Vector3 start )
		{
			return Trace.Ray( start, start + EyeRot.Forward * 5000f )
				.WorldAndEntities()
				.Ignore( this );
		}
		public Trace GetTrace() => GetTrace( EyePos );

		protected void TickActiveChild()
		{
			if ( ActiveChild is Modules.ActiveModule module )
				module.ActiveTick();
		}

		private readonly Vector3[] turbinePositions = new Vector3[]
		{
			new Vector3( -35.37f, 35.37f, 10.0f ),
			new Vector3( 35.37f, 35.37f, 10.0f ),
			new Vector3( 35.37f, -35.37f, 10.0f ),
			new Vector3( -35.37f, -35.37f, 10.0f )
		};

		private float spinAngle;

		[Event( "frame" )]
		public void OnFrame()
		{
			spinAngle += 10000.0f * Time.Delta;
			spinAngle %= 360.0f;

			for ( int i = 0; i < turbinePositions.Length; ++i )
			{
				var transform = Transform.ToWorld( new Transform( turbinePositions[i] * Scale, Rotation.From( new Angles( 0, spinAngle, 0 ) ) ) );

				transform.Scale = Scale;

				SetBoneTransform( i, transform );
			}
		}
	}
}
