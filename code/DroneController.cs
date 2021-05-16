using Sandbox;

namespace DroneCombat
{
	class DroneController : BasePlayerController
	{
		public float StrafeMultiplier => 0.5f;
		public float HeightMultiplier => 0.4f;

		public float BaseSpeed => 500.0f;
		public float BaseAccel => 1.0f;

		public float TurnSpeed => 5f;

		public float MaxTilt => 55f;

		public Vector3 OldVelocity { get; set; } = Vector3.Zero;

		public override void Tick()
		{
			OldVelocity = Velocity;

			BuildWishVelocity();

			ApplyFriction( BaseAccel );

			Accelerate( WishVelocity.Normal, WishVelocity.Length, BaseSpeed, BaseAccel );

			BBox hull = GetHull();

			MoveHelper mover = new( Pos, Velocity );

			mover.Trace = mover.Trace.Size( hull.Mins, hull.Maxs ).HitLayer( CollisionLayer.SKY ).Ignore( Player );
			mover.MaxStandableAngle = 0f;
			mover.TryMove( Time.Delta );

			Pos = mover.Position;
			Velocity = mover.Velocity;

			SetRotation();

			if ( Debug )
				WriteDebugInfo();
		}

		public virtual float GetWishSpeed()
		{
			return BaseSpeed;
		}

		void BuildWishVelocity()
		{
			var up = 0;

			up += Input.Down( InputButton.Jump ) ? 1 : 0;
			up -= Input.Down( InputButton.Run ) ? 1 : 0;

			WishVelocity = new Vector3( Input.Forward, Input.Left * StrafeMultiplier, up * HeightMultiplier );
			WishVelocity *= Rotation.LookAt( Rot.Forward.WithZ( 0 ), Vector3.Up );

			var speed = WishVelocity.Length.Clamp( 0, 1 );

			WishVelocity = WishVelocity.Normal * speed;
			WishVelocity *= GetWishSpeed();
		}

		void Accelerate( Vector3 wishDir, float wishSpeed, float speedLimit, float acceleration )
		{
			if ( speedLimit > 0 && wishSpeed > speedLimit )
				wishSpeed = speedLimit;

			var currentspeed = Velocity.Dot( wishDir );
			var addspeed = wishSpeed - currentspeed;

			if ( addspeed <= 0 )
				return;

			var accelspeed = acceleration * Time.Delta * wishSpeed;

			if ( accelspeed > addspeed )
				accelspeed = addspeed;

			Velocity += wishDir * accelspeed;
		}

		void ApplyFriction( float frictionAmount = 1.0f )
		{
			var speed = Velocity.Length;

			if ( speed < 0.1f ) return;

			var drop = speed * Time.Delta * frictionAmount;

			float newspeed = speed - drop;

			if ( newspeed < 0 ) newspeed = 0;

			if ( newspeed != speed )
			{
				newspeed /= speed;
				Velocity *= newspeed;
			}
		}

		void SetRotation()
		{
			var targetRotation = Rotation.LookAt( Input.Rot.Forward.WithZ( 0 ), Vector3.Up );

			Vector3 diff = Velocity - OldVelocity;

			var scalar = (BaseAccel * Time.Delta * BaseSpeed);

			var pitch = diff.Dot( Rot.Forward.WithZ( 0 ) ) / scalar;
			var roll = diff.Dot( Rot.Right.WithZ( 0 ) ) / scalar;

			targetRotation *= Rotation.FromPitch( pitch * MaxTilt );
			targetRotation *= Rotation.FromRoll( roll * MaxTilt );

			Rot = Rotation.Slerp( Rot, targetRotation, TurnSpeed * Time.Delta );
		}

		void WriteDebugInfo()
		{
			BBox hull = GetHull();

			DebugOverlay.Box( Pos + TraceOffset, hull.Mins, hull.Maxs, Color.Red );
			DebugOverlay.Box( Pos, hull.Mins, hull.Maxs, Color.Blue );

			var lineOffset = Host.IsServer ? 10 : 0;

			DebugOverlay.ScreenText( lineOffset + 0, $"             Pos: {Pos}" );
			DebugOverlay.ScreenText( lineOffset + 1, $"             Vel: {Velocity} ({Velocity.Length:n0})" );
			DebugOverlay.ScreenText( lineOffset + 2, $"    WishVelocity: {WishVelocity}" );
			DebugOverlay.ScreenText( lineOffset + 3, $"    BaseVelocity: {BaseVelocity}" );
			DebugOverlay.ScreenText( lineOffset + 4, $"        Rotation: {Rot.Angles().pitch:n2},{Rot.Angles().yaw:n2},{Rot.Angles().roll:n2}" );
		}
	}
}
