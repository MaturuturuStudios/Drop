﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
public class CanonShoot : MonoBehaviour
{
    #region Private Attributes

    /// <summary>
    /// This is to draw the animation of the particle that move throught the trajectory 
    /// </summary>
    private float journeyLength;
    private float faction_of_path_traveled;

    /// <summary>
    /// This is the arrays of the trajectory points
    /// </summary>
    private List<GameObject> trajectoryPoints;

    /// <summary>
    /// These are the scripts objects
    /// </summary>
    private CharacterControllerCustom cc;
    private CharacterControllerCustom ccc;

    /// <summary>
    /// Vector which contain the information that we need to shoot a drop in the sendflying method
    /// </summary>
    private Vector3  pVelocity;

    /// <summary>
    /// To know if we tauch the trigger
    /// </summary>
    private bool ontriger = false;

    /// <summary>
    /// Ray cast to know where the trajectory points are hitting
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// Vector auxiliar to keep data
    /// </summary>
    private Vector3 fwd;

    /// <summary>
    /// Variable to keep data
    /// </summary>
    private float velocity = 1;

    /// <summary>
    /// Boolean to know if the raycast hitted something
    /// </summary>
    private bool colisiondetected = false;

    #endregion

    #region Public Attributes

    /// <summary>
    /// Prefab of the trajectory points
    /// </summary>
    public GameObject TrajectoryPointPrefeb;

    /// <summary>
    /// Number of trajectoyr points we will have
    /// </summary>
    public int numOfTrajectoryPoints = 100;

    /// <summary>
    /// Angle of the trajectory that we will changing with the input axis
    /// </summary>
    public float angle=45;

    /// <summary>
    ///  the power that the drop will be shooted
    /// </summary>
    public float power = 25;

    #endregion

    #region Methods

    /// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    void Awake()
    {
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

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    void Update()
    {
        
        transform.eulerAngles = new Vector3(0, 0, angle ); //this is to face in the direction you are aming
        Vector3 pos = this.transform.position ;            // float speed = Mathf.Sqrt((power) * ccc.Parameters.Gravity.magnitude);
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
               // Debug.Log(" angle " + transform.eulerAngles);              
            }
        }

    }

    /// <summary>
    /// Method to use in Game Input Script
    /// </summary>
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

    /// <summary>
    /// Method to change the angle and the power of the plant when drop touch the trigger
    /// </summary>
    public void Changeangle()
    {
        angle = 75;
        power = 54;

    }

    /// <summary>
    /// Method that activate when the drop touch the trigger
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        ccc = other.GetComponent<CharacterControllerCustom>();
        if( (ccc != null))
        {                     
            ontriger = true;
        }

    }

    /// <summary>
    /// Method that activate when the drop exit the trigger
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        ccc = other.GetComponent<CharacterControllerCustom>();
        if ((ccc != null))
        {
            ontriger = false;

        }

    }

    /// <summary>
    /// This fuctions delete the trajectory
    /// </summary>
    public void QuitTrajectory()
    {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
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

    ///  <summary>
    /// This fuctions draw the particle trip along the trajectory
    /// </summary>
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

    ///  <summary>
    /// This fuction draw the trajectory prefab depending on the colisions with their raycast
    /// </summary>
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

    #endregion
}
