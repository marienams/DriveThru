using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeTracker : MonoBehaviour
{
    public TextMeshProUGUI timeUI;
    [SerializeField]float timeLeft;
    
    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
    }

    void Countdown()
    {
        if(timeLeft > 0){
            
            timeLeft -= Time.deltaTime;
        }
        else if (timeLeft < 0)
        {
            timeLeft = 0;
            PlayerController.GameOverState = true;
            Debug.Log("Time's Up!");
        }
        int minutes = Mathf.FloorToInt(timeLeft /60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timeUI.text = "Time: " + string.Format("{0:00}:{1:00}",minutes,seconds);
    }
        
}
