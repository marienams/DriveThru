using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor.Build;

public class GoalBehaviour : NetworkBehaviour

{
    public bool _hasReached {get; set;}
    private void Awake() {
        _hasReached = false;
    }

    
}
