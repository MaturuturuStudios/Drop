﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShoot : MonoBehaviour {

    public GameObject BallPrefb;

   
    private GameObject ball;
	private bool  isBallThrown;
    private int lessize=0;
    private int falling = 0;
    private bool shootmode = false;
    

    CharacterControllerCustom ccc;
    CharacterShootTrajectory st;
    GameControllerIndependentControl _gcic;

    //---------------------------------------	
    void Start (){

        ccc = GetComponent<CharacterControllerCustom>();
        st= GetComponent<CharacterShootTrajectory>();
        //gc = GetComponent<GameControllerIndependentControl>(); //Comentado por toni

        _gcic = GameObject.FindGameObjectWithTag("GameController")
                                .GetComponent<GameControllerIndependentControl>();
	}
    public bool isShooting(){
        return shootmode;
    }
	//---------------------------------------	
	void Update (){
        if ((Input.GetKeyDown(KeyCode.X) && ccc.State.IsGrounded == true && (GetComponent<CharacterSize>().GetSize()>1))) {
            if (shootmode == false){
                shootmode = true;
                st.enabled = true;
                
                ccc.Parameters = CharacterControllerParameters.ShootingParameters;
            }else if((shootmode== true))
            {
                shootmode = false;
                st.QuitTrajectory();
                st.enabled = false;
                ccc.Parameters = null;
            }       
        }
        if ((lessize == 1) || ccc.State.IsGrounded == false){
            shootmode = false;
            st.QuitTrajectory();
            st.enabled = false;
            
             //ccc.Parameters = null;                      
        }
        
       
        if ((Input.GetKeyDown(KeyCode.Space)) && (shootmode==true)){
			
			if(!isBallThrown){
                throwBall();
                lessize = GetComponent<CharacterSize>().GetSize();
                lessize -= 1;
                GetComponent<CharacterSize>().SetSize(lessize);

                //_gcic.SetControl(1);
                _gcic.ControlNextDrop();
            }
		}
        
        
	}
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
    //Set ball properties like createBall()
    private void prepareDropToFly()
    {
        Vector3 pos = transform.position;
        pos.z = 1;
        ball.transform.position = pos;
		ball.SetActive(false);
    }

	private void throwBall(){

        ball = _gcic.AddDrop();  

        prepareDropToFly();

        ball.SetActive(true);

		ball.GetComponent<CharacterControllerCustomPlayer>().SendFlying(st.GetComponent<CharacterShootTrajectory>().getvect());	

		   
	}
	
	
}