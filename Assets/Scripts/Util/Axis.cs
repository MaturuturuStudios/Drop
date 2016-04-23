/// <summary>
/// Static class which stores the axis used in the game.
/// Whenever a new axis is created, a new constant should
/// be added to this class using that axis.
/// </summary>
public static class Axis {

	/// <summary>
	/// Used for the character's horizontal movement.
	/// Also used while aiming on shoot mode.
	/// Positive means right, negative means left.
	/// </summary>
	public static readonly string Horizontal = "Horizontal";

	/// <summary>
	/// Used for the character's vertical movement.
	/// Also used while aiming on shoot mode.
	/// Positive means up, negative means down.
	/// </summary>
	public static readonly string Vertical = "Vertical";

	/// <summary>
	/// Used to change the direction the character's facing
	/// while aiming on shoot mode.
	/// Positive means right, negative means left.
	/// </summary>
	public static readonly string LookAtDir = "LookAtDir";

	/// <summary>
	/// Used to change the size of the fired drop while
	/// aiming on shoot mode
	/// Positive means plus, negative means minus.
	/// </summary>
	public static readonly string ShootCounter = "ShootCounter";

	/// <summary>
	/// Used to change control beetween characters.
	/// Positive means next one, negative means previous one.
	/// </summary>
	public static readonly string SelectDrop = "SelectDrop";

	/// <summary>
	/// Used to immediately control a character.
	/// Selects the first one.
	/// </summary>
	public static readonly string SelectDrop1 = "SelectDrop1";

	/// <summary>
	/// Used to immediately control a character.
	/// Selects the second one.
	/// </summary>
	public static readonly string SelectDrop2 = "SelectDrop2";

	/// <summary>
	/// Used to immediately control a character.
	/// Selects the third one.
	/// </summary>
	public static readonly string SelectDrop3 = "SelectDrop3";

	/// <summary>
	/// Used to immediately control a character.
	/// Selects the fourth one.
	/// </summary>
	public static readonly string SelectDrop4 = "SelectDrop4";

	/// <summary>
	/// Used to immediately control a character.
	/// Selects the fifth one.
	/// </summary>
	public static readonly string SelectDrop5 = "SelectDrop5";

	/// <summary>
	/// Used to move the camera horizontally.
	/// Positive means right, negative means left.
	/// </summary>
	public static readonly string CamHorizontal = "CamHorizontal";

	/// <summary>
	/// Used to move the camera vertically.
	/// Positive means up, negative means down.
	/// </summary>
	public static readonly string CamVertical = "CamVertical";

	/// <summary>
	/// Used to jump and wall jump.
	/// </summary>
	public static readonly string Jump = "Jump";

	/// <summary>
	/// Used to feed water to plants and other objects on
	/// the scene.
	/// </summary>
	public static readonly string Irrigate = "Irrigate";

	/// <summary>
	/// Used to interact with devices and other objects
	/// on the scene.
	/// </summary>
	public static readonly string Action = "Action";

	/// <summary>
	/// Used to enter or exit the shoot mode.
	/// </summary>
	public static readonly string ShootMode = "ShootMode";

	/// <summary>
	/// Used as the main button of the game.
	/// </summary>
	public static readonly string Start = "Start";

	/// <summary>
	/// Used as the alternative button of the game.
	/// </summary>
	public static readonly string Back = "Back";

	/// <summary>
	/// Used to read the mouse horizontal displacement on
	/// the screen.
	/// Positive means right, negative means left.
	/// </summary>
	public static readonly string MouseX = "Mouse X";

	/// <summary>
	/// Used to read the mouse vertical displacement on
	/// the screen.
	/// Positive means up, negative means down.
	/// </summary>
	public static readonly string MouseY = "Mouse Y";

	/// <summary>
	/// Used as the submit button on menus and forms.
	/// </summary>
	public static readonly string Submit = "Submit";

	/// <summary>
	/// Used as the cancel button on menus and forms.
	/// </summary>
	public static readonly string Cancel = "Cancel";
}