using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0,5,-7);
    //code cleanup; good practce: no hard coded values
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //offset camera position to above the player vehicle
        transform.position = player.transform.position + offset;
}
}
