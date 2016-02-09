using UnityEngine;
using System.Collections;

public class CharacterSize : MonoBehaviour {

	public float shrinkSpeed = 5f;
	public float enlargeSpeed = 5f;

	private float _targettingSize;
	private int _shrinkOrEnlarge;
	private int _size;
	private int _newSize;

	private Transform _dropTransform;
	private CharacterControllerCustom _dropController;

	// Use this for initialization
	void Start() {
		_dropTransform = gameObject.transform;
		_dropTransform.localScale = Vector3.one;
		_dropController = GetComponent<CharacterControllerCustom>();

		_size = 1;
		_targettingSize = 0;
		SetSize(1);
	}

	// Update is called once per frame
	void Update() {
		if(Input.GetKeyDown(KeyCode.UpArrow))
			IncrementSize();

		if(Input.GetKeyDown(KeyCode.DownArrow))
			DecrementSize();


		//Some number pressed? we use 1-9 as range 1-9
		bool done = false;
		for(int i = 1; i < 10 && !done; i++) {
			if(Input.GetKeyDown("" + i)) {
				SetSize(i);
				done = true;
			}
		}

		GradualModifySize();
	}

	public void IncrementSize() {
		SetSize(_size + 1);
	}

	public void DecrementSize() {
		SetSize(_size - 1);
	}

	public void SetSize(int size) {
		if(size > 0 && this._size != size) {
			//TODO can't move
			//CharacterControllerParameters parameters;
			//parameters.movementBehaviour = CharacterControllerParameters.MovementBehaviour.CantMoveSliding;

			//TODO: watch this value, using only x scale, presuppose  x,y,z has the same scale
			float difference = size - _dropTransform.localScale.x;
			_newSize = size;
			_targettingSize = Mathf.Abs(difference);
			//positive if I grow up
			_shrinkOrEnlarge = (difference > 0) ? (int)Mathf.Ceil(difference) :
				(int)Mathf.Floor(difference);
			this._size = size;
		}
	}

	public float GetSize() {
		return _size;
	}

	private void GradualModifySize() {
		if(_targettingSize > 0) {
			float previousRadius = _dropTransform.localScale.x * 0.5f;
			float newScale = 0;

			//positive if I grow up
			float speed = (_shrinkOrEnlarge < 0) ? shrinkSpeed : enlargeSpeed;

			_targettingSize -= Time.deltaTime * speed * Mathf.Abs(_shrinkOrEnlarge);
			//if finally reached the target size, set the size so we don't have floating remains
			if(_targettingSize <= 0) {
				newScale = _size;
				_targettingSize = 0;
			} else
				newScale = _dropTransform.localScale.x + Time.deltaTime * speed * _shrinkOrEnlarge;

			float newRadius = newScale * 0.5f;

			//control if i have space enough and repositioning it
			Vector3 offset = Vector3.zero;
			if(!CanSetSize(previousRadius, newRadius, out offset))
				SpitDrop();
			else {
				//set the new size
				_dropTransform.localScale = new Vector3(newScale, newScale, newScale);
				//put it where it has no collision
				_dropTransform.position += offset;
			}

			//finished! can move again.
			//Controlling nobody changed the movement to another value
			//TODO: move again
			//if(_targettingSize==0)
			// _dropController.Parameters.movementBehaviour = _previousBehaviour;
		}
	}

	private bool CanSetSize(float previousRadius, float newRadius, out Vector3 offset) {
		bool canGrowUp = false;
		offset = Vector3.zero;

		//get the data of the drop
		Vector3 position = _dropTransform.position;

		//set the center with the new radius
		//position.y += newRadius - previousRadius;

		InfoAxis horizontal_axis = checkAxis(0, position, previousRadius, newRadius);
		InfoAxis vertical_axis = checkAxis(1, position, previousRadius, newRadius);

		if(!horizontal_axis.block && !vertical_axis.block) {
			//move the offset...
			offset = horizontal_axis.offset + vertical_axis.offset;
			canGrowUp = true;
		}

		return canGrowUp;
	}

	private void SpitDrop() {
		//TODO spit a drop when I'm growing up and I can't have more
		//also, truncate the size value

		//final size...
		float finalScale = _dropTransform.localScale.x;
		int finalSize = (int)finalScale;

		int numberDropsRemain = _newSize - finalSize;
		Debug.Log("Spit " + numberDropsRemain + " out");

		//set the final size
		//TODO: can move again
		//_dropController.Parameters.movementBehaviour = _previousBehaviour;
		SetSize(finalSize);
	}

	private struct InfoAxis {
		//0 for horizontal
		//1 for vertical
		public int axis;
		public Vector3 offset;
		public bool block;
	};

	private Vector3[] getDirectionAxis(int axis, int side) {
		Vector3[] directionOneSide = new Vector3[3];

		//rotation between raycast
		Quaternion rotation = Quaternion.Euler(0, 0, 45);
		Quaternion rotation2 = Quaternion.Euler(0, 0, -45);

		switch(axis) {
		case 0: //horizontal
			if(side == 0) {
				directionOneSide[0] = Vector3.left;
				directionOneSide[1] = rotation * Vector3.left;
				directionOneSide[2] = rotation2 * Vector3.left;
			} else {
				directionOneSide[0] = Vector3.right;
				directionOneSide[1] = rotation * Vector3.right;
				directionOneSide[2] = rotation2 * Vector3.right;
			}
			break;

		case 1:
			if(side == 0) {
				directionOneSide[0] = Vector3.up;
				directionOneSide[1] = rotation * Vector3.up;
				directionOneSide[2] = rotation2 * Vector3.up;
			} else {
				directionOneSide[0] = Vector3.down;
				directionOneSide[1] = rotation * Vector3.down;
				directionOneSide[2] = rotation2 * Vector3.down;
			}
			break;
		}

		return directionOneSide;
	}

	private InfoAxis checkAxis(int axis, Vector3 position, float previousRadius, float radius) {
		InfoAxis infoResult;
		infoResult.axis = axis;
		infoResult.block = false;
		infoResult.offset = Vector3.zero;

		Vector3 offset = Vector3.zero;

		//start on center
		Vector3 rayOrigin = position;
		//the distance is the new radius given
		float distance = radius;
		float previousDistance = previousRadius;

		//three raycast per side of axis, so we have three directions of the raycast
		Vector3[] directionOneSide = getDirectionAxis(axis, 0);
		Vector3[] directionOtherSide = getDirectionAxis(axis, 1);

		//check the three raycast of one side
		for(int i = 0; i < 3; ++i) {
			//Debug.DrawRay(rayOrigin + offset, directionOneSide[i] * distance, Color.red);
			RaycastHit hit;
			if(Physics.Raycast(rayOrigin + offset, directionOneSide[i], out hit, distance)) {
				//get the offset from the hit (hit distance minus the previous radius) * the direction of the normal
				offset -= hit.normal * (hit.distance - previousDistance);
			}
		}

		bool recheck = false;
		//check the three raycast of the other side
		for(int i = 0; i < 3 && !infoResult.block; ++i) {
			//Debug.DrawRay(rayOrigin + offset, directionOtherSide[i] * distance, Color.red, 5);
			RaycastHit hit;
			if(Physics.Raycast(rayOrigin + offset, directionOtherSide[i], out hit, distance)) {
				//was a previous collision? axis blocked
				if(offset != Vector3.zero)
					infoResult.block = true;
				else
					//need a recheck of the other side
					recheck = true;

				//get the offset from the hit (hit distance minus the previous radius) * the direction of the normal
				offset -= hit.normal * (hit.distance - previousDistance);
			}
		}

		//need a recheck of the other side
		if(recheck) {
			for(int i = 0; i < 3; ++i) {
				//Debug.DrawRay(rayOrigin + offset, directionOneSide[i] * distance, Color.red, 3);
				RaycastHit hit;
				if(Physics.Raycast(rayOrigin + offset, directionOneSide[i], out hit, distance))
					//blocked!
					infoResult.block = true;
			}
		}


		infoResult.offset = offset;
		return infoResult;
	}

	public void OnCustomCollisionEnter(RaycastHit hit) {
		// TODO: collision with another drop?
	}

	public void OnCustomCollisionStay(RaycastHit hit) {
		// TODO: Test method, remove at will
	}

	public void OnCustomCollisionExit(RaycastHit hit) {
		// TODO: Test method, remove at will
	}
}