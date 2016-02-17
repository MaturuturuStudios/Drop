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

    public GameObject BallPrefab;
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

    /// <summary>
    /// Independent control to create or remove drops
    /// </summary>
    private GameControllerIndependentControl _independentControl;
    #endregion

    #region Methods
    #region Public Methods
    void Awake() {
        _dropTransform = gameObject.transform;
        _dropTransform.localScale = Vector3.one;
        _ratioRadius = GetComponent<CharacterController>().radius;

        _independentControl = GameObject.FindGameObjectWithTag("GameController")
                                .GetComponent<GameControllerIndependentControl>();

        _quietGrowingParameters = new CharacterControllerParameters();
        _quietGrowingParameters.movementControl = CharacterControllerParameters.MovementControl.None;

        _targetSize = 1;
        SetSize(1);
    }

    /// <summary>
    /// Initialization method.
    /// The character start with size one
    /// </summary>
    void Start() {
		
    }

	/// <summary>
    /// Check input and control the size
    /// </summary>
	void Update() {
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
	public int GetSize() {
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

            if(CanSetSize(previousRadius, newRadius, out offset)) {
                //put it where it has no collision
                _dropTransform.position += offset;
                //set the new size to the character
                _dropTransform.localScale = new Vector3(newScale, newScale, newScale);
                

            } else {
                //this line is needed. If spitDrop set the size equal to actual size of character
                //GradualModifySize will not enter so never get inside the condition
                //where we recover the movement to the character
                _targettingSize = 0;
                //I can't, get the maximum size and spit the rest
                SpitDrop(newScale, offset);
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
		InfoAxis horizontal_axis = checkAxis(0, position,  newRadius);
		InfoAxis vertical_axis = checkAxis(1, position, newRadius);

        //if not blocked...
        if(!horizontal_axis.block && !vertical_axis.block) {
            //set the offset and set that I can grow up
            offset.y += offsetCenter;
            canGrowUp = true;
        }

        return canGrowUp;
	}

    /// <summary>
    /// If can't grow up, spit the extra drops and set the maximum size I can
    /// TODO: spit the extra drops
    /// </summary>
    /// <param name="newScale">the scale which doesn't fit</param>
    /// <param name="newPosition">the position according to the scale</param>
	private void SpitDrop(float newScale, Vector3 offsetCenter) {
        //get the maximum size
        int finalSize = setMaximumSize(newScale, _dropTransform.position + offsetCenter);
        //calculate the extra drop I have to spit
        int numberDropsRemain = _targetSize - finalSize;
        Debug.Log("Spit " + numberDropsRemain + " out");

        //TODO: delegate ball creation to independentControl
        //_independentControl.createDrop
        Vector3 position = new Vector3(3,7,0);
        GameObject newDrop=(GameObject)Instantiate(BallPrefab, position, Quaternion.identity);
        newDrop.GetComponent<CharacterSize>().SetSize(numberDropsRemain);
        _independentControl.AddDrop(newDrop);

        //set the final size
        SetSize(finalSize);
    }

    /// <summary>
    /// When the growth is blocked, set the maximum size posible
    /// </summary>
    /// <param name="actualScale">The actual scale which does not fit</param>
    /// <returns>The maximum size posible</returns>
    private int setMaximumSize(float actualScale, Vector3 actualPosition) {
        if(actualScale <= 1.0f) {
            return 1;
        }

        //get the maximum size (truncate the actual scale)
        int maximum = (int) actualScale;
        
        //radius with the size that exceed the limit
        float previousRadius = actualScale * _ratioRadius;

        bool posibleSize = false;
        do {
            if(maximum == 1) {
                return maximum;
            }

            //radius with a lower size
            float newRadius = maximum * _ratioRadius;

            //get the position of the character and set the new center with the new radius
            Vector3 position = actualPosition;
            float offsetCenter = newRadius - previousRadius;
            position.y += offsetCenter;

            //ask the axis
            InfoAxis horizontal_axis = checkAxis(0, position,  newRadius);
            InfoAxis vertical_axis = checkAxis(1, position, newRadius);

            //if not blocked... (or minimum size one)
            posibleSize = !horizontal_axis.block && !vertical_axis.block;

            //one size less for the next iteration
            --maximum;
        } while(!posibleSize);

        //recover the maximum (because we sustracted for the next iteration)
        return maximum+1;
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
                directionOneSide[0] = Vector3.down;
                directionOneSide[1] = rotation * Vector3.down;
                directionOneSide[2] = rotation2 * Vector3.down;
            } else {
                directionOneSide[0] = Vector3.up;
                directionOneSide[1] = rotation * Vector3.up;
                directionOneSide[2] = rotation2 * Vector3.up;    
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
	private InfoAxis checkAxis(int axis, Vector3 position, float radius) {
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

		//three raycast per side of axis, so we have three directions of the raycast
		Vector3[] directionOneSide = getDirectionAxis(axis, 0);
		Vector3[] directionOtherSide = getDirectionAxis(axis, 1);

		//check the three raycast of one side
		for(int i = 0; i < 3; ++i) {
			//Debug.DrawRay(rayOrigin + offset, directionOneSide[i] * distance, Color.red,5);
			RaycastHit hit;
			if(Physics.Raycast(rayOrigin + offset, directionOneSide[i], out hit, distance)) {
                //get the offset
                Vector3 hitting = hit.normal * (distance - hit.distance);
                offset.x = (offset.x > 0 || hitting.x >= 0) ? Mathf.Max(hitting.x, offset.x) : Mathf.Min(hitting.x, offset.x);
                offset.y = (offset.y > 0 || hitting.y >= 0) ? Mathf.Max(hitting.y, offset.y) : Mathf.Min(hitting.y, offset.y);
            }
		}

		bool recheck = false;
        //was a previous collision?
        bool hasCollision = offset != Vector3.zero;
        //check the three raycast of the other side
        for(int i = 0; i < 3 && !infoResult.block; ++i) {
			//Debug.DrawRay(rayOrigin + offset, directionOtherSide[i] * distance, Color.red, 5);
            RaycastHit hit;
			if(Physics.Raycast(rayOrigin + offset, directionOtherSide[i], out hit, distance)) {
				//was a previous collision? axis blocked
				if(hasCollision)
					infoResult.block = true;
				else
					//need a recheck of the other side
					recheck = true;

                //get the offset
                Vector3 hitting = hit.normal * (distance - hit.distance);
                offset.x = (offset.x > 0 || hitting.x >= 0) ? Mathf.Max(hitting.x, offset.x) : Mathf.Min(hitting.x, offset.x);
                offset.y = (offset.y > 0 || hitting.y >= 0) ? Mathf.Max(hitting.y, offset.y) : Mathf.Min(hitting.y, offset.y);
            }
		}

		//need a recheck of the other side
		if(recheck) {
			for(int i = 0; i < 3; ++i) {
				//Debug.DrawRay(rayOrigin + offset, directionOneSide[i] * distance, Color.red, 5);
                RaycastHit hit;
                if(Physics.Raycast(rayOrigin + offset, directionOneSide[i], out hit, distance)) {
                    //blocked!
                    infoResult.block = true;
                }
			}
		}
        
		infoResult.offset = offset;
		return infoResult;
	}


    /// <summary>
    /// Fusion between two drops
    /// </summary>
    /// <param name="anotherDrop">The drop to be absorved</param>
    private void DropFusion(GameObject anotherDrop) {
        //always check the other drop because of a posible race condition
        //checking with the active flag, destroy method does not destroy until the end of frame
        //but this method can be called again with the same object on the same frame, just in case checking...
        if(anotherDrop == null || !anotherDrop.activeInHierarchy) {
            return;
        }

        //TODO: maybe dangerous, setting it inactive will inactivate all his scripts
        anotherDrop.SetActive(false);
        

        //Get the size of the other drop
        CharacterSize otherDropSize = anotherDrop.GetComponent<CharacterSize>();
        int otherSize = otherDropSize.GetSize();
        int totalSize= otherSize + GetSize();
        
        Debug.Log(otherSize);
        Debug.Log(GetSize());
        Debug.Log(totalSize);


        //remove the other drop
        _independentControl.RemoveDrop(anotherDrop);

        //increment size of the actual drop
        SetSize(totalSize);

    }
    #endregion

    #region Override Methods
    /// <summary>
    /// Check if is other drop and need a fusion
    /// </summary>
    /// <param name="hit">The collision data</param>
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        //I'm always the player, is the other a player? or maybe does not exists
        if(hit.gameObject == null || hit.gameObject.tag != "Player") {
            return;
        }

        //Get the size of the other drop
        CharacterSize otherDropSize = hit.gameObject.GetComponent<CharacterSize>();

        //check who's bigger
        int difference = otherDropSize.GetSize() - GetSize();
        if(difference > 0)
            otherDropSize.DropFusion(gameObject);
        else 
            //I' bigger, or has equal size, so lets go with race condition
            //first called will grow up (at least, this one was called)
            DropFusion(hit.gameObject);
    }
    #endregion

    #endregion
}