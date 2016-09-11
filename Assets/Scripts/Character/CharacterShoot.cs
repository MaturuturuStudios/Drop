using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class active, desactive shoot-mode and shoot a drop 
/// </summary>
public class CharacterShoot : MonoBehaviour {

    #region Private Attributes

    /// <summary>
    /// List of observers subscribed to the character shoot's
    /// events.
    /// </summary>
    private List<CharacterShootListener> _listeners = new List<CharacterShootListener>();

    /// <summary>
    /// Defines the boolean to know if we are in shootmode or out of shootmode.
    /// </summary> 
    private bool _shootMode = false;

    /// <summary>
    /// The controller of the character
    /// </summary> 
    private CharacterControllerCustom ccc;
    /// <summary>
    /// The trajectory of the shooting
    /// </summary>
    private CharacterShootTrajectory _shootTrajectory;
    /// <summary>
    /// The independent control to get new drops
    /// </summary>
    private GameControllerIndependentControl _independentControl;
    /// <summary>
    /// The size of the character
    /// </summary>
    private CharacterSize size;

	/// <summary>
	/// Reference to the object which will be shown when the
	/// player is unable to shoot.
	/// </summary>
	public UnableIndicator unableIndicator;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    public void Awake() {
        ccc = GetComponent<CharacterControllerCustom>();
        _shootTrajectory = GetComponent<CharacterShootTrajectory>();
        size = GetComponent<CharacterSize>();
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();
    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    public void Update() {
        //check if we shouldn't be in shootmode
        if (_shootMode
            && (ccc.State.IsGrounded == false || size.GetSize() == 1)) {

            //quit it inmediatly
            _shootTrajectory.QuitShootMode();
            ccc.Parameters = null;

            // Notifies the listeners
            foreach (CharacterShootListener listener in _listeners)
                listener.OnExitShootMode(this);
        }
    }

    /// <summary>
    /// Subscribes a listener to the shoot's events.
    /// Returns false if the listener was already subscribed.
    /// </summary>
    /// <param name="listener">The listener to subscribe</param>
    /// <returns>If the listener was successfully subscribed</returns>
    public bool AddListener(CharacterShootListener listener) {
        if (_listeners.Contains(listener))
            return false;
        _listeners.Add(listener);
        return true;
    }

    /// <summary>
    /// Unsubscribes a listener to the shoot's events.
    /// Returns false if the listener wasn't subscribed yet.
    /// </summary>
    /// <param name="listener">The listener to unsubscribe</param>
    /// <returns>If the listener was successfully unsubscribed</returns>
    public bool RemoveListener(CharacterShootListener listener) {
        if (!_listeners.Contains(listener))
            return false;
        _listeners.Remove(listener);
        return true;
    }

    /// <summary>
	/// Method to know if if we are in shootmode or not.
	/// </summary>
    public bool isShooting() {
        return _shootMode;
    }

    /// <summary>
	/// Method to increase the size of the drop shooted
	/// </summary>
    public void IncreaseSize() {
        if (_shootMode) _shootTrajectory.IncreaseSize();
    }

    /// <summary>
	/// Method to look to the other side
	/// </summary>
    public void LookatRight() {
        if (_shootMode) _shootTrajectory.LookAtRight();
    }

    /// <summary>
	/// Method to look to the other side
	/// </summary>
    public void LookatLeft() {
        if (_shootMode) _shootTrajectory.LookAtLeft();
    }

    /// <summary>
    /// Method to decrease the size of the drop shooted
    /// </summary>
    public void DecreaseSize() {
        if (_shootMode) _shootTrajectory.DecreaseSize();
    }

    /// <summary>
	/// Method to start the shootmode
	/// </summary>
    public void Aim() {
        float actualSize = size.GetSize();

        //if can be in shoot mode, go ahead
        if (ccc.State.IsGrounded == true && (actualSize > 1)
            && _independentControl.CountControlledDrops() < 4) {
			if (actualSize <= 1) {
				unableIndicator.Show();
				return;
			}

            //if not in shoot mode, go
            if (!_shootMode) {
                StartShootMode();

                //if already in shoot mode and not during an animation, quit it
            } else if (_shootMode && !_shootTrajectory.IsInAnimation()) {
                _shootTrajectory.EndShootMode();
            }
        }
    }

    /// <summary>
    /// Must be called only for CharacterShootTrajectory
    /// </summary>
    public void ShootModeEnded() {
        _shootMode = false;
        // Notifies the listeners
        foreach (CharacterShootListener listener in _listeners)
            listener.OnExitShootMode(this);
    }

    /// <summary>
	/// Method to shoot the drop
	/// </summary>
    public void Shoot() {
        if (_shootMode && !_shootTrajectory.IsInAnimation() && _shootTrajectory.CanShoot()) {
            _shootMode = false;
            //stop inmediatly the shoot trajectory
            ccc.Parameters = null;
            //quit shoot mode
            _shootTrajectory.QuitShootMode();
            //reduce the size of the character
            size.SetSize((int)(size.GetSize() - _shootTrajectory.shootSize));

            // Notifies the listeners
            foreach (CharacterShootListener listener in _listeners)
                listener.OnExitShootMode(this);

            throwBall();
        }
    }

    /// <summary>
    /// Draw the trajectory debug
    /// </summary>
    public void OnDrawGizmosSelected() {
        if (!Application.isPlaying) {
            Awake();
            Update();
        }
    }

    /// <summary>
    /// Method to shoot the drop.
    /// </summary>
    private void throwBall() {
        GameObject ball = _independentControl.CreateDrop(this.transform.position, true);
        ball.GetComponent<CharacterSize>().SetSize((int)_shootTrajectory.shootSize);
        ball.GetComponent<CharacterControllerCustom>().SendFlying(_shootTrajectory.GetVelocityDrop());

        // Notifies the listeners
        foreach (CharacterShootListener listener in _listeners)
            listener.OnShoot(this, ball, _shootTrajectory.GetVelocityDrop());
    }

    /// <summary>
    /// Start the shoot mode
    /// </summary>
    private void StartShootMode() {
        if (_shootTrajectory.IsInAnimation()) return;
        _shootTrajectory.EnableTrajectory();
        ccc.Parameters = CharacterControllerParameters.ShootingParameters;

        _shootMode = true;
        // Notifies the listeners
        foreach (CharacterShootListener listener in _listeners)
            listener.OnEnterShootMode(this);
    }


    #endregion
}