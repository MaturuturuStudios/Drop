/// <summary>
/// Provides an specific component for doors using
/// the FollowPath script, limited to two points.
/// It also provides more intuitive methods such as
/// Open and Close.
/// </summary>
public class Door : FollowPath {
	
	/// <summary>
	/// Defines the state of the door.
	/// </summary>
	public enum DoorState {
		/// <summary>
		/// The door is opened. The opened position will be used.
		/// </summary>
		OPENED,

		/// <summary>
		/// The door is closed. The closed position will be used.
		/// </summary>
		CLOSED
	}

	/// <summary>
	/// Initial state of the door when the game launches.
	/// </summary>
	public DoorState initialState = DoorState.CLOSED;

	/// <summary>
	/// Current state of the door.
	/// </summary>
	private DoorState _state;

	new void Start() {
		base.Start();
		automatic = false;
		SetState(initialState);
	}

	/// <summary>
	/// Opens the door. Has no effect if it is already opened.
	/// </summary>
	public void Open() {
		Set(1);
		_state = DoorState.OPENED;
	}

	/// <summary>
	/// Closes the door. Has no effect if it is already closed.
	/// </summary>
	public void Close() {
		Set(0);
		_state = DoorState.CLOSED;
	}

	/// <summary>
	/// Switches the state of the door. The door will be opened
	/// if it was closed an viceversa.
	/// </summary>
	public void Switch() {
		if (_state == DoorState.OPENED)
			Close();
		else
			Open();
	}

	/// <summary>
	/// Directly sets the state of the door.
	/// </summary>
	/// <param name="state">The new state of the door</param>
	public void SetState(DoorState state) {
		if (state == DoorState.OPENED)
			Open();
		else
			Close();
	}

	/// <summary>
	/// Returns the state of the door.
	/// </summary>
	/// <returns>The state of the door</returns>
	public DoorState GetState() {
		return _state;
	}
}
