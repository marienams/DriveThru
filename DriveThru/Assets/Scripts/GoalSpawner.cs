using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GoalSpawner : NetworkBehaviour 
{
    [SerializeField] GoalBehaviour goalPrefab = null;
    bool hasGoalPrefab = false;
    private List<NetworkId> _goals = new List<NetworkId>();
    private void Awake() 
    {
        // var runner = FindAnyObjectByType<NetworkRunner>();
        // if(runner.IsServer){
        //     SpawnGoal();
        // }
        //SpawnGoal();
        
    }
    
    
    public void SpawnGoal()
    {
        if(!hasGoalPrefab)
        {
            int x = Random.Range(-140,140);
            int z = Random.Range(0,200);
            Vector3 position = new Vector3(x, 2.5f, z);
            var goal = Runner.Spawn(goalPrefab, position);
            
            hasGoalPrefab = true;
        }
        
    }
    
}

