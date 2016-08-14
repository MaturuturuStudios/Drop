using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The size of the character. Control if the character can grow up or decrease and how.
/// </summary>
public class CharacterSize : MonoBehaviour {

	#region Public Attributes
	/// <summary>
	/// Initial size of the character
	/// </summary>
	public int initialSize = 1;
	/// <summary>
	/// Decrease rate
	/// </summary>
	public float shrinkSpeed = 5f;
	/// <summary>
	/// Growth rate
	/// </summary>
	public float enlargeSpeed = 5f;
	/// <summary>
	/// Impulse of the drop spitted when growing up
	/// </summary>
	public float impulseSpit = 20f;
	/// <summary>
	/// The time the drop will be still while spitting another drop
	/// </summary>
	public float motionlessTimeSpit = 0.5f;
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
	/// The direction where I spit the drop if I have to when growing up, zero if random
	/// </summary>
	private Vector3 _directionSpitDrop;
	/// <summary>
	/// Set if the drop is in a still situation
	/// </summary>
	private bool _motionless = false;
	/// <summary>
	/// The remaining time of being still
	/// </summary>
	private float _motionlessTime = 0f;
	/// <sumary>
	/// Control if the parameters state was set.
	/// </sumary>
	private bool _setState;
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
	/// 
	/// </summary>
	private LayerMask _layerCast;
	/// <summary>
	/// Independent control to create or remove drops
	/// </summary>
	private GameControllerIndependentControl _independentControl;
	/// <summary>
	/// Controller of the drop
	/// </summary>
	private CharacterControllerCustom _controller;
	/// <summary>
	/// List of listeners subscribed to size changes.
	/// </summary>
	private List<CharacterSizeListener> _listeners;
	#endregion

	#region Methods
	#region "Inherited" Methods
	/// <summary>
	/// Initialization method.
	/// The character start with size one
	/// </summary>
	public void Awake() {
		// Retrieves the desired components
		_layerCast = (1 << LayerMask.NameToLayer("Scene"));
		_dropTransform = gameObject.transform;
		_ratioRadius = GetComponent<CharacterController>().radius;

		_controller = GetComponent<CharacterControllerCustom>();
		_independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
								.GetComponent<GameControllerIndependentControl>();

		// Initialization
		_setState = false;
		_listeners = new List<CharacterSizeListener>();

		//set the motionless situation clear
		_motionless = false;
		_motionlessTime = 0;

		//set the initial size
		if (initialSize <= 0) {
			initialSize = 1;
		}
		ChangeScale(Vector3.one * initialSize);
		_targetSize = initialSize;
		SetSize(initialSize);
	}

	/// <summary>
	/// Check input and control the size
	/// </summary>
	public void Update() {
		GradualModifySize();

		//the drop has to be quiet?
		if (_motionless) {
			//control the time
			_motionlessTime -= Time.deltaTime;
			//done?
			if(_motionlessTime <= 0 && _setState) {
				//recover motion
				_controller.Parameters = null;
				_setState = false;
			}
		}

		if (_motionless && _motionlessTime <= 0) {
			_motionless = false;
			if(_setState){
				_controller.Parameters = null;
				_setState = false;
			}
		} else {
			_motionlessTime -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Set the size when modify values on script
	/// </summary>
	public void OnDrawGizmos() {
		if (!Application.isPlaying)
			Awake();
	}
	#endregion

	#region Public Methods


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
		SetSize(size, Vector3.zero);
	}

	/// <summary>
	/// Set a size. While changing the size, the character will not move
	/// </summary>
	/// <param name="size">New size</param>
	/// <param name="spitDirection">Direction of spit if need to spit a drop</param>
	public void SetSize(int size, Vector3 spitDirection) {
		if(size > 0 && size != _targetSize) {
			_directionSpitDrop = spitDirection;
			//can't move
			if(!_setState){
				_controller.Parameters = CharacterControllerParameters.GrowingParameters;
				_setState = true;
			}

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
	/// Subscribes a listener to the size changes.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(CharacterSizeListener listener) {
		if (_listeners.Contains(listener))
			return false;
		_listeners.Add(listener);
		return true;
	}

	/// <summary>
	/// Unsubscribes a listener to the size changes.
	/// Returns false if the listener wasn't subscribed yet.
	/// </summary>
	/// <param name="listener">The listener to unsubscribe</param>
	/// <returns>If the listener was successfully unsubscribed</returns>
	public bool RemoveListener(CharacterSizeListener listener) {
		if (!_listeners.Contains(listener))
			return false;
		_listeners.Remove(listener);
		return true;
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
	/// Modifies the scale of the object keeping all the constraints.
	/// </summary>
	/// <param name="scale">The new scale of the object</param>
	private void ChangeScale(Vector3 scale) {
		// Notifies of the start of the start of a scale change
		Vector3 previousScale = _dropTransform.localScale;
		foreach (CharacterSizeListener listener in _listeners)
			listener.OnChangeSizeStart(gameObject, previousScale, scale);

		// Modifies the scale
		_dropTransform.localScale = scale;

		// Notifies of the start of the end of a scale change
		foreach (CharacterSizeListener listener in _listeners)
			listener.OnChangeSizeEnd(gameObject, previousScale, scale);
	}

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
				ChangeScale(new Vector3(newScale, newScale, newScale));


			} else {
				//this line is needed. If spitDrop set the size equal to actual size of character
				//GradualModifySize will not enter so never get inside the condition
				//where we recover the movement to the character
				_targettingSize = 0;
				//I can't, get the maximum size and spit the rest
				SpitDrop(newScale, offset);
			}

			//in my size! can move again if there's no time of being quiet
			if(_targettingSize == 0 && !_motionless && _setState) {
				_controller.Parameters = null;
				_setState = false;
			}
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
	/// </summary>
	/// <param name="newScale">the scale which doesn't fit</param>
	/// <param name="newPosition">the position according to the scale</param>
	private void SpitDrop(float newScale, Vector3 offsetCenter) {
		//the principal drop has to be quiet during spitting
		_motionless = true;
		_motionlessTime = motionlessTimeSpit;

		//get the maximum size
		int finalSize = setMaximumSize(newScale, _dropTransform.position + offsetCenter);
		//calculate the extra drop I have to spit
		int numberDropsRemain = _targetSize - finalSize;

		//final position for the spitted drop
		float finalRadius = (finalSize + numberDropsRemain) *_ratioRadius;
		Vector3 position = _dropTransform.position + offsetCenter;
		position += _directionSpitDrop * finalRadius;

		//check in which direction and position I will spit the drop
		Vector3 positionSpit = _dropTransform.position + offsetCenter;
		Vector3 spitDirection = getDirectionSpit(finalSize, numberDropsRemain, positionSpit, out positionSpit);

		//create the drop
		GameObject newDrop = _independentControl.CreateDrop(positionSpit, false);

		//set the position and size
		newDrop.transform.localScale = Vector3.one;
		newDrop.GetComponent<CharacterSize>().SetSize(numberDropsRemain);
		//set a force modified by the size of drop

		CharacterControllerCustom newDropController = newDrop.GetComponent<CharacterControllerCustom>();
		newDropController.SendFlying(spitDirection * impulseSpit * Mathf.Sqrt(numberDropsRemain), false);

		//for test, clarity on behaviour
		_directionSpitDrop = Vector3.zero;

		//set the final size
		SetSize(finalSize);

		// Notifies the listeners
		foreach (CharacterSizeListener listener in _listeners)
			listener.OnSpitDrop(gameObject, newDrop);
	}


	/// <summary>
	/// Precondition: no hole widht can be less than the height of the closest border
	/// Get the direction in which the drop should be spitted
	/// </summary>
	/// <param name="finalSize">The size/number of the drop spitting the remains</param>
	/// <param name="numberDropsSpitted">Size/number of drops being spit</param>
	/// <param name="centerPosition">Center of the main drop</param>
	/// <param name="spitPosition">position where spitted drop should start</param>
	/// <returns>Vector with the direction to spit out</returns>
	private Vector3 getDirectionSpit(int finalSize, int numberDropsSpitted, Vector3 centerPosition, out Vector3 spitPosition) {
		int side = 1;
		//first, check which side, left or right, we have preference for the side where the drop was absorved
		if (_directionSpitDrop != Vector3.zero && _directionSpitDrop.x < 0) {
			side = -1;
		}

		//precalculate...
		RaycastHit raycastHit;
		Vector3 origin = centerPosition;
		spitPosition = origin;
		float distanceCast = (2 + numberDropsSpitted*2 + finalSize) * _ratioRadius;
		float radiusSphereCast = numberDropsSpitted * _ratioRadius;
		float limitDisplacement = _ratioRadius * finalSize * 0.5f;
		//clamp and get the x component
		float displacement = Mathf.Sqrt(Mathf.Clamp(_ratioRadius * 0.5f * (numberDropsSpitted - 1), 0, limitDisplacement));

		//check if i can spit up
		Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * 30*side), Mathf.Cos(Mathf.Deg2Rad * 30), 0);
		Vector3 offset = Vector3.left * side * displacement;
		Debug.DrawRay(origin+offset, direction * distanceCast, Color.green, 2);
		if (!Physics.SphereCast(origin+offset, radiusSphereCast, direction, out raycastHit, distanceCast, _layerCast.value)) {
			spitPosition = origin + offset;
			return direction;
		}


		//if not, check at side
		direction = Vector3.right * side;
		Debug.DrawRay(origin, direction * distanceCast, Color.green, 2);
		if (!Physics.SphereCast(origin, radiusSphereCast, direction, out raycastHit, distanceCast, _layerCast.value)) {
			return direction;
		}

		//if not, check the other side at up
		direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * -30*side), Mathf.Cos(Mathf.Deg2Rad * -30), 0);
		offset = Vector3.right * side * displacement;
		Debug.DrawRay(origin+offset, direction * distanceCast, Color.green, 2);
		if (!Physics.SphereCast(origin+offset, radiusSphereCast, direction, out raycastHit, distanceCast, _layerCast.value)) {
			spitPosition = origin + offset;
			return direction;
		}

		//if not, check at side
		direction = Vector3.left * side;
		Debug.DrawRay(origin, direction * distanceCast, Color.green, 2);
		if (!Physics.SphereCast(origin, radiusSphereCast, direction, out raycastHit, distanceCast, _layerCast.value)) {
			return direction;
		}

		//we are running out of options!
		//check if we have at bottom some space
		origin.y -= ((finalSize - numberDropsSpitted) * _ratioRadius);
		spitPosition = origin;
		direction = Vector3.right * side;
		Debug.DrawRay(origin, direction * distanceCast, Color.green, 2);
		if (!Physics.SphereCast(origin, radiusSphereCast, direction, out raycastHit, distanceCast, _layerCast.value)) {
			return direction;
		}

		direction = Vector3.left * side;
		Debug.DrawRay(origin, direction * distanceCast, Color.green, 2);
		if (!Physics.SphereCast(origin, radiusSphereCast, direction, out raycastHit, distanceCast, _layerCast.value)) {
			return direction;
		}

		//TODO: uh-oh, this can lead to a bug! the drop will be inside of the main drop
		return Vector3.zero;
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
			if(Physics.Raycast(rayOrigin + offset, directionOneSide[i], out hit, distance, _layerCast.value)) {
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
			if(Physics.Raycast(rayOrigin + offset, directionOtherSide[i], out hit, distance, _layerCast.value)) {
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
				if(Physics.Raycast(rayOrigin + offset, directionOneSide[i], out hit, distance, _layerCast.value)) {
					//blocked!
					infoResult.block = true;
				}
			}
		}

		infoResult.offset = offset;
		return infoResult;
	}
	#endregion

	#endregion
}