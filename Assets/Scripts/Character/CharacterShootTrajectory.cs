using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class draws the shoot trajectory 
/// </summary>
public class CharacterShootTrajectory : MonoBehaviour
{
    #region Private Attributes

    private bool animshot = true;
    private bool endscript = false;
    private float radio;
  
    /// <summary>
    /// This is to draw the animation of the particle that move throught the trajectory 
    /// </summary>
    private float journeyLength;
    private float faction_of_path_traveled;
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
    private float shootsize = 1;

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
    public LayerMask Scene=8;
    public LayerMask Character=9;

    public float speedAnimation = 30;
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
    void Awake()
    {      
        this.enabled = false;

        radio = this.GetComponent<CharacterController>().radius;

        angle = 45;

        ccc = GetComponent<CharacterControllerCustom>();
        

        trajectoryPoints = new List<GameObject>();

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            dot.GetComponent<Renderer>().enabled = false;
            //dot.tag = ("Trajectory"+i);
            dot.transform.parent = ccc.transform;
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
        shootsize = size;
        selecting = true;

    }

    /// <summary>
    /// Method to creat the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
    /// </summary>
    public void OnEnable()
    {
        shootsize = 1;
        endscript = false;
        
        sphere = (GameObject)Instantiate(TrajectoryParticlePrefeb);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<Renderer>().enabled = false;

        ball = (GameObject)Instantiate(TrajectorySizeIndicator);
        ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
        ball.SetActive(false);
    
        nextWaypoint = 1;
        lastWaypoint = 0;
    
        animshot = true;

    }

    /// <summary>
	/// Method to destroy the sphere that travel in the trajectory path and the ball the indicate the size of the shooted drop.
	/// </summary>
    public void OnDisable()
    {
        if (ball != null)
            ball.SetActive(false);

        Destroy(sphere);
        Destroy(ball);      

    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    void Update()
    {
        if (endscript)
        {
            QuitTrajectory();
        }else{
            if (animshot == false)
            {
                if (selecting)
                {
                    ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
                    selecting = false;
                }

                h = Input.GetAxis(Axis.Horizontal);
                v = Input.GetAxis(Axis.Vertical);


                angle -= h;
                angle += v;

            }

            //Calculate the vector from the drop  where  be shooted 
            Vector3 pos = this.transform.position;// + GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x + ball.GetComponent<SphereCollider>().radius* ball.transform.lossyScale.x);
                                                  //The power of the shoot
            speed = Mathf.Sqrt((limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize)) * ccc.Parameters.Gravity.magnitude);

            setTrajectoryPoints(pos, angle, speed);
            setvisibility();
            canshooot();
            ParticleTrip();


        }
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

        if (Physics.Raycast(spheredis, fwd, dis, Scene))
        {
            ball.SetActive(false);
            sphere.GetComponent<Renderer>().enabled = false;
            for (int j = 1; j < numOfTrajectoryPoints - 1; j++)
            {
                trajectoryPoints[j].GetComponent<Renderer>().enabled = false;

            }
            return false;

        }
        else
        {
            sphere.GetComponent<Renderer>().enabled = true;
           

            return true;
        }
    }
   public void endingd()
    {
        
        endscript = true;
    }

    public bool animation()
    {
        return animshot;
    }

    public void finishing()
    {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
             trajectoryPoints[i].GetComponent<Renderer>().enabled = false;

        }
        sphere.GetComponent<Renderer>().enabled = false;
        ball.SetActive(false);
    }

    /// <summary>
    /// This fuctions delete the trajectory
    /// </summary>
    public void QuitTrajectory()
    {

        if (finalnextWaypoint ==0)
        {
            trajectoryPoints[finalnextWaypoint].GetComponent<Renderer>().enabled = false;
           
            this.enabled = false;
        }

        Vector3 fullPath = trajectoryPoints[finalnextWaypoint].transform.position - trajectoryPoints[finallastWaypoint].transform.position; //defines the path between lastWaypoint and nextWaypoint as a Vector3
        faction_of_path_traveled += speedAnimation * Time.deltaTime; //animate along the path
        

        if (faction_of_path_traveled > 1) //move to next waypoint
        {
            finallastWaypoint--;
            finalnextWaypoint--;

            faction_of_path_traveled = 0;

            
        }
            //ball.transform.position = (fullPath * 2) + trajectoryPoints[lastWaypoint].transform.position;
        ball.transform.position = (fullPath * faction_of_path_traveled) + trajectoryPoints[finallastWaypoint].transform.position;
        trajectoryPoints[finallastWaypoint].GetComponent<Renderer>().enabled = false;
        
        sphere.GetComponent<Renderer>().enabled = false;
      
        
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
            if(animshot) faction_of_path_traveled += speedAnimation * Time.deltaTime; //animate along the path
            else faction_of_path_traveled += particletrajectoryspeed * Time.deltaTime;
           
        if (animshot)
        {
            ball.SetActive(true);
            //ball.transform.position = (fullPath * 2) + trajectoryPoints[lastWaypoint].transform.position;
            ball.transform.position = (fullPath * faction_of_path_traveled) + trajectoryPoints[lastWaypoint].transform.position;
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
        sphere.GetComponent<Renderer>().enabled = false;

        for (int i = 0; i < numOfTrajectoryPoints - 1 && !colisiondetected; i++)
        {

            if (!animshot)
                trajectoryPoints[i].GetComponent<Renderer>().enabled = true;

            fwd = trajectoryPoints[i + 1].transform.position - trajectoryPoints[i].transform.position;

            dis = fwd.magnitude;

            if ((Physics.Raycast(trajectoryPoints[i].transform.position, fwd, out hitpoint, dis, Character)) || (Physics.Raycast(trajectoryPoints[i].transform.position, fwd, out hitpoint, dis,Scene)))
            {
                ball.SetActive(true);

                Vector3 hitting = hitpoint.point;
                float displacement = ball.transform.lossyScale.x * (radio*shootsize);
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


    #endregion
}
