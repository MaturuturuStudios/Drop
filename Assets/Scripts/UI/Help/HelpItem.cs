﻿using UnityEngine;

/// <summary>
/// Base class for help information items.
/// </summary>
[RequireComponent(typeof(Animator))]
public class HelpItem : MonoBehaviour {

    /// <summary>
    /// Reference to the object's animator component.
    /// </summary>
	protected Animator _animator;

    protected bool _specialTriggered;

    /// <summary>
    /// Unity's method called right after the object
    /// is created.
    /// </summary>
	void Awake() {
		_animator = GetComponent<Animator>();
		OnAwake();
	}

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
	void Update() {
        SetSpecial(IsSpecialTriggered());
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
		_animator.SetBool("shown", false);
		OnHide();
	}

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
