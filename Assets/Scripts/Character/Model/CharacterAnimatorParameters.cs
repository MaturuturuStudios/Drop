/// <summary>
/// Static class which stores the parameters used by the
/// character's animator.
/// </summary>
public static class CharacterAnimatorParameters {

	/// <summary>
	/// Indicates if the character is under control.
	/// </summary>
	public static readonly string Controlled = "controlled";

	/// <summary>
	/// Triggered when the character fuses with another character.
	/// </summary>
	public static readonly string Fusion = "fusion";

	/// <summary>
	/// The size factor of the character.
	/// </summary>
	public static readonly string SizeFactor = "size_factor";

	/// <summary>
	/// Indicates the horizontal speed of the character.
	/// </summary>
	public static readonly string Speed = "speed";

	/// <summary>
	/// Indicates the vertical speed of the character.
	/// </summary>
	public static readonly string FallingSpeed = "falling_speed";

	/// <summary>
	/// Indicates the velocity the character had on the last collision.
	/// </summary>
	public static readonly string CollisionSpeed = "collision_speed";

	/// <summary>
	/// Indicates if the character is grounded.
	/// </summary>
	public static readonly string Grounded = "grounded";

	/// <summary>
	/// Indicates if the character is sliding.
	/// </summary>
	public static readonly string Sliding = "sliding";

	/// <summary>
	/// Triggered when the player begins to jump.
	/// </summary>
	public static readonly string BeginJump = "begin_jump";

	/// <summary>
	/// Time for the jump anticipation's animation to end.
	/// </summary>
	public static readonly string JumpDelay = "jump_delay";

	/// <summary>
	/// Triggered when the player performs the jump.
	/// </summary>
	public static readonly string PerformJump = "jump";

	/// <summary>
	/// Triggered when the player performs a wall jump.
	/// </summary>
	public static readonly string WallJump = "wall_jump";

	/// <summary>
	/// Indicates if the character is on shoot mode.
	/// </summary>
	public static readonly string ShootMode = "shoot_mode";

	/// <summary>
	/// Triggered when the player shoots.
	/// </summary>
	public static readonly string Shoot = "shoot";

	/// <summary>
	/// Triggered when the player waters a plant.
	/// </summary>
	public static readonly string Irrigate = "irrigate";

	/// <summary>
	/// Indicates if flying, hit by an enemy.
	/// </summary>
	public static readonly string Hit = "hit";

	/// <summary>
	/// Triggered when the player trys to perform an action they are unable to.
	/// </summary>
	public static readonly string Unable = "unable";

	/// <summary>
	/// Time in seconds since the last time the character was grounded.
	/// </summary>
	public static readonly string FloatingTime = "floating_time";
}