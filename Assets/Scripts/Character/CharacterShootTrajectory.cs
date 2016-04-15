using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShootTrajectory : MonoBehaviour
{

    public GameObject TrajectoryPointPrefeb;
    public GameObject TrajectorySizeIndicator;
    public GameObject TrajectoryParticlePrefeb;
    public int numOfTrajectoryPoints = 30;
    public float particletrajectoryspeed = 0.08f;

    public float speed = 1.0F;

    private float journeyLength;
    private float faction_of_path_traveled;
    private int lastWaypoint, nextWaypoint, finalWaypoint;

    public LayerMask Scene=8;
    public LayerMask Character=9;

    private List<GameObject> trajectoryPoints;
    private List<GameObject> bolas;
    private CharacterControllerCustom ccc;
    private CharacterController c;


    private Vector3 vel, pVelocity;
    private float shootsize = 1;
    public float limitshoot = 5;


    private RaycastHit hit;
    private Vector3 fwd;
    private bool colisiondetected = false;
    private GameObject sphere, ball;
   
    private float oldvelocity;
    private float h, v;
    //private float shootlimiet = 1;
    private bool selecting = false;
    private float velocity = 1;

    private RaycastHit hitpoint;
    private float angle;
    // Use this for initialization
    void Awake()
    {
       
        this.enabled = false;

        

        angle = 45;

        c = GetComponent<CharacterController>();
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

    public void selectingsize(float size)
    {
        shootsize = size;
        selecting = true;

    }
    //This fuctions create prefabs that we are going to use and initialite some variables
    public void OnEnable()
    {
        shootsize = 1;
        sphere = (GameObject)Instantiate(TrajectoryParticlePrefeb);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<Renderer>().enabled = false;

        ball = (GameObject)Instantiate(TrajectorySizeIndicator);
        ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
        ball.GetComponent<Collider>().enabled = false;


        //shootlimiet = limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize);
       // Debug.Log(" limit shoot " + limitshoot + " character ssize  " + ccc.GetComponent<CharacterSize>().GetSize() + " diasparada " + shootsize);

        ball.GetComponent<Renderer>().enabled = true;

       
        nextWaypoint = 1;
        lastWaypoint = 0;

    }
    //This fuctions delete prefabs that we ar not using
    public void OnDisable()
    {

        if (ball != null)
            ball.GetComponent<Renderer>().enabled = false;
        //stopcourutine = true;
        //StopCoroutine(Example());

        Destroy(sphere);
        Destroy(ball);

    }
    // Update is called once per frame
    void Update()
    {

        if (selecting)
        {
            ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
            //shootlimiet = limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize);

            selecting = false;
        }

        h = Input.GetAxis(Axis.Horizontal);
        //v = Input.GetAxis(Axis.Vertical);

        angle += h;
       // Debug.Log(" angulo " + angle);
        // transform.eulerAngles = new Vector3(0, 0, angle);    this is to face in the direction you are aming

        Vector3 pos = this.transform.position + GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x + sphere.GetComponent<SphereCollider>().radius* sphere.transform.lossyScale.x);
        float speed = Mathf.Sqrt((limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize)) * ccc.Parameters.Gravity.magnitude);
        // float speed = shootlimiet;

        setTrajectoryPoints(pos, angle, speed);
        setvisibility();
        canshooot();
        Example();

    }

    //This fuctions calculate if there is a colision with the raycast of the first shoot trajectory  before it
    public bool canshooot()
    {
        float dis = 0;
        Vector3 spheredis = transform.position + GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x);
        fwd = trajectoryPoints[0].transform.position - spheredis;

        dis = fwd.magnitude;

        Debug.DrawRay(spheredis, fwd, Color.green);

        if (Physics.Raycast(spheredis, fwd, dis))
        {
            ball.GetComponent<Renderer>().enabled = false;
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

    //This fuctions delete the trajectory
    public void QuitTrajectory()
    {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
        }
        sphere.GetComponent<Renderer>().enabled = false;
        
        //stopcourutine = true;
        //StopCoroutine(Example());
    }

    //This fuctions return the shoot vector for the shoot script
    public Vector3 GetpVelocity()
    {
        return pVelocity; 
    }

    //This fuctions calculate the points of the trajectory and the shoot vector which is pVelocity
    void setTrajectoryPoints(Vector3 pStartPosition, float angle, float speed)
    {

        pVelocity = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * speed, Mathf.Sin(angle * Mathf.Deg2Rad) * speed, 0);
       // Debug.Log(" APUNTANDO " + pVelocity);

        velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));

        float fTime = 0;

        fTime += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
            trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100);
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (ccc.Parameters.Gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }

    }

    //This fuctions draw the particle trip along the trajectory
    public void Example()
    {
         //Debug.Log("FINAL " + finalWaypoint);
            if (nextWaypoint > finalWaypoint)
            {
                nextWaypoint = 1;
                lastWaypoint = 0;
            }

            Vector3 fullPath = trajectoryPoints[nextWaypoint].transform.position - trajectoryPoints[lastWaypoint].transform.position; //defines the path between lastWaypoint and nextWaypoint as a Vector3
            faction_of_path_traveled += particletrajectoryspeed * Time.deltaTime; //animate along the path
            if (faction_of_path_traveled > 1) //move to next waypoint
            {
                lastWaypoint++; nextWaypoint++;

                faction_of_path_traveled = 0;

                return;
            }

            sphere.transform.position = (fullPath * faction_of_path_traveled) + trajectoryPoints[lastWaypoint].transform.position;
        
    }



    //This fuction draw the trajectory prefab depending on the colisions with their raycast
    public void setvisibility()
    {
        float dis = 0;
        int j=0;
        sphere.GetComponent<Renderer>().enabled = true;

        for (int i = 0; i < numOfTrajectoryPoints - 1 && !colisiondetected; i++)
        {

            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            fwd = trajectoryPoints[i + 1].transform.position - trajectoryPoints[i].transform.position;

            dis = fwd.magnitude;

            if ((Physics.Raycast(trajectoryPoints[i].transform.position, fwd, out hitpoint, dis, Character)) || (Physics.Raycast(trajectoryPoints[i].transform.position, fwd, out hitpoint, dis,Scene)))
            {
                ball.GetComponent<Renderer>().enabled = true;
                Vector3 hitting = hitpoint.point;
                float displacement = ball.transform.lossyScale.x * ball.GetComponent<SphereCollider>().radius;
                ball.transform.position = hitting + hitpoint.normal * displacement;
                colisiondetected = true;

                finalWaypoint = i;

                for (j = i + 1; j < numOfTrajectoryPoints - 1; j++)
                {
                    trajectoryPoints[j].GetComponent<Renderer>().enabled = false;

                }
                trajectoryPoints[numOfTrajectoryPoints - 1].GetComponent<Renderer>().enabled = false;
                
            }

            Debug.DrawRay(trajectoryPoints[i].transform.position, fwd, Color.green);

        }
        colisiondetected = false;

    }

}
