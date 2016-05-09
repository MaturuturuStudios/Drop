using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HelpItem : MonoBehaviour {

	protected Animator _animator;

	protected void Awake() {
		_animator = GetComponent<Animator>();
		OnAwake();
	}

	protected void Update() {
		OnUpdate();
    }

	public void Show() {
		_animator.SetBool("shown", true);
		OnShow();
	}

	public void Hide() {
		_animator.SetBool("shown", false);
		OnHide();
	}

	protected virtual void OnAwake() { }

	protected virtual void OnUpdate() { }

	protected virtual void OnShow() { }

	protected virtual void OnHide() { }
}
