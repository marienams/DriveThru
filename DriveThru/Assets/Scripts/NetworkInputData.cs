using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData :  INetworkInput
{
    public Vector3 direction;
    public float rotation;
    public float timeLeft;
     //sturcture to store user input
    // for moving ahead, vertical
    // public float forwardInput;
    // // for rotating, horizontal
    // public float horizontalInput;
    // public Vector2 scale;

    // public bool isCameraSwitched;
}
