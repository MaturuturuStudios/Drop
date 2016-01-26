using UnityEngine;
using System.Collections;

public class Character_Size : MonoBehaviour {
	public float shrink_speed=0.4f;
	public float enlarge_speed=0.4f;

	private float targetting_size;
	private int shrink_or_enlarge;
	private int size;
	private Transform drop_transform;
	
	// Use this for initialization
	void Start () {
		drop_transform = gameObject.transform;
		drop_transform.localScale = Vector3.one;
		size = 1;
		targetting_size = 0;
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

		GradualModifySize ();
	}

	public void IncrementSize(){
		SetSize (size+1);
	}

	public void DecrementSize(){
		SetSize (size-1);
	}

	public void SetSize(int size){
		if(size>0 && this.size != size){
			//TODO: watch this value, using only x scale, presuppose  x,y,z has the same scale
			float difference=size - drop_transform.localScale.x;
			targetting_size = Mathf.Abs (difference);
			//positive if I grow up
			shrink_or_enlarge = (difference > 0)? (int)Mathf.Ceil(difference): (int)Mathf.Floor(difference);
			this.size = size;
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

	private void GradualModifySize(){
		if (targetting_size > 0) {
			float radius = gameObject.GetComponent<SphereCollider>().radius;
			float previous_radius = radius * this.size;

			//positive if I grow up
			float speed = (shrink_or_enlarge < 0)? shrink_speed : enlarge_speed;

			targetting_size -= Time.deltaTime * speed * Mathf.Abs(shrink_or_enlarge);
			//if finally reached the target size, set the size so we don't have floating remains
			if (targetting_size <= 0) {
				drop_transform.localScale = new Vector3 (size, size, size);
				targetting_size = 0;
			} else {
				drop_transform.localScale += Vector3.one * Time.deltaTime * speed * shrink_or_enlarge;
			}
			SetCenter(previous_radius);
		}
	}
}