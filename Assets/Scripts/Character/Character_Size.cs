using UnityEngine;
using System.Collections;

public class Character_Size : MonoBehaviour {
	private int size;
	private Transform drop_transform;
	
	// Use this for initialization
	void Start () {
		drop_transform = gameObject.transform;
		size = 0;
		SetSize(1);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			IncrementSize ();
		}

		if(Input.GetKeyDown(KeyCode.DownArrow)){
			DecrementSize ();
		}

		//Some number pressed? we use 1-9 as range 1-9
		bool done=false;
		for(int i=1; i<10 && !done; i++){
			if (Input.GetKeyDown (""+i)) {
				SetSize (i);
				done = true;
			}
		}
	}

	public void IncrementSize(){
		SetSize (size+1);
	}

	public void DecrementSize(){
		SetSize (size-1);
	}

	public void SetSize(int size){
		if(size>0 && this.size != size){
			float radius = gameObject.GetComponent<SphereCollider>().radius;
			float previous_radius = radius * this.size;

			this.size = size;

			drop_transform.localScale = new Vector3 (size, size, size);
			SetCenter(previous_radius);
		}
	}

	public float GetSize() {
		return size;
	}

	private void SetCenter(float previousRadius){
		float radius = gameObject.GetComponent<SphereCollider>().radius;
		float new_radius = radius * size;
		float offset = new_radius - previousRadius;

		Vector3 localPosition = drop_transform.localPosition;
		localPosition.y += offset;
		drop_transform.localPosition=new Vector3(localPosition.x,localPosition.y,localPosition.z);
	}
}