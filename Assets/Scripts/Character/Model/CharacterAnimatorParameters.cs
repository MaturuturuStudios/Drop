/// <summary>
/// Static class which stores the parameters used by the
/// character's animator.
/// </summary>
public static class CharacterAnimatorParameters {

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
	/// Triggered when the player jumps.
	/// </summary>
	public static readonly string Jump = "jump";
}