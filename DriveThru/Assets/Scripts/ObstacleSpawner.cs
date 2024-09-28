using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;



public class ObstacleSpawner : NetworkBehaviour 
{
    [SerializeField] GameObject obstaclePrefab = null;
    private List<NetworkId> _obstacles = new List<NetworkId>();
    public override void FixedUpdateNetwork()
    {
        SpawnObstacles();
    }

    void SpawnObstacles(){
        Debug.Log("Obstacles should be spawning");
        int x = Random.Range(-140,140);
        int z = Random.Range(0,200);
        Vector3 position = new Vector3(x, -3, z);
        var obstacle = Runner.Spawn(obstaclePrefab, position);
        _obstacles.Add(obstacle);
    }

}
