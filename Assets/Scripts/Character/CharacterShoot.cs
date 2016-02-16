using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShoot : MonoBehaviour {
	
	public GameObject BallPrefb;
	
	private GameObject ball;
	private bool  isBallThrown;

    private bool shootmode = false;


    CharacterControllerCustom ccc;
    CharacterShootTrajectory st;
    GameControllerIndependentControl gc;

    //---------------------------------------	
    void Start (){

        ccc = GetComponent<CharacterControllerCustom>();
        st= GetComponent<CharacterShootTrajectory>();
        gc = GetComponent<GameControllerIndependentControl>();
      
	}
	//---------------------------------------	
	void Update (){
        if ((Input.GetKeyDown(KeyCode.X) && ccc.State.IsGrounded)) {
            if (shootmode == false){
                shootmode = true;
                st.enabled = true;
                
                ccc.Parameters = CharacterControllerParameters.ShootingParameters;
            }else if(shootmode== true){
                shootmode = false;
                st.QuitTrajectory();
                st.enabled = false;
                ccc.Parameters = null;
            }

        }
        
        if ((Input.GetKeyDown(KeyCode.Space)) && (shootmode==true)){
			
			if(!isBallThrown){
                createBall();
                throwBall();             
            }
		}
        
        
	}
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
	private void createBall(){

        //var ball = Instantiate(BallPrefb) as GameObject;

        ball = (GameObject) Instantiate(BallPrefb);

		Vector3 pos = transform.position;
        pos.z = 1;
        //float a=GetComponent<CharacterController>().radius;
		ball.transform.position = pos;
        ball.GetComponent<CharacterControllerCustom>().Parameters = CharacterControllerParameters.FlyingParameters;
        
		ball.SetActive(false);
	}
	//---------------------------------------	
	private void throwBall(){
		ball.SetActive(true);	

        ball.GetComponent<CharacterControllerCustom>().AddForce(st.GetComponent<CharacterShootTrajectory>().getvect(), ForceMode.VelocityChange);

        gc.AddDrop(ball);
    }
	
	
}