using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor.Build;

public class Player : NetworkBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] GameObject cameraPrefab;
    NetworkCharacterController networkCharacterController;
    private void Awake() {
        //networkCharacterController = GetComponent<NetworkCharacterController>();
        //networkCharacterController.enabled = false;
         networkCharacterController = GetComponent<NetworkCharacterController>();
        
    }
    

    private void Start()
    {
        // Only enable the camera for the local player
        // if (Object.HasInputAuthority)
        // {

        //     playerCamera.gameObject.SetActive(true);
        //     playerCamera.transform.SetParent(null); // Ensure the camera is not a child of the player prefab
        // }
        // else
        // {
        //     playerCamera.gameObject.SetActive(false);
        // }
    }
    private void Update() {
        
        // Only move the camera for the local player
        // if (Object.HasInputAuthority)
        // {
        //     // Update the camera position and rotation to follow the player
        //     playerCamera.transform.position = transform.position + new Vector3(0, 5, -10); // Example offset
        //     playerCamera.transform.LookAt(transform.position);
        // }
    }
    public override void FixedUpdateNetwork()
    {
        

        if (GetInput(out NetworkInputData data))
        {
            Debug.Log("Input Data: " + data.direction);
            //data.direction.Normalize();
            networkCharacterController.Move(100*data.direction*Runner.DeltaTime);
        }
    }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            // Instantiate the camera prefab
            GameObject playerCamera = Instantiate(cameraPrefab);

            // Set the camera's position and parent it to the player
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0, 5, -10); // Adjust this offset as needed
            playerCamera.transform.localRotation = Quaternion.identity;

            // Ensure the camera follows the player
            FollowPlayer cameraFollow = playerCamera.GetComponent<FollowPlayer>();
            if (cameraFollow != null)
            {
                cameraFollow.player = transform;
            }
        }
    }
}
