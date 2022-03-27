using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float increase = 30f;
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    { 
        float angle = 0.0f;
        float anglePitch = 0.0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle += increase;
            Debug.Log("Help");
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            angle -= increase;
            Debug.Log("Help 2 Electric boogaloo");
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            anglePitch -= increase;
            Debug.Log("Help but up");
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            anglePitch += increase;
            Debug.Log("Help but down");
        }

        transform.RotateAround(Vector3.zero, Vector3.up, angle * Time.deltaTime);
        transform.Rotate(Vector3.right, anglePitch * Time.deltaTime);
    }
}
