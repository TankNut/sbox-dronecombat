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

		public override void Simulate()
		{
			EyePosLocal = Vector3.Zero;
			EyeRot = Input.Rotation;

			OldVelocity = Velocity;

			BuildWishVelocity();

			ApplyFriction( BaseAccel );

			Accelerate( WishVelocity.Normal, WishVelocity.Length, BaseSpeed, BaseAccel );

			BBox hull = GetHull();

			MoveHelper mover = new( Position, Velocity );

			mover.Trace = mover.Trace.Size( hull.Mins, hull.Maxs ).HitLayer( CollisionLayer.SKY ).Ignore( Pawn );
			mover.MaxStandableAngle = 0f;
			mover.TryMove( Time.Delta );

			Position = mover.Position;
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
			WishVelocity *= Rotation.LookAt( Rotation.Forward.WithZ( 0 ), Vector3.Up );

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
			var targetRotation = Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );

			Vector3 diff = Velocity - OldVelocity;

			var scalar = (BaseAccel * Time.Delta * BaseSpeed);

			var pitch = diff.Dot( Rotation.Forward.WithZ( 0 ) ) / scalar;
			var roll = diff.Dot( Rotation.Right.WithZ( 0 ) ) / scalar;

			targetRotation *= Rotation.FromPitch( pitch * MaxTilt );
			targetRotation *= Rotation.FromRoll( roll * MaxTilt );

			Rotation = Rotation.Slerp( Rotation, targetRotation, TurnSpeed * Time.Delta );
		}

		void WriteDebugInfo()
		{
			BBox hull = GetHull();

			DebugOverlay.Box( Position + TraceOffset, hull.Mins, hull.Maxs, Color.Red );
			DebugOverlay.Box( Position, hull.Mins, hull.Maxs, Color.Blue );

			var lineOffset = Host.IsServer ? 10 : 0;

			DebugOverlay.ScreenText( lineOffset + 0, $"             Pos: {Position}" );
			DebugOverlay.ScreenText( lineOffset + 1, $"             Vel: {Velocity} ({Velocity.Length:n0})" );
			DebugOverlay.ScreenText( lineOffset + 2, $"    WishVelocity: {WishVelocity}" );
			DebugOverlay.ScreenText( lineOffset + 3, $"    BaseVelocity: {BaseVelocity}" );
			DebugOverlay.ScreenText( lineOffset + 4, $"        Rotation: {Rotation.Angles().pitch:n2},{Rotation.Angles().yaw:n2},{Rotation.Angles().roll:n2}" );
		}
	}
}
