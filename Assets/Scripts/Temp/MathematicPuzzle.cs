using UnityEngine;
using System.Collections;

public class MathematicPuzzle : MonoBehaviour {
	/// <summary>
	/// The previous value.
	/// </summary>
	private int previousValue=1;
	/// <summary>
	/// Reference to the original transform
	/// </summary>
	private Vector3 originalPosition;
	/// <summary>
	/// The max value to clamp
	/// </summary>
	public int maxValue=6;
	/// <summary>
	/// The minimum value to clamp
	/// </summary>
	public int minValue=-3;

	public void Start(){
		originalPosition = this.transform.position;
	}

	public void Add(int value){
		previousValue += value;
		Vector3 position = originalPosition;
		position.y += Mathf.Clamp(previousValue,minValue,maxValue);
		this.transform.position = position;
	}

	public void Multiply(int value){
		previousValue *= value;
		Vector3 position = originalPosition;
		position.y += Mathf.Clamp(previousValue,minValue,maxValue);
		this.transform.position = position;
	}

	public void Reset(){
		previousValue = 1;
		this.transform.position = originalPosition;
	}
}
