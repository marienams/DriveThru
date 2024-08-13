using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField]float speed = 5.0f;
    [SerializeField]float turnSpeed;
    float horizontalInput;
    
    float forwardInput;
    public Camera mainCamera;
    public Camera hoodCamera;
    KeyCode switchKey;
    string inputID; //for local multiplayer

    public float targetYPosition = -15.0f; // The Y position to check for
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerOffTrack();
    }
    void PlayerMovement(){
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
    void PlayerOffTrack(){
        if(Mathf.Approximately(transform.position.y, targetYPosition)){
            Debug.Log("Player fell over");
        }
    }
}
