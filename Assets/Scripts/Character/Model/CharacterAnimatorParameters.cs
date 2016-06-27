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
}