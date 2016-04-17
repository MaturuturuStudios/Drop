using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanonShoot : MonoBehaviour
{

    public GameObject TrajectoryPointPrefeb;
   
    //public GameObject TrajectoryParticlePrefeb;
    public int numOfTrajectoryPoints = 30;
    //public float particletrajectoryspeed = 0.08f;
    public float angle=45;
    //public float speed = 1.0F;

    private float journeyLength;
    private float faction_of_path_traveled;


    private List<GameObject> trajectoryPoints;
    private List<GameObject> bolas;
    private CharacterControllerCustom cc;

    CharacterControllerCustom ccc;
   

    private Vector3 vel, pVelocity;

    public float power = 25;
    private bool ontriger=false;

    private RaycastHit hit;
    private Vector3 fwd;
    private bool colisiondetected = false;

   
    private float oldvelocity;
    private float h, v;
    //private float shootlimiet = 1;

    private float velocity = 1;

    // Use this for initialization
    void Awake()
    {

       // c = GetComponent<CharacterController>();
        //ccc = GetComponent<CharacterControllerCustom>();

        cc = GameObject.FindGameObjectWithTag(Tags.Player)
                                .GetComponent<CharacterControllerCustom>();

        trajectoryPoints = new List<GameObject>();

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            dot.GetComponent<Renderer>().enabled = false;
            //dot.tag = ("Trajectory"+i);
            dot.transform.parent = transform;
            trajectoryPoints.Insert(i, dot);
        }

    }

    //This fuctions create prefabs that we are going to use and initialite some variables
    
    // Update is called once per frame
    void Update()
    {
        
        transform.eulerAngles = new Vector3(0, 0, angle ); //this is to face in the direction you are aming
        Vector3 pos = this.transform.position ;                                             // float speed = Mathf.Sqrt((power) * ccc.Parameters.Gravity.magnitude);
        float speed = Mathf.Sqrt((power) * cc.Parameters.Gravity.magnitude);
        setTrajectoryPoints(pos, angle, speed);
        setvisibility();

        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown(Axis.Action))
        {
            if (ontriger)
            {
                ontriger = false;
                ccc.transform.position = this.transform.position;
                ccc.Stop();
                ccc.SendFlying(GetpVelocity());
                Debug.Log(" angle " + transform.eulerAngles);

                
            }
        }

    }

    public void Shoot()
    {
        if (ontriger)
        {
            ontriger = false;
            ccc.transform.position = this.transform.position;
            ccc.Stop();
            ccc.SendFlying(GetpVelocity());
            Debug.Log(" angle " + transform.eulerAngles);


        }
    }

    public void Changeangle()
    {

        angle = 75;
        power = 54;


    }

    void OnTriggerEnter(Collider other)
    {
        ccc = other.GetComponent<CharacterControllerCustom>();
        if( (ccc != null))
        {                     
            ontriger = true;
        }

        }

    void OnTriggerExit(Collider other)
    {
        ccc = other.GetComponent<CharacterControllerCustom>();
        if ((ccc != null))
        {
            ontriger = false;

        }

    }


    //This fuctions delete the trajectory
    public void QuitTrajectory()
    {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
        }
        
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
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (cc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
            trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100);
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (cc.Parameters.Gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }

    }

    //This fuctions draw the particle trip along the trajectory
   /* public void Example()
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
        
    }*/

    //This fuction draw the trajectory prefab depending on the colisions with their raycast
    public void setvisibility()
    {
        float dis = 0;
        int j=0;

        for (int i = 0; i < numOfTrajectoryPoints - 1 && !colisiondetected; i++)
        {

            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            fwd = trajectoryPoints[i + 1].transform.position - trajectoryPoints[i].transform.position;

            dis = fwd.magnitude;

            if ((Physics.Raycast(trajectoryPoints[i].transform.position, fwd, dis)))
            {
                colisiondetected = true;


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
