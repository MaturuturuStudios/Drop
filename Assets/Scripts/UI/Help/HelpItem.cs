using UnityEngine;

/// <summary>
/// Base class for help information items.
/// </summary>
[RequireComponent(typeof(Animator))]
public class HelpItem : MonoBehaviour {

    /// <summary>
    /// Reference to the object's animator component.
    /// </summary>
	protected Animator _animator;

	/// <summary>
	/// Flag indicating if the special has been already triggered.
	/// </summary>
    protected bool _specialTriggered;

	/// <summary>
	/// Reference to this entity's Transform component.
	/// </summary>
	protected Transform _transform;

	/// <summary>
	/// Unity's method called right after the object
	/// is created.
	/// </summary>
	void Awake() {
		_transform = transform;
		_animator = GetComponent<Animator>();
		OnAwake();
	}

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
	void Update() {
		// Orientates the object to the camera
		_transform.rotation = Camera.main.transform.rotation;

		// Checks if the spectial is triggered
		SetSpecial(IsSpecialTriggered());

		// Delegates into OnUpdate
		OnUpdate();
    }

    /// <summary>
    /// Shows the information item.
    /// </summary>
	public void Show() {
        SetSpecial(IsSpecialTriggered());
        _animator.SetBool("shown", true);
		OnShow();
	}

    /// <summary>
    /// Hides the information item.
    /// </summary>
    public void Hide() {
		// Checks if the object has been destroyed
		if (this == null)
			return;

		if (gameObject.activeInHierarchy)
			_animator.SetBool("shown", false);
		OnHide();
	}

	/// <summary>
	/// Modifies the special trigger flag of the help item.
	/// </summary>
	/// <param name="specialTriggered">If the special trigger is active or not</param>
    public void SetSpecial(bool specialTriggered) {
        if (specialTriggered != _specialTriggered) {
            _specialTriggered = specialTriggered;
            _animator.SetBool("special", specialTriggered);
            OnSpecialTriggered(specialTriggered);
        }
    }

    /// <summary>
    /// Delegate for children's Awake calls.
    /// </summary>
	protected virtual void OnAwake() { }

    /// <summary>
    /// Delegate for children's Update calls.
    /// </summary>
	protected virtual void OnUpdate() { }

    /// <summary>
    /// Delegate for children's Show calls.
    /// </summary>
	protected virtual void OnShow() { }

    /// <summary>
    /// Delegate for children's Hide calls.
    /// </summary>
	protected virtual void OnHide() { }

    /// <summary>
    /// Delegate for children's SetSpecial calls.
    /// </summary>
	protected virtual void OnSpecialTriggered(bool specialTriggered) { }

    /// <summary>
    /// Delegate for children's IsSpecial calls.
    /// </summary>
	protected virtual bool IsSpecialTriggered() { return false; }
}
