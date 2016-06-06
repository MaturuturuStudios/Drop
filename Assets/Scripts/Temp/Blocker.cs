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

	private Transform _transform;

	private AudioSource _audioSource;

	void Awake() {
		_audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		_transform = transform;

		path.Reset();
		path.MoveNext();

		_transform.position = path.Current.position;
    }

	void Update() {		
		if (active) {
			_elapsedTime += Time.deltaTime;
			if (_elapsedTime >= delay) {
				Next();
			}
		}

		_transform.position = Vector3.MoveTowards(_transform.position, path.Current.position, speed * Time.deltaTime);
	}

	public void SetActive(bool active) {
		this.active = active;
	}

	public void Reset() {
		path.Reset();
		Next();
	}

	public void Next() {
		path.Next();
		_audioSource.Play();
		_elapsedTime = 0;
	}

	public void Previous() {
		path.Previous();
		_audioSource.Play();
		_elapsedTime = 0;
	}

	public void Set(int index) {
		path.SetIndex(index);
	}
}
