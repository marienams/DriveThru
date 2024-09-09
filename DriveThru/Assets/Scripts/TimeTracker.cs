// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Fusion;
// using Fusion.Sockets;

// public class TimeTracker : NetworkBehaviour
// {
//     //public TextMeshProUGUI timeUI;
    
//     [Networked] 
//     public float timeLeft {get;  set;}
    

//     public float countdownDuration = 120;
//     public float previousTimeLeft;
//     public TMP_Text timeText;
//     private NetworkRunner runner;
//     private ChangeDetector _changeDetector;
//     private void Awake() {
//         timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
//     }
    
//     private void Spawned() {

        
//         _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
//         if(Object.HasStateAuthority){
//             timeLeft = countdownDuration;
//             //Debug.Log("I have StateAuthority and set timeLeft to: " + timeLeft);
//         }
//         previousTimeLeft = timeLeft;
//         // if (!Object.HasStateAuthority) {
//         //     //Debug.Log("Client received updated timeLeft: " + timeLeft);
//         // }
        
//     }
    

//     public override void FixedUpdateNetwork()
//     {
//         // if(Object.HasStateAuthority){
            
//         //     Debug.Log("I have StateAuthority and set timeLeft to: " + timeLeft);
//         // }
        
//         // if (!Object.HasStateAuthority) {
//         //     Debug.Log("Client received updated timeLeft: " + timeLeft);
//         // }
        
//         // if (Mathf.Abs(timeLeft - previousTimeLeft) > Mathf.Epsilon)
//         // {
//         //     Debug.Log("Time Change detected");
//         //     previousTimeLeft = timeLeft;  // Update the previous value
//         // }
//     }
//     private void Update() {
//         if(Object.HasStateAuthority && timeLeft >0){
            
//             Countdown();
//         }
//     }

//     void Countdown()
//     {
        
//         if(timeLeft > 0){
//             Debug.Log("Time deducting");
//             timeLeft -= Runner.DeltaTime; ;
//         }
//         else if (timeLeft < 0)
//         {
//             timeLeft = 0;
//             PlayerController.GameOverState = true;
//             Debug.Log("Time's Up!");
//         }

//         foreach (var change in _changeDetector.DetectChanges(this))
//         {
//             switch (change)
//             {
//                 case nameof(timeLeft):
//                     Debug.Log("timeLeft changed");
//                     break;
//             }
//         }
        
//     }
//     void DisplayTime(){
//         Debug.Log("Time should be displaying now");
        
        
//         if(timeText == null){
//             Debug.Log("Time Text UI not found");
//             return;
//         }
//         int minutes = Mathf.FloorToInt(timeLeft /60);
//         int seconds = Mathf.FloorToInt(timeLeft % 60);
//         Debug.Log("timeLeft "+timeLeft);
//         timeText.text = "Timo: " + string.Format("{0:00}:{1:00}",minutes,seconds);
//     }
    
        
// }
