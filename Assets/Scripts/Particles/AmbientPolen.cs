using UnityEngine;
using System.Collections;

public class AmbientPolen : MonoBehaviour {
    private ParticleSystem system;
    private ParticleSystem.ShapeModule shape;


	// Use this for initialization
	void Awake() {
        system = GetComponent<ParticleSystem>();
        shape = system.shape;
	}
	
	// Update is called once per frame
	void Update () {
        //center the effect
        Transform camera = Camera.main.transform;
        Rect area = new Rect(camera.position.x, camera.position.y, camera.localScale.x, camera.localScale.y);
        Vector3 position = new Vector3(area.x, area.y, system.transform.position.z);
        system.transform.position = position;
        
        //scale the effect
        //shape.box = new Vector3(camera.localScale.x, camera.localScale.x * 9 / 16, camera.localScale.x * 9 / 16); ;
    }
}
