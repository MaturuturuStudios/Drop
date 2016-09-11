using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class draws the shoot trajectory 
/// </summary>
public class CharacterShootTrajectory : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// Return the size of shoot
    /// </summary>
    public int shootSize {
        get { return (int)_shootSize; }
    }

    /// <summary>
    /// Renderer to use for the trajectory
    /// </summary>
    public new LineRenderer renderer;

    /// <summary>
    /// Prefab of the size indicator
    /// </summary>
    public GameObject trajectorySizeIndicator;

    /// <summary>
    /// Prefab of the particles of rainbow
    /// </summary>
    public GameObject particleRainbow;

    /// <summary>
    /// Number of trajectoyr points we will have
    /// </summary>
    public int numOfTrajectoryPoints = 100;

    /// <summary>
    /// Define the speed of the rainbow when open and close
    /// </summary>
    public float speedRainbow = 10;

    /// <summary>
    /// Define the speed the angle will change
    /// </summary>
    public float speedInputAngle = 1;

    /// <summary>
    /// Layer to indicate with what things the raycast will hit
    /// </summary>
    public LayerMask mask;

    /// <summary>
    /// Indicate how long can reach a shoot
    /// </summary>
    public float limitShoot = 10;

    /// <summary>
    /// Variable to set the animation speed when use j,l or cross in the game pad
    /// </summary>
    public float looking = 3;

    #endregion


    #region Private Attributes
    /// <summary>
    /// Character controller
    /// </summary>
    private CharacterControllerCustom _ccc;
    /// <summary>
    /// Size of the drop
    /// </summary>
    private CharacterSize _sizeDrop;
    /// <summary>
    /// Model of the drop
    /// </summary>
    private CharacterModelController modelDrop;

    /// <summary>
    /// Control if is in shoot mode
    /// </summary>
    private bool _inShootMode = false;

    /// <summary>
    /// Control if the shoot mode is ending
    /// </summary>
    private bool _endScript = false;

    /// <summary>
    /// The size indicator
    /// </summary>
    private GameObject _ballIndicator;

    /// <summary>
    /// A game object where we store the particles of the rainbow
    /// </summary>
    private GameObject _rainParticles;

    /// <summary>
    /// Radius ratio of the rainbow particle
    /// </summary>
    private float _particlesRadiusRatio = 0.5f;

    /// <summary>
    /// Control if the drop is in an animation of the trajectory
    /// </summary>
    private bool _trajectoryAnimation;
    /// <summary>
    /// Check the point collision, so we draw the line until that point
    /// </summary>
    private int _lastColision;
    /// <summary>
    /// When opening or closing the rainbow, this control the progress until we reach the point collision
    /// </summary>
    private float _progressTrajectory;
    /// <summary>
    /// Intermediate value to know the percentage during hiding rainbow
    /// </summary>
    private float _totalProgress;

    /// <summary>
    /// Variable to control the rainbow animation, the final speed calculated with the speedRainbow
    /// </summary>
    private float _speedAnimation;

    /// <summary>
    /// List of positions of the trajectory
    /// </summary>
    private Vector3[] _trajectoryPoints;
    /// <summary>
    /// List of the particles systems of the trajectory
    /// </summary>
    private List<ParticleSystem[]> _particlesTrajectory;

    /// <summary>
    /// Ray cast to know where the last trajectory point hit something
    /// </summary>
    private RaycastHit _hitPoint;

    /// <summary>
    /// Variable to keep the instance of the line renderer
    /// </summary>
    private LineRenderer _linerenderer;
    /// <summary>
    /// Width of the line renderer (rainbow)
    /// </summary>
    private float _lineWidth;
    /// <summary>
    /// Desired width of the line renderer (rainbow)
    /// </summary>
    private float _targetLineWidth;

    /// <summary>
    /// Variable to keep the particle radio when we change the size of the drop we will shooot
    /// </summary>
    private float _radio;
    /// <summary>
    /// Size of the drop that will be shooted
    /// </summary>
    private float _shootSize = 1;
    /// <summary>
    /// Desired target size of the drop to be shooted
    /// </summary>
    private int _targetSize;
    /// <summary>
    /// If we are changing the size of the drop to be shooted
    /// </summary>
    private bool _changeSize = false;

    /// <summary>
    /// Angle of the trajectory that we will changing with the input axis
    /// </summary>
    private float _angle;
    /// <summary>
    /// Desired angle
    /// </summary>
    private float _targetAngle;
    /// <summary>
    /// If we are changing the side of angle or not
    /// </summary>
    private bool _isInLookingAtChange;

    /// <summary>
    /// Vector which contain the information that we need to shoot a drop in the sendflying method
    /// </summary>
    private Vector3 _shootVelocity;
    /// <summary>
    /// Speed that help to calculate the power that the drop will be shooted
    /// </summary>
    private float _speed = 1.0F;
    /// <summary>
    /// Desired speed
    /// </summary>
    private float _targetSpeed = 1f;

    /// <summary>
    /// Max distance of segment lines
    /// </summary>
    private float distanceLineSegments = 80;
    #endregion


    #region Methods

    #region Public Methods
    /// <summary>
    /// Initialize everything
    /// </summary>
    public void Awake() {
        _ccc = GetComponent<CharacterControllerCustom>();
        _sizeDrop = GetComponent<CharacterSize>();
        modelDrop = GetComponentInChildren<CharacterModelController>();

        _trajectoryPoints = new Vector3[numOfTrajectoryPoints];


        for (int i = 0; i < numOfTrajectoryPoints; i++) {
            _trajectoryPoints[i] = transform.position;
        }

        _linerenderer = (LineRenderer)Instantiate(renderer);
        _linerenderer.transform.parent = this.transform;

        //create the object where to store particles
        _rainParticles = new GameObject();
        _rainParticles.transform.parent = this.transform;
        _rainParticles.name = "Rainbow Particle";
        //we init only 20, we usually don't use more. If needed, will create more
        _particlesTrajectory = new List<ParticleSystem[]>(20);
        for (int i = 0; i < 20; i++) {
            GameObject system = Instantiate(particleRainbow);
            system.transform.parent = _rainParticles.transform;

            //initialize particle system
            ParticleSystem[] subSystems = system.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem subSystem in subSystems) {
                subSystem.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                subSystem.Simulate(0, true, true);
                subSystem.Play();

                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = false;
            }
            _particlesTrajectory.Add(subSystems);
        }
    }

    /// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    public void Start() {
        _radio = this.GetComponent<CharacterController>().radius;
        _speed = 0;
        _angle = 45;
        _targetAngle = 45;
    }

    /// <summary>
    /// Method to creat the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
    /// </summary>
    public void EnableTrajectory() {
        _inShootMode = true;

        //correct speed animation with the size of the character
        _speedAnimation = speedRainbow * _sizeDrop.GetSize();

        //enabling trajectory always start with animation
        _trajectoryAnimation = true;
        _progressTrajectory = 0;

        //check if posible to continue with the same size as before
        int max = (_sizeDrop.GetSize() / 2);
        if (_shootSize > max) {
            //no posible? give the maximum
            _shootSize = max;
        }
        _lineWidth = _shootSize;
        _targetLineWidth = _lineWidth;
        _linerenderer.SetWidth(_lineWidth, _lineWidth);

        //speed for the line trajectory
        _speed = Mathf.Sqrt((limitShoot * (_sizeDrop.GetSize() - shootSize))
                                * _ccc.Parameters.Gravity.magnitude);
        _targetSpeed = _speed;

        _lastColision = 0;
        _progressTrajectory = 0;

        //reset all states
        _changeSize = false;
        _isInLookingAtChange = false;
        _endScript = false;

        //correct the angle with the direction of the drop
        float angleLook = modelDrop.GetLookingDirection();
        if (angleLook > 0 && _angle > 90) {
            _angle = _angle - 90;
            _angle = 90 - _angle;
        }

        if (angleLook < 0 && _angle < 90) {
            _angle = 180 - (_angle);
        }

        if (_ballIndicator != null) Destroy(_ballIndicator);
        _ballIndicator = Instantiate(trajectorySizeIndicator);
        //set the size of ball
        _ballIndicator.transform.localScale = new Vector3(_shootSize, _shootSize, _shootSize);
        _rainParticles.SetActive(true);


        //set initial data
        Vector3 position = this.transform.position;
        //set the trajectory points
        SetTrajectoryPoints(position, _angle, _speed);
        //update the indicator position
        UpdateIndicator();
    }

    /// <summary>
	/// Method to destroy the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
	/// </summary>
    public void DisableTrajectory() {
        _inShootMode = false;

        //clear the line trajectory
        _linerenderer.SetVertexCount(0);
        _lastColision = 0;

        //reset all states
        _changeSize = false;
        _isInLookingAtChange = false;
        _endScript = false;
        _trajectoryAnimation = false;

        Destroy(_ballIndicator);
        //disable particles
        for (int i = 0; i < _particlesTrajectory.Count; i++) {
            ParticleSystem[] subSystems = _particlesTrajectory[i];
            for (int j = 0; j < subSystems.Length; j++) {
                ParticleSystem.EmissionModule emission = subSystems[j].emission;
                emission.enabled = false;
            }
        }
    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    public void Update() {
        //shoot mode not available
        if (!_inShootMode) return;

        if (_endScript) {
            //set the quit animation
            QuitTrajectory();

        } else {
            Vector3 position = this.transform.position;
            _speed = Mathf.MoveTowards(_speed, _targetSpeed, 5 * Time.deltaTime);

            //update the angle
            UpdateAngle();
            //update the change of size
            UpdateSize();
            //set the trajectory points
            SetTrajectoryPoints(position, _angle, _speed);

            //set the visibility of the trajectory
            SetVisibility();
            //check if is posible to shoot in this concrete angle and position
            CanShoot();

            //if in animation of start shooting (open rainbow)
            OpenTrajectory();
        }

        //update the indicator position
        UpdateIndicator();
        //update the particles
        ParticlesRainbow();
        //draw the trajectory
        DrawLineRenderer();
    }

    /// <summary>
    /// Tell where is the shooting indicator
    /// </summary>
    /// <returns></returns>
    public Vector3 GetIndicatorPosition() {
        return _ballIndicator.transform.position;
    }

    /// <summary>
    /// Return the velocity of the drop to be shooted
    /// </summary>
    /// <returns></returns>
    public Vector3 GetVelocityDrop() {
        return _shootVelocity;
    }

    /// <summary>
    /// Return true if is in shoot mode
    /// </summary>
    /// <returns></returns>
    public bool IsInShootMode() {
        return _inShootMode;
    }

    /// <summary>
    /// True if is changing size or just in a middle of animation
    /// </summary>
    /// <returns></returns>
    public bool IsInAnimation() {
        return _changeSize || _trajectoryAnimation || _isInLookingAtChange;
    }

    /// <summary>
    /// Increase the size of the drop to shoot if posible
    /// </summary>
    public void IncreaseSize() {
        //in the middle of animation does not react
        if (IsInAnimation()) return;
        if (_shootSize + 1 <= (_sizeDrop.GetSize() / 2)) {
            SelectSize((int)_shootSize + 1);
        }
    }

    /// <summary>
    /// Decrease the size of the drop to shoot if posible
    /// </summary>
    public void DecreaseSize() {
        //in the middle of animation does not react
        if (IsInAnimation()) return;
        if (_shootSize > 1) {
            SelectSize((int)_shootSize - 1);
        }
    }

    /// <summary>
    /// Change at right if not already changing
    /// </summary>
    public void LookAtRight() {
        if (_isInLookingAtChange) return;
        if (_angle < 90) {
            _isInLookingAtChange = true;
            _targetAngle = 180 - _angle;
        }
    }

    /// <summary>
    /// Change at left if not already changing
    /// </summary>
    public void LookAtLeft() {
        if (_isInLookingAtChange) return;
        if (_angle > 90) {
            _isInLookingAtChange = true;
            _targetAngle = 180 - _angle;
        }
    }

    /// <summary>
    /// Set the ending of the shoot mode, start the animation of pick up the rainbow trajectory
    /// </summary>
    public void EndShootMode() {
        if (_endScript) return; //already ending
        _endScript = true;
        _progressTrajectory = _lastColision;
        _totalProgress=_lastColision;
    }

    /// <summary>
    /// Quit the shoot mode abruptly, no come back animation
    /// for example, when shooting the drop
    /// </summary>
    public void QuitShootMode() {
        _inShootMode = false;
        DisableTrajectory();
    }

    /// <summary>
    /// To know the actual angle of the shootmode
    /// </summary>
    public float GetActualAngle() {
        return _angle;
    }

    /// <summary>
    /// Check if I can shoot (there is no collider between the drop
    /// and the first line of the trajectory)
    /// </summary>
    /// <returns></returns>
    public bool CanShoot() {
        Vector3 direction = _trajectoryPoints[0] - transform.position;
        float distance = direction.magnitude;

        bool result = true;
        if (Physics.SphereCast(transform.position, _radio, direction, out _hitPoint, distance, mask)) {
            _lastColision = 0;
            _ballIndicator.SetActive(false);
            result = false;
        } else {
            _ballIndicator.SetActive(true);
        }
        return result;
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Change the size of the drop to be shooted
    /// </summary>
    /// <param name="size"></param>
    private void SelectSize(int size) {
        if (size == _shootSize) return;
        _shootSize = size;
        _changeSize = true;

        //adjust line width with the size
        _targetLineWidth = size;
        _targetSize = size;

        //with the change of the size to shoot, the speed shooting the drop
        //will change too
        _targetSpeed = Mathf.Sqrt((limitShoot * (_sizeDrop.GetSize() - _shootSize))
                             * _ccc.Parameters.Gravity.magnitude);


        //update size of particles
        for (int i = 0; i < _particlesTrajectory.Count; i++) {
            ParticleSystem[] subSystems = _particlesTrajectory[i];
            for (int j = 0; j < subSystems.Length; j++) {
                ParticleSystem.ShapeModule shape = subSystems[j].shape;
                shape.radius = _targetSize * _particlesRadiusRatio;
            }
        }
    }

    /// <summary>
    /// Update the position of the indicator
    /// </summary>
    private void UpdateIndicator() {
        Vector3 finalPosition = Vector3.one;
        float displacement = _ballIndicator.transform.lossyScale.x * (_radio);

        //when hiding rainbow or opening it
        if (_endScript || _trajectoryAnimation) {
            int integerPart = (int)_progressTrajectory;
            if (integerPart < 0) integerPart = 0;
            float remain = _progressTrajectory - integerPart;
            Vector3 nextPoint = _trajectoryPoints[integerPart + 1];

            //in the last part of the trajectory don't get the next point in trajectory
            //get the hit point of collision to get the right lerp
            if ((_endScript && integerPart==(int)_totalProgress-1) ||
                (!_endScript && integerPart==_lastColision-1)) {
                nextPoint = _hitPoint.point;
            }
            //we have the point of the indicator
            if (integerPart < 0) return;
            finalPosition = Vector3.Lerp(_trajectoryPoints[integerPart], nextPoint, remain);

            //we need the offset because of the radius of the indicator
            //will make the correction progresive because the offset applied when the indicator 
            //is in his final position is not adecuate at the first moment
            Vector3 move = _hitPoint.normal * displacement;
            //get the percentage of the trajectory
            float percentage = 0;
            if (_endScript && _totalProgress!=0) {
                percentage = _progressTrajectory / (_totalProgress);
             } else if (_endScript && _lastColision!=0){
                percentage = _progressTrajectory / (_lastColision * 1f);
            }
            //ease the displacement...
            percentage = Mathf.Pow(percentage,5);
            //the final displacement
            move = Vector3.Lerp(Vector3.zero, move, percentage);

            finalPosition += move;

        } else {
            finalPosition = _hitPoint.point;
            finalPosition += _hitPoint.normal * displacement;
        }

        _ballIndicator.transform.position = finalPosition;
    }

    /// <summary>
    /// Update and draw the particles
    /// </summary>
    private void ParticlesRainbow() {
        //check if we have enough particles
        int rest = _lastColision - _particlesTrajectory.Count;
        for (int i = 0; i < rest; i++) {
            GameObject system = Instantiate(particleRainbow);
            system.transform.parent = _rainParticles.transform;

            //initialize particle system
            ParticleSystem[] subSystems = system.GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < subSystems.Length; j++) {
                ParticleSystem subSystem = subSystems[j];
                subSystem.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                subSystem.Simulate(0, true, true);
                subSystem.Play();

                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = false;

                ParticleSystem.ShapeModule shape = subSystem.shape;
                int size = (_changeSize) ? _targetSize : (int)_shootSize;
                shape.radius = size * _particlesRadiusRatio;
            }
            _particlesTrajectory.Add(subSystems);
        }

        //time to draw
        float increment = 1f / (numOfTrajectoryPoints);
        for (int i = 0; i < _lastColision; i++) {
            float x = i * increment;

            //set position
            Transform parent = _particlesTrajectory[i][0].gameObject.transform.parent;
            parent.transform.position = particleRainbow.transform.InverseTransformPoint(_trajectoryPoints[i]);

            //enable
            ParticleSystem[] subSystems = _particlesTrajectory[i];
            for (int j = 0; j < subSystems.Length; j++) {
                ParticleSystem.EmissionModule emission = subSystems[j].emission;
                emission.enabled = true;

                subSystems[j].startColor = new Color(x, 0f, 0f);
            }
        }

        //disable the rest of particles
        for (int i = _lastColision; i < _particlesTrajectory.Count; i++) {
            if (i < 0) {
                i = 0;
                continue;
            }
            ParticleSystem[] subSystems = _particlesTrajectory[i];
            for (int j = 0; j < subSystems.Length; j++) {
                ParticleSystem.EmissionModule emission = subSystems[j].emission;
                emission.enabled = false;
            }

        }
    }

    /// <summary>
    /// Update the size of the shoot size and the indicator
    /// </summary>
    private void UpdateSize() {
        if (!_changeSize) return;

        _shootSize = Mathf.MoveTowards(_shootSize, _targetSize, Time.deltaTime);
        //update indicator size
        _ballIndicator.transform.localScale = new Vector3(_shootSize, _shootSize, _shootSize);
        //with the change of the size, we need to update the velocity vector too
        _speed = Mathf.MoveTowards(_speed, _targetSpeed, 5 * Time.deltaTime);

        //smooth the change of line renderer
        _lineWidth = Mathf.MoveTowards(_lineWidth, _targetLineWidth, Time.deltaTime);
        _linerenderer.SetWidth(_lineWidth, _lineWidth);

        if (_targetSpeed == _speed) _changeSize = false;
    }

    /// <summary>
    /// Retire the trajectory
    /// </summary>
    private void QuitTrajectory() {
        if (_progressTrajectory <= 0) {
            //finished
            DisableTrajectory();
            //Warning listener on character shoot
            this.GetComponent<CharacterShoot>().ShootModeEnded();
            _ccc.Parameters = null;
            _progressTrajectory = 0;
        } else {
            _progressTrajectory -= _speedAnimation * Time.deltaTime;
            _lastColision = (int)_progressTrajectory;
        }
    }

    ///  <summary>
    /// This fuctions draw the particle trip along the trajectory
    /// </summary>
    public void OpenTrajectory() {
        if (!_trajectoryAnimation) return;
        _progressTrajectory += _speedAnimation * Time.deltaTime;

        //if progress reach the point collision, is over
        if (_progressTrajectory >= _lastColision) {
            _trajectoryAnimation = false;
        }
    }

    /// <summary>
    /// Update the angle of the trajectory
    /// </summary>
    private void UpdateAngle() {
        //while changing, don't see the input
        if (!_isInLookingAtChange) {
            //get input
            float _h = Input.GetAxis(Axis.Horizontal) * speedInputAngle;
            float _v = Input.GetAxis(Axis.Vertical) * speedInputAngle;

            //modify first horizontal
            _angle -= _h;
            //correct angle with vertical if needed
            if (_angle < 90) _angle += _v;
            if (_angle > 90) _angle -= _v;
            //correct limits
            if (_angle < 0) _angle = 0;
            if (_angle > 180) _angle = 180;

        } else {
            //update the looking at movement
            _angle = Mathf.MoveTowards(_angle, _targetAngle, 20 * looking * Time.deltaTime);
            if (_angle == _targetAngle) _isInLookingAtChange = false;
        }
    }

    ///  <summary>
    /// This fuctions calculate the points of the trajectory and the shoot vector
    /// </summary>
    private void SetTrajectoryPoints(Vector3 pStartPosition, float angle, float speed) {
        //calculate the end vector of the trajectory
        _shootVelocity = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * speed, Mathf.Sin(angle * Mathf.Deg2Rad) * speed, 0);
        //magnitud of the velocity to calculate de distance que es igual al valor de speed
        float velocity = Mathf.Sqrt((_shootVelocity.x * _shootVelocity.x) + (_shootVelocity.y * _shootVelocity.y));

        float fTime = 0;

        fTime += 0.1f;
        _trajectoryPoints[0] = this.transform.position;

        for (int i = 1; i < numOfTrajectoryPoints; i++) {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (_ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
            _trajectoryPoints[i] = Vector3.MoveTowards(_trajectoryPoints[i], pos, distanceLineSegments);
            fTime += 0.1f;
        }

    }

    ///  <summary>
    /// This fuction check the longitude of the trajectory to be draw
    /// </summary>
    private void SetVisibility() {
        float dis = 0;
        bool _colisiondetected = false;
        _lastColision = numOfTrajectoryPoints;

        for (int i = 0; i < numOfTrajectoryPoints - 1 && !_colisiondetected; i++) {
            Vector3 _fwd = _trajectoryPoints[i + 1] - _trajectoryPoints[i];
            dis = _fwd.magnitude;
            if ((Physics.SphereCast(_trajectoryPoints[i], _radio, _fwd, out _hitPoint, dis, mask))) {
                _colisiondetected = true;
                _lastColision = i + 1;
            }
            Debug.DrawRay(_trajectoryPoints[i], _fwd, Color.green);
        }
    }

    /// <summary>
    /// Functions to draw the rainbow renderer
    /// </summary>
    private void DrawLineRenderer() {
        int last = _lastColision;

        if (_trajectoryAnimation)
            last = (int)_progressTrajectory+1;

        if (last < 0) last = 0;

        // adjust the line renderer to the real longitude
        //known by the last colision
        //plus one because one special segment
        _linerenderer.SetVertexCount(last);

        //get the points position to renderer
        for (int i = 0; i < last; ++i) {
            _linerenderer.SetPosition(i, _trajectoryPoints[i]);
        }

        //adjust the last point
        //make sure we have at least two points (zero means only one point in line renderer, no segment at all)
        if (last > 0) {
            Vector3 lastPosition;
            if (_trajectoryAnimation || _endScript) {
                int integerPart = (int)_progressTrajectory;
                float remain = _progressTrajectory - integerPart;
                lastPosition = Vector3.Lerp(_trajectoryPoints[integerPart], _trajectoryPoints[integerPart + 1], remain);
                
            } else {
                lastPosition = _hitPoint.point;
            }

            _linerenderer.SetPosition(last-1, lastPosition);
        }
    }
    #endregion

    #endregion
}