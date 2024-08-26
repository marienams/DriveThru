using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0,5,-7);

    public float followSpeed = 10f;  // Speed at which the camera follows the player
    public float rotateSpeed = 10f;  // Speed at which the camera rotates to match the player's rotation

    //code cleanup; good practce: no hard coded values
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //offset camera position to above the player vehicle
        //transform.position = player.transform.position + offset;

         // Follow player's position
         if(player!= null){
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Follow player's rotation
            Quaternion desiredRotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, rotateSpeed * Time.deltaTime);
            transform.rotation = desiredRotation;
         }
        
}
}
