using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is not final as it's functionallity could be extended
/// to support many similar behaviours.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Blocker : MonoBehaviour {

	public PathDefinition path;

	public float speed = 50.0f;

	public float delay = 2.0f;

	public bool active = true;

	private float _elapsedTime = 0;

	private IEnumerator<Transform> _enumerator;

	private Transform _transform;

	private AudioSource _audioSource;

	private Transform _previous;

	void Awake() {
		_audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		_transform = transform;

		_enumerator = path.GetLoopEumerator();
		_previous = _enumerator.Current;
        _enumerator.MoveNext();

		_transform.position = _enumerator.Current.position;
    }

	void Update() {		
		if (active) {
			_elapsedTime += Time.deltaTime;
			if (_elapsedTime >= delay) {
				Next();
			}
		}

		_transform.position = Vector3.MoveTowards(_transform.position, _enumerator.Current.position, speed * Time.deltaTime);
	}

	public void SetActive(bool active) {
		this.active = active;
	}

	public void Reset() {
		_enumerator = path.GetLoopEumerator();
		_previous = _enumerator.Current;
		Next();
	}

	public void Next() {
		_previous = _enumerator.Current;
		_enumerator.MoveNext();
		_audioSource.Play();
		_elapsedTime = 0;
	}

	public void Previous() {
		_enumerator = path.GetLoopEumerator();
		_enumerator.MoveNext();
		_audioSource.Play();
		_elapsedTime = 0;
		Transform temp = _previous;
		while (_enumerator.Current != _previous) {
			temp = _enumerator.Current;
            _enumerator.MoveNext();
		}
		_previous = temp;
    }

	public void Set(int index) {
		Reset();
		for (int i = 0; i < index; i++)
			_enumerator.MoveNext();
	}
}
