using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShootTrajectory : MonoBehaviour {

    public GameObject TrajectoryPointPrefeb;
    public int numOfTrajectoryPoints = 30;
    private List<GameObject> trajectoryPoints;

    

    private CharacterControllerCustom ccc;
    private  CharacterShoot s;

    private Vector3 vel;

    

    private float power = 25;

    // Use this for initialization
    void Start () {
        this.enabled = false;

        vel.x = 5;
        vel.y = 5;

        ccc = GetComponent<CharacterControllerCustom>();
        s= GetComponent<CharacterShoot>();

        trajectoryPoints = new List<GameObject>();
        
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            dot.GetComponent<Renderer>().enabled = false;
            trajectoryPoints.Insert(i, dot);
        }
    }

	// Update is called once per frame
	void Update () {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        vel.x += h;
        vel.y += v;

        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            vel.y += 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            vel.y -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            vel.x += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            vel.x -= 1;
        }*/

        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        setTrajectoryPoints(transform.position, vel);
    }
    public void QuitTrajectory() {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
        }
    }

    public Vector3 getvect() {
        return vel;
    }
    //---------------------------------------	
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos) {
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;//*ball.rigidbody.mass;
    }
    //---------------------------------------	
    // It displays projectile trajectory path
    //---------------------------------------	
    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity) {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        fTime += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints; i++) {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (ccc.Parameters.gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100 );
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (ccc.Parameters.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }
    }
}
