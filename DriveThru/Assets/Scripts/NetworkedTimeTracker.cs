using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkedTimeTracker : NetworkBehaviour {

    [Networked] private TickTimer _timer { get; set; }
    [SerializeField] private float _endDelay = 4.0f;
    [SerializeField] TextMeshProUGUI _endScreen;
    string _winner;
    bool hasGameEnded;

    public override void Spawned()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        // if (_timer.IsRunning && !_timer.ExpiredOrNotRunning(Runner))
        //used the above code but are you stoopid? if timer stops running it will not
        //not run the UpdateEndScreen function again
        if (hasGameEnded)
        {
                
            Debug.Log("Time Tracker: Timer will be updated now");
            UpdateEndScreen();
            
        }
    }

    // public void GameHasEnded(string name)
    // {
    //     _winner = name;
    //     Debug.Log("Time Tracker: timer started");
    //     _timer = TickTimer.CreateFromSeconds(Runner, _endDelay);
    //     _endScreen.gameObject.SetActive(true);
    // }

    public void GetWinner(string name){
        _winner = name;
    }

    public void GameHasEnded(string name, bool gameEnded)
    {
        _winner = name;
        hasGameEnded = gameEnded;
        Debug.Log("Time Tracker: timer started");
        _timer = TickTimer.CreateFromSeconds(Runner, _endDelay);
        Debug.Log($"Timer State - Running: {_timer.IsRunning}, Remaining Time: {_timer.RemainingTime(Runner)}");
        _endScreen.gameObject.SetActive(true);
    }

    public void UpdateEndScreen()
    {
        Debug.Log("Time Tracker: screen updated");
        _endScreen.text =
                $"{_winner} won. Disconnecting in {Mathf.RoundToInt(_timer.RemainingTime(Runner) ?? 0)}";
        Debug.Log($"Timer Check - IsRunning: {_timer.IsRunning}, Expired: {_timer.ExpiredOrNotRunning(Runner)}");
        if (_timer.ExpiredOrNotRunning(Runner) == false) return;
        Debug.Log("Should ShutDown Now");
        Runner.Shutdown();
        SceneManager.LoadScene(0);
    }

}

