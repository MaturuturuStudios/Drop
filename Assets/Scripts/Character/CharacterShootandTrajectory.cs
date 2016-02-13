using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShootandTrajectory : MonoBehaviour 
{
	public GameObject TrajectoryPointPrefeb;
	public GameObject BallPrefb;
	
	private GameObject ball;
	private bool isPressed, isBallThrown;
	private float power = 25;
	public int numOfTrajectoryPoints = 30;
	private List<GameObject> trajectoryPoints;

    private Vector3 vel;

    private Vector3 aux;

    public GameObject gamecontroler;
    //---------------------------------------	
    void Start ()
	{
        vel.x = vel.y=3;
        vel.z= 0f;

        createBall();

        trajectoryPoints = new List<GameObject>();
		isPressed = isBallThrown =false;
		for(int i=0;i<numOfTrajectoryPoints;i++)
		{
			GameObject dot= (GameObject) Instantiate(TrajectoryPointPrefeb);
			dot.GetComponent<Renderer>().enabled = false;
			trajectoryPoints.Insert(i,dot);
		}
	}
	//---------------------------------------	
	void Update () 
	{
		
		
		isPressed = true;
       


        if (Input.GetMouseButtonUp(0))
		{
			isPressed = false;
			if(!isBallThrown)
			{
                
                throwBall();
                createBall();
            }
		}
        if (Input.GetKeyDown(KeyCode.X))
        {
            //shoot mode on
            //GetComponent<CharacterControllerCustom>().Parameters.movementBehaviour =1;

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //shoot mode of
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
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
        }
        if (isPressed)
		{
			//Vector3 vel = GetForceFrom(ball.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
			float angle = Mathf.Atan2(vel.y,vel.x)* Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0,0,angle);
			setTrajectoryPoints(transform.position, vel/ball.GetComponent<CharacterControllerCustom>().Parameters.mass);
        }
	}
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
	private void createBall()
	{
		ball = (GameObject) Instantiate(BallPrefb);
		Vector3 pos = transform.position;
		pos.z=1;
		ball.transform.position = pos;
		ball.SetActive(false);
	}
	//---------------------------------------	
	private void throwBall()
	{
		ball.SetActive(true);	
		//ball.GetComponent<Rigidbody>().useGravity = true;
        //ball.GetComponent<Rigidbody>().AddForce(GetForceFrom(ball.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)),ForceMode.Impulse);
        //ball.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);
        ball.GetComponent<CharacterControllerCustom>().AddForce(vel);
        //isBallThrown = true;
    }
	//---------------------------------------	
	private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
	{
		return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y))*power;//*ball.rigidbody.mass;
	}
	//---------------------------------------	
	// It displays projectile trajectory path
	//---------------------------------------	
	void setTrajectoryPoints(Vector3 pStartPosition , Vector3 pVelocity )
	{
		float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
		float angle = Mathf.Rad2Deg*(Mathf.Atan2(pVelocity.y , pVelocity.x));
		float fTime = 0;
		
		fTime += 0.1f;
		for (int i = 0 ; i < numOfTrajectoryPoints ; i++)
		{
			float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
			Vector3 pos = new Vector3(pStartPosition.x + dx , pStartPosition.y + dy ,2);
			trajectoryPoints[i].transform.position = pos;
			trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
			trajectoryPoints[i].transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude)*fTime,pVelocity.x)*Mathf.Rad2Deg);
			fTime += 0.1f;
		}
	}
}