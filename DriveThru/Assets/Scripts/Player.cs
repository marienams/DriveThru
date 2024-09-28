using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor.Build;
using TMPro;
using System;
using Fusion.LagCompensation;

public class Player : NetworkBehaviour
{
    //--------------MOVEMENT AND CAMERA
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 10f;
    public Vector3 offset = new Vector3(0,3,-15);
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] private LayerMask _goalCollisionLayer;
    //---------------------TIMER
    [Networked] 
    public float timeLeft {get;  set;}
     public TMP_Text timeText;
     private bool IsTimeSync = false;
    //-------------------------MULTIPLAYER
    [Networked][HideInInspector]
    public NetworkString<_16> NickName { get; private set; }

    PlayerOverviewUI playerOverview = null;
    NetworkCharacterController networkCharacterController;
    private ChangeDetector _changeDetector;
    //NetworkRunner runner;
    
    //---------------------------GOAL
    [SerializeField] private TextMeshProUGUI _playerExitUI = null;
    [Networked]
    public bool isGoalComplete{ get; private set; }
    NetworkedTimeTracker _timetracker;
    
    private void Awake() {
        
        
        networkCharacterController = GetComponent<NetworkCharacterController>();
        timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
        //runner = FindObjectOfType<NetworkRunner>();
        
        
    }

    
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        _timetracker = FindObjectOfType<NetworkedTimeTracker>();
        isGoalComplete = false;
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
            //set player nick name through remote procedure call
            RpcSetNickName(playerName);
        }
        // attaching local camera to local player, not networked
        if (Object.HasInputAuthority)
        {
            AttachCamera();
            
        }
        // setting displaying player stats
        playerOverview = FindObjectOfType<PlayerOverviewUI>();
        playerOverview.AddPlayer(Object.InputAuthority, this);
        playerOverview.UpdatePlayerName(Object.InputAuthority, NickName.ToString());
        isGoalComplete = FindAnyObjectByType<GoalBehaviour>()._hasReached;
        
    }

    private IEnumerator WaitForTimeSync()
    {
        
        yield return new WaitForSeconds(5f);
    }

    // public override void FixedUpdateNetwork()
    // {
    //     Movement();
        
    // }

    void Movement(){
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
    }
    
    public override void FixedUpdateNetwork() {
        Movement();
        //Detect changes to the timeLeft networked var
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            
            switch(change)
            {
                case nameof(NickName):
                    playerOverview.UpdatePlayerName(Object.InputAuthority, NickName.ToString());
                    break;
                case nameof(timeLeft):
                    DisplayTime();
                    break;
                case nameof(isGoalComplete):
                    //Remove Entries when a player wins
                    Debug.Log("Goal var change detected");
                    
                    
                    break;
            }
        }
        
    }
    private void OnTriggerEnter(Collider other) {
        
        var goalBehaviour = other.GetComponent<GoalBehaviour>();
        
        //detecting goal collision
        if(Object.HasInputAuthority && other.CompareTag("Goal") && !goalBehaviour._hasReached){
            Debug.Log("Goal triggered");
            // goalBehaviour._hasReached = true;
            isGoalComplete = true;
            // playerOverview.DisplayEndScreen();
            RPCreachedGoal();
            // if(_timetracker == null) {Debug.Log("Time tracker instance empty"); return;}
            // _timetracker.GameHasEnded(NickName.ToString(), isGoalComplete);
        }
    }
    
    void AttachCamera(){
        // Instantiate the camera prefab

        GameObject playerCamera = Instantiate(cameraPrefab, this.transform);

        // Set the camera's position and parent it to the player
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = transform.position + offset; // Adjust this offset as needed
        playerCamera.transform.localRotation = transform.rotation;

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

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCreachedGoal()
    {
        //Debug.Log($"{NickName} has reached the goal!");
        isGoalComplete = true;
        // Debug.Log("Goal status RPC communicated");
        // playerOverview.UpdateEndScreen(NickName.ToString());
        
        // Add further game logic here, like ending the game or progressing to the next level
        playerOverview.DisplayEndScreen(Object.InputAuthority,NickName.ToString());
    }

    public void Restart(){
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner.IsServer)
        {
            // Shutdown the NetworkRunner and stop the session
            runner.Shutdown();
            Debug.Log("Runner shut down.");
        }
        else
        {
            Debug.Log("Only the server can shutdown the session.");
        }
    }

    
}
