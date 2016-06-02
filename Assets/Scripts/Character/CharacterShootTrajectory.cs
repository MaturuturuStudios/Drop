using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class draws the shoot trajectory 
/// </summary>
public class CharacterShootTrajectory : MonoBehaviour
{
    #region Private Attributes

    /// <summary>
    /// Variable to draw the particle shoot mode 
    /// </summary>
    private bool explosion = false;

    /// <summary>
    /// Variable to control the rainbow animation 
    /// </summary>
    private float speedAnimation;

    private ParticleSystem.Particle[] points;
    private ParticleSystem rain;
    private float rainbowsize = 0.3f;
    private List<ParticleSystem> lluvia;
    private GameObject rainparticle;
    private bool finish = false;
    private bool animshot = true;
    private bool endscript = false;
    private float radio;
    private float oldspeed;
    private bool moving = false;
    private Vector3 oldpVelocity;
    private bool retrajectoring = false;
    private float oldsize;
    private LineRenderer linerenderer;
    private List<Vector3> aux ;
    private bool sizeanimation;
    private GameObject listtrajectory;
    private float oldrenderwidth,renderwidth;
    private float anglelook;
    /// <summary>
    /// This is to draw the animation of the particle that move throught the trajectory 
    /// </summary>
    private float journeyLength;
    private float faction_of_path_traveled;
    private float faction_of_traveled;
    private int lastWaypoint, nextWaypoint, finalWaypoint;

    private int finallastWaypoint, finalnextWaypoint;
    /// <summary>
    /// This is the arrays of the trajectory points
    /// </summary>
    private List<GameObject> trajectoryPoints;

    /// <summary>
    /// These are the scripts objects
    /// </summary>
    private CharacterControllerCustom ccc;

    /// <summary>
    /// Ray cast to know where the trajectory points are hitting
    /// </summary>
    private RaycastHit hit;

    /// <summary>
    /// Vector which contain the information that we need to shoot a drop in the sendflying method
    /// </summary>
    private Vector3 pVelocity;

    /// <summary>
    /// Size of the drop that will be shooted
    /// </summary>
    public float shootsize = 1;	// Changed by Nacho: Other objects may need to read this

    /// <summary>
    /// Vector auxiliar to keep data
    /// </summary>
    private Vector3 fwd;

    /// <summary>
    /// Boolean to know if the raycast hitted something
    /// </summary>
    private bool colisiondetected = false;

    /// <summary>
    /// Objects that represent the sphere that travel around the trajectory and the ball that indicate the size of the drop shooted
    /// </summary>
    private GameObject sphere, ball;

    /// <summary>
    /// Float that catch the Axis Inputs
    /// </summary>
    private float h, v;

    /// <summary>
    /// Float to indiccate if we changed the size of the drop shooted
    /// </summary>
    private bool selecting = false;

    /// <summary>
    /// Variable to keep data
    /// </summary>
    private float velocity = 1;

    /// <summary>
    /// Ray cast to know where the last trajectory point hit something
    /// </summary>
    private RaycastHit hitpoint;

    /// <summary>
    /// Angle of the trajectory that we will changing with the input axis
    /// </summary>
    private float angle;

    /// <summary>
    /// Speed that help to calculate the power that the drop will be shooted
    /// </summary>
    private float speed = 1.0F;

    #endregion

    #region Public Attributes

    

    public ParticleSystem particleRainbow;

    public new LineRenderer renderer;
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
    void Start()
    {      
        this.enabled = false;

        speed = 0;
        oldspeed = 0;
        radio = this.GetComponent<CharacterController>().radius;
      
        aux = new List<Vector3>();

        angle = 45;

        ccc = GetComponent<CharacterControllerCustom>();

        ParticleSystem.EmissionModule emision = particleRainbow.emission;
        emision.enabled = false;

        points = new ParticleSystem.Particle[numOfTrajectoryPoints];
        lluvia = new List<ParticleSystem>();

        rainparticle = new GameObject();
        rainparticle.name = "rain particle";
        //rainparticle.transform.parent = ccc.transform;
        lluvia.Clear();

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            ParticleSystem agua = Instantiate(particleRainbow);

            ParticleSystem.EmissionModule emission = agua.emission;
            emission.enabled = false;
            agua.GetComponent<Transform>().parent = ccc.transform;

            lluvia.Insert(i, agua);
            //lluvia[i].startSize = rainbowsize;

        }

        linerenderer = (LineRenderer) Instantiate(renderer);

        linerenderer.transform.parent = ccc.transform;
        listtrajectory = new GameObject();
        listtrajectory.name = " List Trajectory ";
        listtrajectory.transform.parent = ccc.transform;

        renderwidth = 1;
        

        trajectoryPoints = new List<GameObject>();

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            dot.GetComponent<Renderer>().enabled = false;
            //dot.tag = ("Trajectory"+i);
            dot.transform.parent = listtrajectory.transform;
            trajectoryPoints.Insert(i, dot);
        }

        lastWaypoint = 0;
        nextWaypoint = 1;
        finalWaypoint = trajectoryPoints.Capacity;
    }

    /// <summary>
	/// Method to know if we changed the size of the drop shooted
	/// </summary>
    public void selectingsize(float size)
    {
        oldspeed = Mathf.Sqrt((limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize)) * ccc.Parameters.Gravity.magnitude);
        oldsize = shootsize;
        shootsize = size;
        selecting = true;

        oldrenderwidth = renderwidth;

        if (oldsize > size)
        {
            renderwidth-=1;
        }else if( size > oldsize)
        {
            renderwidth+=1;
        }

    }

    /// <summary>
    /// Method to creat the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
    /// </summary>
    public void OnEnable()
    {
      
        explosion = false;

        renderwidth = 1;

        speedAnimation = speedrainbow*this.GetComponent<CharacterSize>().GetSize();

        finish = false;

        shootsize = 1;
        endscript = false;
        
        sphere = (GameObject)Instantiate(TrajectoryParticlePrefeb);
        //sphere.GetComponent<Transform>().parent = this.transform;
        sphere.SetActive(false);

        ball = (GameObject)Instantiate(TrajectorySizeIndicator);
        //ball.GetComponent<Transform>().parent = this.transform;
        ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
        ball.SetActive(false);
    
        nextWaypoint = 1;
        lastWaypoint = 0;
    
        animshot = true;
        sizeanimation = false;

        anglelook=this.GetComponentInChildren<CharacterModelController>().GetLookingDirection();


        if (anglelook > 0 && angle >90) {
            angle = angle-90;
            angle = 90 - angle;

        }
        if (anglelook < 0 && angle<90)
        {
            angle = 180 - ( angle);

        }

       
    }

    /// <summary>
	/// Method to destroy the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
	/// </summary>
    public void OnDisable()
    {
        animshot = false;
        renderwidth = 1;

        if (ball != null)
            ball.SetActive(false);

        if (sphere != null)
            sphere.SetActive(false);

        
        Destroy(sphere);
        Destroy(ball);
        //Destroy(rainparticle);

    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    void Update()
    {
        if (endscript)
        {
            animshot = true;
            QuitTrajectory();
            drawlinerenderer();
            
        }
        else {

            if (animshot == false)
            {
                if (!retrajectoring)
                {
                    if (selecting)
                    {                      
                        selecting = false;
                        moving = true;
                    }

                    h = Input.GetAxis(Axis.Horizontal);
                    v = Input.GetAxis(Axis.Vertical);


                    angle -= h;
                    

                    //Debug.Log(" axis horizontal " + h);

                    if (angle < 90)
                    {
                        angle += v;
                    }

                    if (angle > 90) angle -= v;

                    if (angle < 0) angle = 0;
                    if (angle > 180) angle = 180;

                   
                }
            }         

            //Calculate the vector from the drop  where  be shooted 
            Vector3 pos = this.transform.position;// + GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x + ball.GetComponent<SphereCollider>().radius* ball.transform.lossyScale.x);
                                                  //The power of the shoot
            speed = Mathf.Sqrt((limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize)) * ccc.Parameters.Gravity.magnitude);

            if (moving)
            {
                sizeanimation = true;

                retrajectoring = true;
                oldspeed = Mathf.MoveTowards(oldspeed, speed, 5 * Time.deltaTime); 
                oldsize= Mathf.MoveTowards(oldsize, shootsize, Time.deltaTime);

                ball.transform.localScale = new Vector3(oldsize, oldsize, oldsize);
                sphere.transform.localScale=new Vector3(oldsize, oldsize, oldsize);

                setTrajectoryPoints(pos, angle, oldspeed);

                oldrenderwidth = Mathf.MoveTowards(oldrenderwidth, renderwidth, Time.deltaTime);               

                linerenderer.SetWidth(oldrenderwidth, oldrenderwidth);

                if (oldspeed == speed)
                {
                    moving = false;
                    sizeanimation = false;

                }
            }
            else
            {
                //animshot = false;
                setTrajectoryPoints(pos, angle, speed);
                retrajectoring = false;
            }
            
            setvisibility();
            canshooot();
            
            drawlinerenderer();
            
        }
        ParticleTrip();
        ParticleRainbow();
    }


    /// <summary>
    /// This fuctions calculate if there is a colision with the raycast of the first shoot trajectory  before it
    /// </summary>
    public bool canshooot()
    {
        float dis = 0;
        Vector3 spheredis = transform.position;//+ GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x);
        fwd = trajectoryPoints[0].transform.position - spheredis;

        dis = fwd.magnitude;

        Debug.DrawRay(spheredis, fwd, Color.green);

       

        if (Physics.SphereCast(spheredis,radio, fwd,out hitpoint, dis, mask))
        {
            ball.SetActive(false);
            sphere.SetActive( false);
            for (int j = 0; j < numOfTrajectoryPoints - 1; j++)
            {
                trajectoryPoints[j].GetComponent<Renderer>().enabled = false;

            }
            return false;

        }
        else
        {
            sphere.SetActive(true);
           

            return true;
        }
    }

    public bool sizeAnimation()
    {
        return sizeanimation;

    }

   public void endingd()
    {
        
        endscript = true;
    }


    public bool isending()
    {
        return finish;
    }

    public bool animation()
    {
        return animshot;
    }

    public float Angle()
    {
        return angle;

    }

    public void finishing()
    {      

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
             trajectoryPoints[i].GetComponent<Renderer>().enabled = false;

            ParticleSystem.EmissionModule emission = lluvia[i].emission;
            emission.enabled = false;

        }
        linerenderer.SetVertexCount(0);
         linerenderer.SetWidth( 1,1);
        sphere.SetActive( false);
        ball.SetActive(false);
        //Destroy(rainparticle);

    }

    /// <summary>
    /// This fuctions delete the trajectory
    /// </summary>
    public void QuitTrajectory()
    {

        

        if (finalnextWaypoint ==0)
        {
           
            trajectoryPoints[finalnextWaypoint].GetComponent<Renderer>().enabled = false;
            animshot = false;
            renderwidth = 1;
            linerenderer.SetWidth(1, 1);
            this.GetComponent<CharacterShoot>().Endshootmode();
            ccc.Parameters = null;


            this.enabled = false;
        }

        Vector3 fullPath = trajectoryPoints[finalnextWaypoint].transform.position - trajectoryPoints[finallastWaypoint].transform.position; //defines the path between lastWaypoint and nextWaypoint as a Vector3
        faction_of_traveled += speedAnimation * Time.deltaTime; //animate along the path
        

        if (faction_of_traveled > 1) //move to next waypoint
        {
            finallastWaypoint--;
            finalnextWaypoint--;

            faction_of_traveled = 0;

            
        }
            //ball.transform.position = (fullPath * 2) + trajectoryPoints[lastWaypoint].transform.position;
        ball.transform.position = (fullPath * faction_of_traveled) + trajectoryPoints[finallastWaypoint].transform.position;
        trajectoryPoints[finallastWaypoint].GetComponent<Renderer>().enabled = false;

        if (ball.transform.position.x <= sphere.transform.position.x && angle <90)
        {
            sphere.SetActive(false);
           
            
        }
        else if (ball.transform.position.x >= sphere.transform.position.x && angle>90)
        {
            sphere.SetActive(false);
           

        }

        
        
       
    }

    ///  <summary>
    /// This fuctions return the shoot vector for the shoot script
    /// </summary>
    public Vector3 GetpVelocity()
    {
        return pVelocity; 
    }

    ///  <summary>
    /// This fuctions calculate the points of the trajectory and the shoot vector which is pVelocity
    /// </summary>
    void setTrajectoryPoints(Vector3 pStartPosition, float angle, float speed)
    {
        //calculate the end vector of the trajectory
        pVelocity = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * speed, Mathf.Sin(angle * Mathf.Deg2Rad) * speed, 0);
        //magnitud of the pVelocity to calculate de distance que es igual al valor de speed
        velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));

       float fTime = 0;

        fTime += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
             trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100);
           // trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (ccc.Parameters.Gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }

    }

    ///  <summary>
    /// This fuctions draw the particle trip along the trajectory
    /// </summary>
    public void ParticleTrip()
    {
        
        if (nextWaypoint > finalWaypoint)
            {
                nextWaypoint = 1;
                lastWaypoint = 0;
                animshot = false;
            }

            Vector3 fullPath = trajectoryPoints[nextWaypoint].transform.position - trajectoryPoints[lastWaypoint].transform.position; //defines the path between lastWaypoint and nextWaypoint as a Vector3
            if(animshot && !endscript) faction_of_path_traveled += speedAnimation * Time.deltaTime; //animate along the path
            else faction_of_path_traveled += particletrajectoryspeed * Time.deltaTime;
           
        if (animshot && !endscript && canshooot())
        {
            
            ball.SetActive(true);
            //ball.transform.position = (fullPath * 2) + trajectoryPoints[lastWaypoint].transform.position;
            ball.transform.position = (fullPath * faction_of_path_traveled) + trajectoryPoints[lastWaypoint].transform.position;
            sphere.transform.position = (fullPath * faction_of_path_traveled) + trajectoryPoints[lastWaypoint].transform.position;
            trajectoryPoints[lastWaypoint].GetComponent<Renderer>().enabled = true;
            finalnextWaypoint = lastWaypoint;
            finallastWaypoint = nextWaypoint;


        }
        else sphere.transform.position = (fullPath * faction_of_path_traveled) + trajectoryPoints[lastWaypoint].transform.position;

        if (faction_of_path_traveled > 1) //move to next waypoint
        {
            lastWaypoint++; nextWaypoint++;
            faction_of_path_traveled = 0;
        }
    }

    ///  <summary>
    /// This fuction draw the trajectory prefab depending on the colisions with their raycast
    /// </summary>
    public void setvisibility()
    {
        float dis = 0;
        int j=0;
        sphere.SetActive(false) ;

        for (int i = 0; i < numOfTrajectoryPoints-1  && !colisiondetected; i++)
        {

            if (!animshot)
            {
                trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
                ball.SetActive(true);
            }

            fwd = trajectoryPoints[i + 1].transform.position - trajectoryPoints[i].transform.position;

            dis = fwd.magnitude;

            if ((Physics.SphereCast(trajectoryPoints[i].transform.position,radio, fwd, out hitpoint, dis, mask)))
            {
                

                Vector3 hitting = hitpoint.point;
                float displacement = ball.transform.lossyScale.x * (radio);
                ball.transform.position = hitting + hitpoint.normal * displacement;
                colisiondetected = true;

                finalWaypoint = i;
                finalnextWaypoint = finalWaypoint;
                finallastWaypoint = finalWaypoint+1;

                
                    

                for (j = i+1 ; j < numOfTrajectoryPoints - 1; j++)
                {
                    trajectoryPoints[j].GetComponent<Renderer>().enabled = false;

                }
                trajectoryPoints[numOfTrajectoryPoints - 1].GetComponent<Renderer>().enabled = false;
                
            }

            Debug.DrawRay(trajectoryPoints[i].transform.position, fwd, Color.green);

        }
        colisiondetected = false;

    }

    public void drawlinerenderer()
    {
        //Limpio vector auxiliar
        aux.Clear();
        //limpio el line renderer 
        linerenderer.SetVertexCount(0);

        //recorro todos los puntos y guardo las posiciones de los que estan anctivos porque implica que su raycast no ha colisionado
        for (int i = 0; i < numOfTrajectoryPoints; ++i) {
            if (trajectoryPoints[i].GetComponent<Renderer>().enabled == true)
            {
                aux.Insert(i, trajectoryPoints[i].transform.position);
            }
            if (trajectoryPoints[i].GetComponent<Renderer>().enabled == false)
            {
                aux.Insert(i, trajectoryPoints[i].transform.position);
                i = numOfTrajectoryPoints;
            }
        }
        
        // pongo en el line renderer la capacidad del vector auxiliar donde hemos guardado la posicion de ls puntos de la trayectoria
        linerenderer.SetVertexCount(aux.Count);

        //pongo en el line renderer la posicion de los puntos de la trayectoria
        for (int j = 0; j < aux.Count; ++j)
        {
            linerenderer.SetPosition(j, aux[j]);
        }

       
    }

    public void ParticleRainbow()
    {
        float increment = 1f / (numOfTrajectoryPoints - 1);
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float x = i * increment;
            points[i].position = new Vector3(0f, 0f, x);

            points[i].position = particleRainbow.transform.InverseTransformPoint(trajectoryPoints[i].GetComponent<Transform>().position);

            if (trajectoryPoints[i].GetComponent<Renderer>().enabled == true)
            {
                ParticleSystem.EmissionModule emission = lluvia[i].emission;
                emission.enabled = true;
                lluvia[i].GetComponent<Transform>().position = trajectoryPoints[i].GetComponent<Transform>().position;
            }
            else if (trajectoryPoints[i].GetComponent<Renderer>().enabled == false)
            {
                ParticleSystem.EmissionModule emission = lluvia[i].emission;
                emission.enabled = false;
            }

                points[i].startColor = new Color(x, 0f, 0f);
            points[i].startSize = 0.1f;
        }

    }
    #endregion
}
