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

    CharacterControllerCustom ccc;

    private bool shootmode = false;
    
    //---------------------------------------	
    void Start ()
	{

        ccc = GetComponent<CharacterControllerCustom>();

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
            if (shootmode == false)
            {
                shootmode = true;
                ccc.Parameters = CharacterControllerParameters.ShootingParameters;

            }else if (shootmode == true)
            {
                shootmode = false;
            }

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //shoot mode of
            shootmode = false;
            Vector3 vo;
            vo.x = 0;
            vo.z = 5;
            vo.y = 0;
            transform.eulerAngles = new Vector3(0, 0, 0);
            setTrajectoryPoints(transform.position, vo);
            
            ccc.Parameters = null;
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
        if (shootmode)
		{
			//Vector3 vel = GetForceFrom(ball.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
			float angle = Mathf.Atan2(vel.y,vel.x)* Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0,0,angle);
           // setTrajectoryPoints(transform.position, vel / ball.GetComponent<Rigidbody>().mass);
			setTrajectoryPoints(transform.position, vel);
        }
	}
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
	private void createBall()
	{
		ball = (GameObject) Instantiate(BallPrefb);
		Vector3 pos = transform.position;
        pos.z = 1;
        float a=GetComponent<CharacterController>().radius;
		ball.transform.position = pos;
        ball.GetComponent<CharacterControllerCustom>().Parameters = CharacterControllerParameters.FlyingParameters;
        
		ball.SetActive(false);
	}
	//---------------------------------------	
	private void throwBall()
	{
		ball.SetActive(true);	
		//ball.GetComponent<Rigidbody>().useGravity = true;
        //ball.GetComponent<Rigidbody>().AddForce(GetForceFrom(ball.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)),ForceMode.Impulse);
       // ball.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);
        ball.GetComponent<CharacterControllerCustom>().AddForce(vel, ForceMode.VelocityChange);
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
			float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
			Vector3 pos = new Vector3(pStartPosition.x + dx , pStartPosition.y + dy ,2);
			trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100 * Time.deltaTime);
			trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
			trajectoryPoints[i].transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(pVelocity.y - (ccc.Parameters.Gravity.magnitude)*fTime,pVelocity.x)*Mathf.Rad2Deg);
			fTime += 0.1f;
		}
	}
}