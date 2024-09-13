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
    [Networked]
    public NetworkString<_16> NickName { get; private set; }

    public TMP_Text timeText;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 10f;
    public Vector3 offset = new Vector3(0,3,-10);
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] PlayerOverviewUI playerOverview = null;
    NetworkCharacterController networkCharacterController;
    private ChangeDetector _changeDetector;
    private bool IsTimeSync = false;
    
    
    private void Awake() {
        
        
        networkCharacterController = GetComponent<NetworkCharacterController>();
        timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
    }

    
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        //initialize timer value
        if(Runner.IsServer){
            Debug.Log("Server initiates the time");
            timeLeft = 120;
        }
        if(Object.HasInputAuthority) 
        {
            // waiting to get the timer value: FIX IT
            StartCoroutine(WaitForTimeSync());
            // for getting player name from playerData.cs
            var playerName = FindObjectOfType<PlayerData>().GetPlayerName();
            
            RpcSetNickName(playerName);
        }
        // attaching local camera to local player, not networked
        if (Object.HasInputAuthority)
        {
            AttachCamera();
            
        }
        // displaying player stats
        playerOverview = FindObjectOfType<PlayerOverviewUI>();
        playerOverview.AddPlayer(Object.InputAuthority, this);
        playerOverview.UpdatePlayerName(Object.InputAuthority, NickName.ToString());
        
        
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
            //networkCharacterController.Move(100*data.direction*Runner.DeltaTime);
            // Apply movement based on direction input
            Vector3 movement = data.direction.normalized * moveSpeed * Runner.DeltaTime;
            transform.position += transform.forward * movement.z;

            // Apply rotation based on rotation input
            if (data.rotation != 0)
            {
                transform.Rotate(0, data.rotation * rotateSpeed * Runner.DeltaTime, 0);
            }
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
            // if (change == nameof(timeLeft))
            // {
            //     Debug.Log("TimeLeft changed: " + timeLeft);

            //     // Mark that the timeLeft has been synchronized for the client
            //     IsTimeSync = true;
            //     DisplayTime();
                
            // }
            switch(change)
            {
                case nameof(NickName):
                    playerOverview.UpdatePlayerName(Object.InputAuthority, NickName.ToString());
                    break;
                case nameof(timeLeft):
                    DisplayTime();
                    break;
            }
        }
    }
    
    
    void AttachCamera(){
        // Instantiate the camera prefab

            GameObject playerCamera = Instantiate(cameraPrefab, this.transform);

            // Set the camera's position and parent it to the player
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = transform.position + offset; // Adjust this offset as needed
            playerCamera.transform.localRotation = transform.rotation;

            // Ensure the camera follows the player
            // FollowPlayer cameraFollow = playerCamera.GetComponent<FollowPlayer>();
            // if (cameraFollow != null)
            // {
            //     cameraFollow.player = transform;
            // }
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
    // RPC used to send player information to the Host
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RpcSetNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return;
            Debug.Log("RPC call for player "+nickName);
            NickName = nickName;
        }

        
}
