using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Obstacle : NetworkBehaviour
{
    public float speed = 10.0f;
    // Start is called before the first frame update
    public override void Spawned()
    {
        Debug.Log("Obstacle Spanwed " + this.transform.position);
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        transform.Translate(Vector3.forward * Runner.DeltaTime * speed);
    }
    //What I did to sync obstacle vehicle across client and host
    //1. changed Time.deltaTime to Runner.deltaTime
    //2. in obstacle spawner script, limited the spawning to the host and
    //3. added network transform component to the obstacle vehicle prefab
    //4. while spawning added Runner.localPlayer as the playerRef in the spawn() 
}
