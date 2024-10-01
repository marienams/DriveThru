using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;



public class ObstacleSpawner : NetworkBehaviour 
{
    [SerializeField] GameObject obstaclePrefab = null;
    
    [SerializeField] private float _minSpawnDelay = 1.0f;
    [SerializeField] private float _maxSpawnDelay = 1.2f;

    private List<NetworkId> _obstacles = new List<NetworkId>();
    private TickTimer _spawnDelay;
    GoalBehaviour goalReached;
    public override void FixedUpdateNetwork()
    {
        
        SpawnObstacles();
        goalReached = FindObjectOfType<GoalBehaviour>();
    }

    void SpawnObstacles()
    {
        //if the ticker is not expired, it won't spawn an obstacle vehicle
        if (_spawnDelay.Expired(Runner) == false) return;
        //if(goalReached._hasReached == true) Debug.Log("Stop obstacle spawning"); return;
        
        int x = Random.Range(-140,140);
        int z = Random.Range(0,200);
        int yAngle = Random.Range(90,270);
        Vector3 position = new Vector3(x, 6f, z);
        Quaternion rotation = Quaternion.Euler(0,yAngle,0);
        // var obstacle = Runner.Spawn(obstaclePrefab, position);
        // Debug.Log("New Obstacle Position "+ obstacle.transform.position);
        // _obstacles.Add(obstacle);

        // //Start the delay for spawning
        // SetSpawnDelay("Obstacle SPawner called set delay");
        if (Runner.IsServer)
        {
            // Spawn the obstacle prefab at the generated position and sync across clients
            var obstacle = Runner.Spawn(obstaclePrefab, position, rotation, Runner.LocalPlayer);

            // Add the obstacle to the list
            _obstacles.Add(obstacle);

            // Start the delay for spawning the next obstacle
            SetSpawnDelay();
        }
    }

    public void SetSpawnDelay()
        {
            
            // Chose a random amount of time until the next spawn.
            var time = Random.Range(_minSpawnDelay, _maxSpawnDelay);
            _spawnDelay = TickTimer.CreateFromSeconds(Runner, time);
        }

}
