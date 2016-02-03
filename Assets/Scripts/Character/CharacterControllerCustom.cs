using UnityEngine;
using System.Collections;

public class CharacterControllerCustom : MonoBehaviour {
	// TODO: El codigo que hay es para borrar. Solo se conserva para pruebas.
	
	// Public variables
	public float acceleration = 100;
	public float max_speed = 10;
	
	// Private variables
	private float h_input;
	private float target_speed;
	
	// References
	private CharacterSize size_component;
	private Rigidbody rigidbody_component;
	
	void Start () {
		// Looks for the Size component
		size_component = GetComponent<CharacterSize>();
		// Looks for the Rigidbody component
		rigidbody_component = GetComponent<Rigidbody>();
	}
	
	void Update () {
		// Calculates the target speed
		target_speed = h_input * max_speed * size_component.GetSize();
		
		// Calculates de new speed of the entity
		float speed = rigidbody_component.velocity.x;
		if (rigidbody_component.velocity.x < target_speed)
			speed += acceleration * Time.deltaTime;
		else if (rigidbody_component.velocity.x > target_speed)
			speed -= acceleration * Time.deltaTime;
		
		// If the speed is too close to the target speed, sets the speed to the target speed
		if (Mathf.Abs(speed - target_speed) <= acceleration * Time.deltaTime)	// The acceleration acts as the tolerance
			speed = target_speed;
		
		// Sets the velocity of the rigidbody
		rigidbody_component.velocity = new Vector3(speed, rigidbody_component.velocity.y, rigidbody_component.velocity.z);
	}
	
	public void SetInput(float h_input) {
		// Sets the current horizontal input
		this.h_input = h_input;
	}

	public void Jump ()
	{
		// TODO: Por ahora no hace nada.
	}
}