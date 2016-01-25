﻿using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public GameObject character;
    private float speed = 0.1f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 position = this.transform.position;
            position.z++;
            this.transform.position = position;
           

        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 position = this.transform.position;
            position.z--;
            this.transform.position = position;
            
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 position = this.transform.position;
            position.x++;
            this.transform.position = position;
          
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3 position = this.transform.position;
            position.x--;
            this.transform.position = position;
           
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 position = this.transform.position;
            position.y++;
            this.transform.position = position;

        }
    }
}