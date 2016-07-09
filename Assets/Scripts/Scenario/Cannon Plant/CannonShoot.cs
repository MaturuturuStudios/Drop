using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
[ExecuteInEditMode]
public class CannonShoot : ActionPerformer {
    public LaunchCharacter launch;

    public float AnimationTime=0.0f;

    private float _animationtime;

    private bool _throwing=false;

    private CharacterControllerCustom _ccc;

    private GameObject _character;
    /// <summary>
    /// List of listeners registered to this component's events.
    /// </summary>
    private List<CannonShootListener> _listeners = new List<CannonShootListener>();

	/// <summary>
	/// Subscribes a listener to the components's events.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(CannonShootListener listener) {
		if (_listeners.Contains(listener))
			return false;
		_listeners.Add(listener);
		return true;
	}

	/// <summary>
	/// Unsubscribes a listener to the components's events.
	/// Returns false if the listener wasn't subscribed yet.
	/// </summary>
	/// <param name="listener">The listener to unsubscribe</param>
	/// <returns>If the listener was successfully unsubscribed</returns>
	public bool RemoveListener(CannonShootListener listener) {
		if (!_listeners.Contains(listener))
			return false;
		_listeners.Remove(listener);
		return true;
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	public void Update(){
		//transform.eulerAngles = new Vector3(0, 0, launch.GetAngle()); //this is to face in the direction you are aming

        if (_throwing){
            Debug.Log(" timer " + _animationtime);
            _animationtime += Time.deltaTime;
            ThrowDrop();
        }
	}

	protected override bool OnAction(GameObject character)	{
        _ccc = character.GetComponent<CharacterControllerCustom>();
        character.SetActive(false);
        _character = character;
        _throwing = true;
		return true;
    }

    public void ThrowDrop(){
        
        if (_animationtime > AnimationTime){
            _throwing = false;
            _animationtime = 0.0f;
            _character.SetActive(true);
            _ccc.transform.position = launch.pointOrigin.transform.position;
            _ccc.Stop();
             Vector3 velocity = launch.GetNeededVelocityVector();

        
            _ccc.SendFlying(velocity);

            // Notifies the listeners
            foreach (CannonShootListener listener in _character.GetComponents<CannonShootListener>())
                listener.OnCannonShoot(this, _character, velocity);
            foreach (CannonShootListener listener in _listeners)
                listener.OnCannonShoot(this, _character, velocity);
        }
    }

    public void ChangeAngleCannon(float newAngle) {
        launch.SetAngle(newAngle);
    }

    public void OnDrawGizmos() {
        launch.OnDrawGizmos();
    }
}
