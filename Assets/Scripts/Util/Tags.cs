/// <summary>
/// Static class which stores the tags used in the game.
/// Whenever a new tag is created, a new constant should
/// be added to this class using that tag.
/// </summary>
public static class Tags {

	/// <summary>
	/// Tag for the scene's Game Controller object.
	/// </summary>
	public static readonly string GameController = "GameController";
	
	/// <summary>
	/// Tag for the player's character objects.
	/// </summary>
	public static readonly string Player = "Player";

	/// <summary>
	/// Tag for the scene's GUI menus.
	/// </summary>
	public static readonly string Menus = "Menus";

	/// <summary>
	/// Tag for a moving platform where the character can land.
	/// </summary>
	public static readonly string MovingPlatform = "MovingPlatform";

	/// <summary>
	/// Tag for wall where the character can perform a wall jump.
	/// </summary>
	public static readonly string WallJump = "WallJump";

	/// <summary>
	/// Tag for wall where the character can perform a wall jump.
	/// </summary>
	public static readonly string Level = "Level";

    /// <summary>
    /// Tag for the object with the data of the game
    /// </summary>
    public static readonly string GameData = "GameData";
}