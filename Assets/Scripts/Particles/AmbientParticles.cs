using UnityEngine;

/// <summary>
/// Defines a particle system which emitter will move with the
/// camera, creating a particle effect on the scene.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AmbientParticles : MonoBehaviour {

	/// <summary>
	/// Reference to the Transform component.
	/// </summary>
    private Transform _transform;
	
	void Awake() {
		_transform = transform;
	}
	
	void Update () {
        // Centers the effect
        Transform camera = Camera.main.transform;
        Rect area = new Rect(camera.position.x, camera.position.y, camera.localScale.x, camera.localScale.y);
        Vector3 position = new Vector3(area.x, area.y, _transform.position.z);
		_transform.position = position;
        
        // Scales the effect
        //shape.box = new Vector3(camera.localScale.x, camera.localScale.x * 9 / 16, camera.localScale.x * 9 / 16);
    }
}
