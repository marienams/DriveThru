using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 5.0f;
    public float turnSpeed;
    public float horizontalInput;
    
    public float forwardInput;
    public Camera mainCamera;
    public Camera hoodCamera;
    public KeyCode switchKey;
    public string inputID; //for local multiplayer
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //we will move forward our vehicle
        //horizontalInput = Input.GetAxis("Horizontal");
        //forwardInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("horizontal" + inputID);
        forwardInput = Input.GetAxis("vertical" + inputID);
        transform.Translate(Vector3.forward * Time.deltaTime * speed *forwardInput);
        //transform.Rotate(Vector3.forward * Time.deltaTime * turnSpeed *horizontalInput);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

        if (Input.GetKeyDown(switchKey))                        // condition to switch camera through key press
        {
            mainCamera.enabled = !mainCamera.enabled;
            hoodCamera.enabled = !hoodCamera.enabled;
        }
        
    }
}
