using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor.Build;
using TMPro;
using System;

public class Player : NetworkBehaviour
{
    [Networked] 
    public float timeLeft {get;  set;}
    public TMP_Text timeText;
    [SerializeField] float speed = 5f;
    [SerializeField] GameObject cameraPrefab;
    NetworkCharacterController networkCharacterController;
    private ChangeDetector _changeDetector;
    private bool IsTimeSync = false;
    
    private void Awake() {
        
        
        networkCharacterController = GetComponent<NetworkCharacterController>();
        timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
    }

    private void Initialized(){
        Debug.Log("Initialized called");
    }
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        //initialize timer value
        if(Runner.IsServer){
            Debug.Log("Server initiates the time");
            timeLeft = 120;
        }
        else {
            StartCoroutine(WaitForTimeSync());
        }
        
        if (Object.HasInputAuthority)
        {
            AttachCamera();
            
        }
        
    }

    private IEnumerator WaitForTimeSync()
    {
        
        yield return new WaitForSeconds(5f);
    }

    public override void FixedUpdateNetwork()
    {
        if(Runner.IsClient){
            Debug.Log("Client side time in FUN"+ timeLeft);
        }
        if (GetInput(out NetworkInputData data))
        {
            //data.direction.Normalize();
            networkCharacterController.Move(100*data.direction*Runner.DeltaTime);
        }
        if(Runner.IsServer){
            Debug.Log("Server calls Countdown");
            Countdown();
        }
        
        
    }
    
    private void Update() {
        //Detect changes to the timeLeft networked var
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            // switch (change)
            // {
            //     case nameof(timeLeft):
            //         DisplayTime();
            //         break;
            // }
            if (change == nameof(timeLeft))
            {
                Debug.Log("TimeLeft changed: " + timeLeft);

                // Mark that the timeLeft has been synchronized for the client
                IsTimeSync = true;
                DisplayTime();
                
            }
        }
    }
    
    
    void AttachCamera(){
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
    
    void Countdown(){
        Debug.Log("Server inside Countdown");
        if(timeLeft > 0){
            Debug.Log("Server deducting time");
            timeLeft -= Runner.DeltaTime; ;
        }
        else if (timeLeft < 0)
        {

            timeLeft = 0;
            PlayerController.GameOverState = true;
            Debug.Log("Time's Up!");
        }
        if(Runner.IsClient){
            Debug.Log("Client side time in Countdown"+ timeLeft);
        }
    }
    void DisplayTime(){
        Debug.Log("Time should be displaying now");
        
        
        if(timeText == null){
            Debug.Log("Time Text UI not found");
            return;
        }
        if(Runner.IsServer){
            Debug.Log("Server side time" + timeLeft);
        }
        if(Runner.IsClient){
            Debug.Log("Client side time "+ timeLeft);
        }
        int minutes = Mathf.FloorToInt(timeLeft /60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        Debug.Log("timeLeft "+timeLeft);
        //timeText.text = "Time: " + string.Format("{0:00}:{1:00}",minutes,seconds);
        timeText.text = $"Time: {minutes:00}:{seconds:00}"; 
    }
}
