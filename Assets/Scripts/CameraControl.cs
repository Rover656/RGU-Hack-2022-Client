using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float increase = 30f;
    public float zoomAmount = 0.5f;
    public float zoom = 60f;

    void Start()
    { 
    
    }

    // Update is called once per frame
    void Update()
    { 
        float angle = 0.0f;
        float anglePitch = 0.0f; 

        //Left and right camera movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle += increase;
            Debug.Log("Help");
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            angle -= increase;
            Debug.Log("Help 2 Electric boogaloo");
        }
        
        transform.RotateAround(Vector3.zero, Vector3.up, angle * Time.deltaTime);
        
        //Upwards and downwards camera movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            anglePitch -= increase;
            Debug.Log("Help but up");
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            anglePitch += increase;
            Debug.Log("Help but down");
        }

        transform.Rotate(Vector3.right, anglePitch * Time.deltaTime);

        //Zoom controls
        if (Input.GetKeyDown(KeyCode.Q))
        {
            zoom += zoomAmount;
            Debug.Log("Zoom in");
            
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            zoom -= zoomAmount;
            Debug.Log("Zoom out");
        }

        Camera.main.fieldOfView = Mathf.Clamp(zoom, 15.0f, 100.0f);

    }
}
