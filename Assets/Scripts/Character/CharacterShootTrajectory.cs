using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class draws the shoot trajectory 
/// </summary>
public class CharacterShootTrajectory : MonoBehaviour {
    #region Private Attributes

    /// <summary>
    /// Variables to keep size shoot of the drop shooted and line renderer width 
    /// </summary>
    private float _linewidth;
    private float _sizeindicator;
    private float _particlesizekeeped;

    /// <summary>
    /// Variable to knonw if we pressed the horizontal cross 
    /// </summary>
    private List<bool> _boolrender;
    /// <summary>
    /// Variable to knonw if we pressed the horizontal cross 
    /// </summary>
    private bool  _lookingat=false;
    private float _oldLookingat;

    /// <summary>
    /// Variable to control the rainbow animation 
    /// </summary>
    private float _speedAnimation;

    /// <summary>
    /// Radious of the rainbow particle
    /// </summary>
    private float _particlerainbowradious = 0.5f;

    /// <summary>
    /// Array of particles system
    /// </summary>
    private ParticleSystem.Particle[] _points;

    /// <summary>
    /// Auxiliar variable particlesystem
    /// </summary>
    private ParticleSystem _rain;

    /// <summary>
    /// List of particlesystem in the rainbow
    /// </summary>
    // private List<ParticleSystem[]> _lluvia;
    private List<ParticleSystem[]> _trajectoryPoints;

    /// <summary>
    /// Instance of the public variable rainbow particle
    /// </summary>
    private GameObject _rainparticle;

    /// <summary>
    /// Variable used when we shooted
    /// </summary>
    private bool _finish = false;

    /// <summary>
    /// Variable to control the star and end shootmode animation
    /// </summary>
    private bool _animshot = true;

    /// <summary>
    /// Variable used when we shooted
    /// </summary>
    private bool _endscript = false;

    /// <summary>
    /// Variable to keep the particle radio when we change the size of the drop we will shooot
    /// </summary>
    private float _radio;

    /// <summary>
    /// Variable to keep the old speed when we change the size of the drop we will shoot
    /// </summary>
    private float _oldspeed;


    private bool _moving = false;

    /// <summary>
    /// Variable to keep the old vector oof the drop the will be shooted when we change the size of the drop we will shoot
    /// </summary>
    private Vector3 _oldpVelocity;

    /// <summary>
    /// Variable to draw the rainbow when we change the size of the drop in the shootmode
    /// </summary>
    private bool _retrajectoring = false;

    /// <summary>
    /// Variable to keep the old size when we change the size of the drop we will shoot
    /// </summary>
    private float _oldsize;

    /// <summary>
    /// Variable to keep the instance of the line renderer
    /// </summary>
    private LineRenderer _linerenderer;

    /// <summary>
    /// Auxiliar list to keep the information of thee line renderer when we are calculating the rainbow trajectory
    /// </summary>
    private List<Vector3> _aux ;

    /// <summary>
    /// Variable to knonw if we changed the size of the drop in the shoootmode to start the retrajectoring animagion
    /// </summary>
    private bool _sizeanimation;

    /// <summary>
    /// Variable to keep the instance of the trajectory
    /// </summary>
    private GameObject _listtrajectory;

    /// <summary>
    /// Variable used to change the width of the rainbow renderer when we change the size of the dropp shooted
    /// </summary>
    private float _oldrenderwidth,_renderwidth;

    /// <summary>
    /// Variable to know the angle where the eyes are looking
    /// </summary>
    private float _anglelook;

    /// <summary>
    /// This is to draw the animation of the particle that move throught the trajectory to do an effect similar to MoveTowrds or Lerp.
    /// </summary>
    private float _journeyLength;
    private float _faction_of_path_traveled;
    private float _faction_of_traveled;
    private int _lastWaypoint, _nextWaypoint, _finalWaypoint;
    private int _finallastWaypoint, _finalnextWaypoint;

    /// <summary>
    /// This is the arrays of the trajectory points
    /// </summary>
   // private List<GameObject> _trajectoryPoints;

    /// <summary>
    /// These are the scripts objects
    /// </summary>
    private CharacterControllerCustom _ccc;

    /// <summary>
    /// Ray cast to know where the trajectory points are hitting
    /// </summary>
    private RaycastHit _hit;

    /// <summary>
    /// Vector which contain the information that we need to shoot a drop in the sendflying method
    /// </summary>
    private Vector3 _shootVelocity;

    /// <summary>
    /// Size of the drop that will be shooted
    /// </summary>
    public float _shootsize = 1;	// Changed by Nacho: Other objects may need to read this

    /// <summary>
    /// Vector auxiliar to keep data
    /// </summary>
    private Vector3 _fwd;

    /// <summary>
    /// Boolean to know if the raycast hitted something
    /// </summary>
    private bool _colisiondetected = false;

    /// <summary>
    /// Objects that represent the sphere that travel around the trajectory and the ball that indicate the size of the drop shooted
    /// </summary>
    private GameObject _sphere, _ball;

    /// <summary>
    /// Float that catch the Axis Inputs
    /// </summary>
    private float _h, _v;

    /// <summary>
    /// Float to indiccate if we changed the size of the drop shooted
    /// </summary>
    private bool _selecting = false;

    /// <summary>
    /// Variable to keep data
    /// </summary>
    private float _velocity = 1;

    /// <summary>
    /// Ray cast to know where the last trajectory point hit something
    /// </summary>
    private RaycastHit _hitpoint;

    /// <summary>
    /// Angle of the trajectory that we will changing with the input axis
    /// </summary>
    private float _angle;

    /// <summary>
    /// Speed that help to calculate the power that the drop will be shooted
    /// </summary>
    private float _speed = 1.0F;

    #endregion

    #region Public Attributes

    public GameObject particleRainbow;

    public new LineRenderer renderer;

    /// <summary>
    /// Variable to set the animation speed when use j,l or cross in the game pad
    /// </summary>
    public float looking=1;

    /// <summary>
    /// Prefab of the trajectory points
    /// </summary>
    public GameObject TrajectoryPointPrefeb;

    /// <summary>
    ///Prefab of the size indicator at the end of the trajectory path
    /// </summary>
    public GameObject TrajectorySizeIndicator;

    /// <summary>
    /// Prefab of the particle that travel in the trajectory path
    /// </summary>
    public GameObject TrajectoryParticlePrefeb;

    /// <summary>
    /// Number of trajectoyr points we will have
    /// </summary>
    public int numOfTrajectoryPoints = 100;

    /// <summary>
    /// Speed of the particle that travel in the trajectory path
    /// </summary>
    public float particletrajectoryspeed = 0.08f;

    /// <summary>
    /// Layer to indicate with what things the raycast will hit
    /// </summary>
    public LayerMask mask;

    public float speedrainbow = 10;
    /// <summary>
    /// Variable to calculate the max distance of the trajectory path
    /// </summary>
    public float limitshoot = 5;

    #endregion

    #region Methods

    /// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    void Start() {      
        this.enabled = false;

        _linewidth = 1;
        _sizeindicator = 1;

        _speed = 0;
        _oldspeed = 0;
        _radio = this.GetComponent<CharacterController>().radius;
      
        _aux = new List<Vector3>();

        _angle = 45;

        _ccc = GetComponent<CharacterControllerCustom>();

        _points = new ParticleSystem.Particle[numOfTrajectoryPoints];
        //_lluvia = new List<ParticleSystem[]>();
        //_rainparticle.transform.parent = ccc.transform;
        _rainparticle = new GameObject();
        _rainparticle.transform.parent= _ccc.transform; 
        _rainparticle.name = " Rainbow Particle ";
        //_lluvia.Clear();

        _trajectoryPoints = new List<ParticleSystem[]>();
        _boolrender = new List<bool>();

        for (int i = 0; i < numOfTrajectoryPoints; i++){
            GameObject system = Instantiate(particleRainbow);
            system.transform.parent = _rainparticle.transform;

            ParticleSystem[] subSystems = system.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem subSystem in subSystems) {
                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = false;         
            }
            _trajectoryPoints.Insert(i, subSystems);
            _boolrender.Insert(i, false);

        }

        _linerenderer = (LineRenderer) Instantiate(renderer);
        _linerenderer.transform.parent = _ccc.transform;

        _renderwidth = _linewidth;

        _lastWaypoint = 0;
        _nextWaypoint = 1;
        _finalWaypoint = _trajectoryPoints.Capacity;
    }

    /// <summary>
	/// Method to know if we changed the size of the drop shooted
	/// </summary>
    public void selectingsize(float size) {
        _oldspeed = Mathf.Sqrt((limitshoot * (_ccc.GetComponent<CharacterSize>().GetSize() - _shootsize)) * _ccc.Parameters.Gravity.magnitude);
        _oldsize = _shootsize;
        _shootsize = size;
        _selecting = true;

        _oldrenderwidth = _renderwidth;

        if (_oldsize > size) {
            _renderwidth-=1;
        }else if( size >_oldsize) {
            _renderwidth+=1;
        }

        if (_oldsize > _shootsize) _particlerainbowradious -= 0.5f;
        else if (_shootsize > _oldsize)_particlerainbowradious += 0.5f;

        for (int i = 0; i < numOfTrajectoryPoints; i++) {
            if (_boolrender[i] ) {
                foreach (ParticleSystem subSystem in _trajectoryPoints[i]) {
                    ParticleSystem.ShapeModule shape = subSystem.shape;
                    shape.radius = _particlerainbowradious;
                }
            }
        }
    }

    /// <summary>
    /// Method to creat the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
    /// </summary>
    public void OnEnable() {
       
        _renderwidth = _linewidth;

        _speedAnimation = speedrainbow*this.GetComponent<CharacterSize>().GetSize();

        _finish = false;

        _shootsize = _sizeindicator;
        _endscript = false;
        
        _sphere = (GameObject)Instantiate(TrajectoryParticlePrefeb);
        _sphere.transform.localScale = new Vector3(_shootsize, _shootsize, _shootsize);
        _sphere.SetActive(false);

        _ball = (GameObject)Instantiate(TrajectorySizeIndicator);
        //ball.GetComponent<Transform>().parent = this.transform;
        _ball.transform.localScale = new Vector3(_shootsize, _shootsize, _shootsize);
        _ball.SetActive(false);
    
        _nextWaypoint = 1;
        _lastWaypoint = 0;
    
        _animshot = true;
        _sizeanimation = false;

        _anglelook=this.GetComponentInChildren<CharacterModelController>().GetLookingDirection();

        if (_anglelook > 0 && _angle >90) {
            _angle = _angle-90;
            _angle = 90 - _angle;

        }
        if (_anglelook < 0 && _angle<90) {
            _angle = 180 - ( _angle);
        }      
    }

    /// <summary>
	/// Method to destroy the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
	/// </summary>
    public void OnDisable() {

        _animshot = false;
        //_renderwidth = 1;
     
        _sizeindicator=_shootsize;

        if (_ball != null)
            _ball.SetActive(false);

        if (_sphere != null)
            _sphere.SetActive(false);
      
        Destroy(_sphere);
        Destroy(_ball);
        //Destroy(rainparticle);

    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    void Update(){
        if (_endscript) {
            _animshot = true;
            QuitTrajectory();
            Drawlinerenderer();           
        }
        else {
            if (!_animshot) {
                if (!_retrajectoring) {
                    if (_selecting)  {                      
                        _selecting = false;
                        _moving = true;
                    }

                    _h = Input.GetAxis(Axis.Horizontal);
                    _v = Input.GetAxis(Axis.Vertical);

                    _angle -= _h;
                    
                    if (_angle < 90) {
                        _angle += _v;
                    }

                    if (_angle > 90) _angle -= _v;
                    if (_angle < 0) _angle = 0;
                    if (_angle > 180) _angle = 180;                
                }
            }         

            //Calculate the vector from the drop  where  be shooted 
            Vector3 pos = this.transform.position;// + GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x + ball.GetComponent<SphereCollider>().radius* ball.transform.lossyScale.x);
                                                  //The power of the shoot
            _speed = Mathf.Sqrt((limitshoot * (_ccc.GetComponent<CharacterSize>().GetSize() - _shootsize)) * _ccc.Parameters.Gravity.magnitude);

            if (_moving) {
                _sizeanimation = true;

                _retrajectoring = true;
                _oldspeed = Mathf.MoveTowards(_oldspeed, _speed, 5 * Time.deltaTime); 
                _oldsize= Mathf.MoveTowards(_oldsize, _shootsize, Time.deltaTime);

                _ball.transform.localScale = new Vector3(_oldsize, _oldsize, _oldsize);
                _sphere.transform.localScale=new Vector3(_oldsize, _oldsize, _oldsize);

                setTrajectoryPoints(pos, _angle, _oldspeed);

                _oldrenderwidth = Mathf.MoveTowards(_oldrenderwidth, _renderwidth, Time.deltaTime);               

                _linerenderer.SetWidth(_oldrenderwidth, _oldrenderwidth);
             
                if (_oldspeed == _speed){
                    _moving = false;
                    _sizeanimation = false;
                }
            }else{
                //animshot = false;
                setTrajectoryPoints(pos, _angle, _speed);
                _retrajectoring = false;
            }
            if (_lookingat) {
                _oldLookingat = Mathf.MoveTowards(_oldLookingat, _angle, 20 * looking * Time.deltaTime);

                setTrajectoryPoints(pos, _oldLookingat, _speed);

                if (_oldLookingat == _angle){
                    _lookingat = false;
                }
            } 
            
            Setvisibility();
            Canshooot();
            
            Drawlinerenderer();
            
        }
        ParticleTrip();
        ParticleRainbow();
    }


    /// <summary>
    /// This fuctions calculate if there is a colision with the raycast of the first shoot trajectory  before it
    /// </summary>
    public bool Canshooot() {
        float dis = 0;
        Vector3 spheredis = transform.position;//+ GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x);
        _fwd =_trajectoryPoints[0][0].transform.position - spheredis;

        dis = _fwd.magnitude;

        Debug.DrawRay(spheredis, _fwd, Color.green);
     
        if (Physics.SphereCast(spheredis,_radio, _fwd,out _hitpoint, dis, mask)) {
            _ball.SetActive(false);
            _sphere.SetActive( false);
            for (int j = 0; j < numOfTrajectoryPoints - 1; j++) {
                _boolrender[j] = false;
            }
            return false;

        }
        else {
            _sphere.SetActive(true);
           
            return true;
        }
    }

    /// <summary>
    /// Fuctions used when we use the horizontal inputo of the gamepad
    /// </summary>
    public void LookatRight() {
        if (_angle < 90) {
            //_animshot = true;
            _lookingat = true;
            _oldLookingat = _angle;
            _angle = 180 - _angle;
        }   
    }

    /// <summary>
    /// Fuctions used when we use the horizontal inputo of the gamepad
    /// </summary>
    public void LookatLeft(){
        if (_angle > 90) {
            //_animshot = true;
            _lookingat = true;
            _oldLookingat = _angle;
            _angle = 180 - _angle;
        }
    }

    /// <summary>
    /// Return if drop is turning to look to another direction
    /// </summary>
    public bool Lookingat(){
        return _lookingat;
    }

    /// <summary>
    /// To know if we are doing the size animation to not to be avaible to shoot in the shootmode
    /// </summary>
    public bool SizeAnimation(){
        return _sizeanimation;
    }

    /// <summary>
    /// To end the shootmode
    /// </summary>
    public void Endingd() {       
        _endscript = true;
    }

    /// <summary>
    /// To know if the shootmode is ending and to not to be avaible to shoot in the shootmode
    /// </summary>
    public bool Isending() {
        return _finish;
    }

    /// <summary>
    /// To know if the shootmode is in his animation and to not to be avaible to shoot in the shootmode
    /// </summary>
    public bool Animation() {
        return _animshot;
    }

    /// <summary>
    /// To know the actual angle of the shootmode
    /// </summary>
    public float Angle(){
        return _angle;

    }

    /// <summary>
    /// Cleaning all the visual information of the shootmode
    /// </summary>
    public void Finishing(){      

        for (int i = 0; i < numOfTrajectoryPoints; i++){
            _boolrender[i] = false;
            foreach (ParticleSystem subSystem in _trajectoryPoints[i]) {
                //subSystem.simulationSpace = ParticleSystemSimulationSpace.World;
                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = false;
            }
        }
        _linerenderer.SetVertexCount(0);
        _linerenderer.SetWidth(1, 1);
        _shootsize = 1;
        _linewidth = 1;
        _sphere.SetActive( false);
        _ball.SetActive(false);
        //Destroy(rainparticle);

    }

    /// <summary>
    /// This fuctions delete the trajectory
    /// </summary>
    public void QuitTrajectory() {

        for (int i = 0; i < numOfTrajectoryPoints; i++) {
            if (_boolrender[i]) {
                foreach (ParticleSystem subSystem in _trajectoryPoints[i]) {
                    //subSystem.simulationSpace = ParticleSystemSimulationSpace.World;
                    ParticleSystem.ShapeModule shape = subSystem.shape;
                    shape.radius = 0.5f;
                }
            }
        }

        if (_finalnextWaypoint ==0) {           
            _boolrender[_finalnextWaypoint] = false;
            _animshot = false;
            _linewidth=_renderwidth;
            _linerenderer.SetWidth(_linewidth, _linewidth);
            this.GetComponent<CharacterShoot>().Endshootmode();
            _ccc.Parameters = null;
            this.enabled = false;
        }

        Vector3 fullPath = _trajectoryPoints[_finalnextWaypoint][0].transform.position - _trajectoryPoints[_finallastWaypoint][0].transform.position; //defines the path between lastWaypoint and nextWaypoint as a Vector3
        _faction_of_traveled += _speedAnimation * Time.deltaTime; //animate along the path
        
        if (_faction_of_traveled > 1){ //move to next waypoint
            _finallastWaypoint--;
            _finalnextWaypoint--;
            _faction_of_traveled = 0;         
        }
            //ball.transform.position = (fullPath * 2) + trajectoryPoints[lastWaypoint].transform.position;
        _ball.transform.position = (fullPath * _faction_of_traveled) + _trajectoryPoints[_finallastWaypoint][0].transform.position;
        _boolrender[_finallastWaypoint] = false;

        if (_ball.transform.position.x <= _sphere.transform.position.x && _angle <90){
            _sphere.SetActive(false);                  
        }
        else if (_ball.transform.position.x >= _sphere.transform.position.x && _angle>90){
            _sphere.SetActive(false);         
        }     
    }

    ///  <summary>
    /// This fuctions return the shoot vector for the shoot script
    /// </summary>
    public Vector3 GetpVelocity(){
        return _shootVelocity; 
    }

    ///  <summary>
    /// This fuctions calculate the points of the trajectory and the shoot vector which is pVelocity
    /// </summary>
    void setTrajectoryPoints(Vector3 pStartPosition, float angle, float speed){
        //calculate the end vector of the trajectory
        _shootVelocity = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * speed, Mathf.Sin(angle * Mathf.Deg2Rad) * speed, 0);
        //magnitud of the pVelocity to calculate de distance que es igual al valor de speed
        _velocity = Mathf.Sqrt((_shootVelocity.x * _shootVelocity.x) + (_shootVelocity.y * _shootVelocity.y));

       float fTime = 0;
       fTime += 0.1f;

        _trajectoryPoints[0][0].transform.position = this.transform.position;

        for (int i = 1; i < numOfTrajectoryPoints; i++){
            float dx = _velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = _velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (_ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
             _trajectoryPoints[i][0].transform.position = Vector3.MoveTowards(_trajectoryPoints[i][0].transform.position, pos, 100);
           // trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
            _trajectoryPoints[i][0].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(_shootVelocity.y - (_ccc.Parameters.Gravity.magnitude) * fTime, _shootVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }

    }

    ///  <summary>
    /// This fuctions draw the particle trip along the trajectory
    /// </summary>
    public void ParticleTrip() {       
        if (_nextWaypoint > _finalWaypoint){
                _nextWaypoint = 1;
                _lastWaypoint = 0;
                _animshot = false;
            }

        Vector3 fullPath = _trajectoryPoints[_nextWaypoint][0].transform.position - _trajectoryPoints[_lastWaypoint][0].transform.position; //defines the path between lastWaypoint and nextWaypoint as a Vector3
        if(_animshot && !_endscript) _faction_of_path_traveled += _speedAnimation * Time.deltaTime; //animate along the path
        else _faction_of_path_traveled += particletrajectoryspeed * Time.deltaTime;
           
        if (_animshot && !_endscript && Canshooot()){           
            _ball.SetActive(true);
            //ball.transform.position = (fullPath * 2) + trajectoryPoints[lastWaypoint].transform.position;
            _ball.transform.position = (fullPath * _faction_of_path_traveled) + _trajectoryPoints[_lastWaypoint][0].transform.position;
            _sphere.transform.position = (fullPath * _faction_of_path_traveled) + _trajectoryPoints[_lastWaypoint][0].transform.position;
            _boolrender[_lastWaypoint] = true;
            _finalnextWaypoint = _lastWaypoint;
            _finallastWaypoint = _nextWaypoint;
        }
        else _sphere.transform.position = (fullPath * _faction_of_path_traveled) + _trajectoryPoints[_lastWaypoint][0].transform.position;

        if (_faction_of_path_traveled > 1){ //move to next waypoint      
            _lastWaypoint++; _nextWaypoint++;
            _faction_of_path_traveled = 0;
        }
    }

    ///  <summary>
    /// This fuction draw the trajectory prefab depending on the colisions with their raycast
    /// </summary>
    public void Setvisibility(){
        float dis = 0;
        int j=0;
        _sphere.SetActive(false) ;

        for (int i = 0; i < numOfTrajectoryPoints-1  && !_colisiondetected; i++){
            if (!_animshot) {
                _boolrender[i] = true;
                _ball.SetActive(true);
                //foreach (ParticleSystem subSystem in _trajectoryPoints[i]) {
                    //subSystem.simulationSpace = ParticleSystemSimulationSpace.Local;
                //}          
            }

            _fwd = _trajectoryPoints[i + 1][0].transform.position - _trajectoryPoints[i][0].transform.position;
            dis = _fwd.magnitude;

            if ((Physics.SphereCast(_trajectoryPoints[i][0].transform.position,_radio, _fwd, out _hitpoint, dis, mask))){               
                Vector3 hitting = _hitpoint.point;
                float displacement = _ball.transform.lossyScale.x * (_radio);
                _ball.transform.position = hitting + _hitpoint.normal * displacement;
                _colisiondetected = true;

                _finalWaypoint = i;
                _finalnextWaypoint = _finalWaypoint;
                _finallastWaypoint = _finalWaypoint+1;

                for (j = i+1 ; j < numOfTrajectoryPoints - 1; j++) {
                    _boolrender[j] = false;
                }
              _boolrender[numOfTrajectoryPoints - 1] = false;              
            }
            Debug.DrawRay(_trajectoryPoints[i][0].transform.position, _fwd, Color.green);
        }
        _colisiondetected = false;
    }

    /// <summary>
    /// Functions to draw the rainbow renderer
    /// </summary>
    public void Drawlinerenderer() {
        //Limpio vector auxiliar
        _aux.Clear();
        //limpio el line renderer 
        _linerenderer.SetVertexCount(0);

        //recorro todos los puntos y guardo las posiciones de los que estan anctivos porque implica que su raycast no ha colisionado
        for (int i = 0; i < numOfTrajectoryPoints; ++i) {
            if (_boolrender[i]) {
                _aux.Add(_trajectoryPoints[i][0].transform.position);
            }
            else {
				if ( i > 0) {
					Vector3 pointsDistance = _trajectoryPoints[i][0].transform.position - _trajectoryPoints[i - 1][0].transform.position;
					Vector3 clampedDistance = Vector3.ClampMagnitude(pointsDistance, _hitpoint.distance);
					_aux[i - 1] = _trajectoryPoints[i - 1][0].transform.position + clampedDistance;
                }
				break;
            }
        }
        
        // pongo en el line renderer la capacidad del vector auxiliar donde hemos guardado la posicion de ls puntos de la trayectoria
        _linerenderer.SetVertexCount(_aux.Count);

        //pongo en el line renderer la posicion de los puntos de la trayectoria
        for (int j = 0; j < _aux.Count; ++j){
            _linerenderer.SetPosition(j, _aux[j]);
        }      
    }

    /// <summary>
    /// Functions to draw the particle systems in the list of trajectories points
    /// </summary>
    public void ParticleRainbow(){
        float increment = 1f / (numOfTrajectoryPoints - 1);
        for (int i = 0; i < numOfTrajectoryPoints; i++) {
            float x = i * increment;
            _points[i].position = new Vector3(0f, 0f, x);
            _points[i].position = particleRainbow.transform.InverseTransformPoint(_trajectoryPoints[i][0].GetComponent<Transform>().position);

            foreach (ParticleSystem subSystem in _trajectoryPoints[i]) {
                if (_boolrender[i]) {
                    ParticleSystem.EmissionModule emission = subSystem.emission;
                    emission.enabled = true;
                    subSystem.GetComponent<Transform>().position = _trajectoryPoints[i][0].GetComponent<Transform>().position;
                }
                else {
                    ParticleSystem.EmissionModule emission = subSystem.emission;
                    emission.enabled = false;
                }
            }

            _points[i].startColor = new Color(x, 0f, 0f);
            _points[i].startSize = 0.1f;
        }
    }
    #endregion
}
