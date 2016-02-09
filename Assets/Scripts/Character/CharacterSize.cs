using UnityEngine;
using System.Collections;

/// <summary>
/// The size of the character. Control if the character can grow up or decrease and how.
/// </summary>
public class CharacterSize : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// Decrease rate
    /// </summary>
    public float shrinkSpeed = 5f;
    /// <summary>
    /// Growth rate
    /// </summary>
    public float enlargeSpeed = 5f;
    #endregion

    #region Custom private Enumerations
    /// <summary>
    /// Information about the axis when check for collisions
    /// </summary>
    private struct InfoAxis {
        //0 for horizontal
        //1 for vertical
        public int axis;
        public Vector3 offset;
        public bool block;
    };
    #endregion

    #region Variables
    /// <summary>
    /// Growing or decreasing
    /// </summary>
	private int _shrinkOrEnlarge;
    /// <summary>
    /// Target size of the character
    /// </summary>
	private int _targetSize;
    /// <summary>
    /// Parameters setted to the moment the character is growing up/decreasing
    /// </summary>
    CharacterControllerParameters _quietGrowingParameters;
    #endregion

    #region Private Attributes
    /// <summary>
    /// Transform of the character
    /// </summary>
    private Transform _dropTransform;
    /// <summary>
    /// Radius of character
    /// </summary>
    private float _ratioRadius;
    #endregion

    #region Methods
    #region Public Methods
    /// <summary>
    /// Initialization method.
    /// The character start with size one
    /// </summary>
    void Start() {
		_dropTransform = gameObject.transform;
		_dropTransform.localScale = Vector3.one;
        _ratioRadius=GetComponent<CharacterController>().radius;

        _quietGrowingParameters = new CharacterControllerParameters();
        _quietGrowingParameters.movementControl = CharacterControllerParameters.MovementControl.None;

        _targetSize = 1;
		SetSize(1);
	}

	/// <summary>
    /// Check input and control the size
    /// </summary>
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

    /// <summary>
    /// Increment the size by one
    /// </summary>
    public void IncrementSize() {
		SetSize(_targetSize + 1);
	}

    /// <summary>
    /// Decrement the size by one (with minimum of one)
    /// </summary>
	public void DecrementSize() {
		SetSize(_targetSize - 1);
	}

    /// <summary>
    /// Set a size. While changing the size, the character will not move
    /// </summary>
    /// <param name="size">New size</param>
	public void SetSize(int size) {
		if(size > 0 && size != _targetSize) {
            //can't move
            GetComponent<CharacterControllerCustom>().Parameters = _quietGrowingParameters;

            //set the new size
            _targetSize = size;

            //set if growing up or decreasing. Positive if I grow up
            //TODO: watch this value, using only x scale, presuppose  x,y,z has the same scale
            float difference = size - _dropTransform.localScale.x;
            _shrinkOrEnlarge = (difference > 0) ? (int)Mathf.Ceil(difference) :
				                                (int)Mathf.Floor(difference);
		}
	}

    /// <summary>
    /// Get the actual size
    /// </summary>
    /// <returns>The actual/Targeted size, may not be the actual size of the character</returns>
	public float GetSize() {
        return _targetSize;
	}
    #endregion

    #region Private Methods
    /// <summary>
    /// Control the gradual change of size
    /// </summary>
    private void GradualModifySize() {
        //if I have pending change...
        if(_targetSize != transform.localScale.x) {
            //set the size, positive if I grow up
            float speed = (_shrinkOrEnlarge < 0) ? shrinkSpeed : enlargeSpeed;

            //substract the growth
            float _targettingSize = Mathf.Abs(_targetSize - _dropTransform.localScale.x);
            _targettingSize -= Time.deltaTime * speed * Mathf.Abs(_shrinkOrEnlarge);


            float newScale = 0;
            if(_targettingSize <= 0) {
                //if finally reached the target size, set the size harcoding so we don't have floating remains
                newScale = _targetSize;
                _targettingSize = 0;

            } else
                newScale = _dropTransform.localScale.x + Time.deltaTime * speed * _shrinkOrEnlarge;


            //control if i have space enough and repositioning it
            Vector3 offset = Vector3.zero;
            float newRadius = newScale * _ratioRadius;
            float previousRadius = _dropTransform.localScale.x * _ratioRadius;

            if(!CanSetSize(previousRadius, newRadius, out offset)) {
                //this line is needed. If spitDrop set the size equal to actual size of character
                //GradualModifySize will not enter so never get inside the condition
                //where we recover the movement to the character
                _targettingSize = 0;
                //I can't, get the maximum size and spit the rest
                SpitDrop(newScale);

            } else {
                //set the new size to the character
                _dropTransform.localScale = new Vector3(newScale, newScale, newScale);
                //put it where it has no collision
                _dropTransform.position += offset;
            }

            //in my size! can move again.
            if(_targettingSize == 0) 
                GetComponent<CharacterControllerCustom>().Parameters = null;
            
        }
	}

    /// <summary>
    /// Check If I have enough space with the actual size
    /// </summary>
    /// <param name="previousRadius">The last radius</param>
    /// <param name="newRadius">The new radius</param>
    /// <param name="offset">The needed offset for the character for not collide</param>
    /// <returns>true if the new size is posible</returns>
	private bool CanSetSize(float previousRadius, float newRadius, out Vector3 offset) {
        bool canGrowUp = false;
        offset = Vector3.zero;

        //set the center with the new radius
        float offsetCenter = newRadius - previousRadius;

        //if is decreasing, does not check never, where a big one fit, a smaller one too
        //only set the center
        if(_shrinkOrEnlarge < 0) {
            offset.y += offsetCenter;
            return true;
        }

		//get the position of the character
		Vector3 position = _dropTransform.position;
		//set the center with the new radius
        position.y += offsetCenter;

        //ask the axis
		InfoAxis horizontal_axis = checkAxis(0, position, previousRadius, newRadius);
		InfoAxis vertical_axis = checkAxis(1, position, previousRadius, newRadius);

        //if not blocked...
		if(!horizontal_axis.block && !vertical_axis.block) {
			//set the offset and set that I can grow up
			offset = horizontal_axis.offset + vertical_axis.offset;
            offset.y += offsetCenter;
			canGrowUp = true;
		}

		return canGrowUp;
	}

    /// <summary>
    /// If can't grow up, spit the extra drops and set the maximum size I can
    /// TODO: spit the extra drops
    /// </summary>
	private void SpitDrop(float newScale) {
		//final size...
		float finalScale = _dropTransform.localScale.x;
        //truncate value
		int finalSize = setMaximumSize((int) finalScale);

        //calculate the extra drop I have to spit
		int numberDropsRemain = _targetSize - finalSize;
		Debug.Log("Spit " + numberDropsRemain + " out");
        
        //set the final size
        SetSize(finalSize);
	}

    /// <summary>
    /// When the growth is blocked, set the maximum size posible
    /// </summary>
    /// <param name="sizeMaximum">The know maximum</param>
    /// <returns>The maximum size posible</returns>
    private int setMaximumSize(int sizeMaximum) {
        if(sizeMaximum == 1) {
            return 1;
        }

        int maximum = sizeMaximum+1;

        bool posibleSize = false;
        do {
            --maximum;

            //get the position of the character
            Vector3 position = _dropTransform.position;

            float newRadius = maximum * _ratioRadius;
            float previousRadius = _dropTransform.localScale.x * _ratioRadius;

            //set the center with the new radius
            float offsetCenter = newRadius - previousRadius;
            position.y += offsetCenter;

            //ask the axis
            InfoAxis horizontal_axis = checkAxis(0, position, newRadius, newRadius);
            InfoAxis vertical_axis = checkAxis(1, position, newRadius, newRadius);

            //if not blocked... (or minimum one)
            posibleSize = (!horizontal_axis.block && !vertical_axis.block) || maximum==1;
        } while(!posibleSize);
        
        return maximum;
    }

    /// <summary>
    /// Get the vectors of the direction for the RayCast to the given axis and side
    /// </summary>
    /// <param name="axis">The axis</param>
    /// <param name="side">The side (0-1)</param>
    /// <returns>Three vectors indicating the direction</returns>
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

    /// <summary>
    /// Check an axis.
    /// </summary>
    /// <param name="axis">The axis to check</param>
    /// <param name="position">the center of the character</param>
    /// <param name="previousRadius">The previous radius of the character</param>
    /// <param name="radius">The new/actual radius to get the distance to cast</param>
    /// <returns>Information about the axis. Will include the offset needed to move to avoid collisions.
    /// If the axis is blocked, the offset is not reliable.</returns>
	private InfoAxis checkAxis(int axis, Vector3 position, float previousRadius, float radius) {
        //prepare the information result
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
    #endregion

    #region "Inherited" Methods
    public void OnCustomCollisionEnter(RaycastHit hit) {
		// TODO: collision with another drop?
	}

	public void OnCustomCollisionStay(RaycastHit hit) {
		// TODO: Test method, remove at will
	}

	public void OnCustomCollisionExit(RaycastHit hit) {
		// TODO: Test method, remove at will
	}
    #endregion
    #endregion
}