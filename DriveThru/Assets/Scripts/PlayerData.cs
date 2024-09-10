using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private string _playerName = null;
    // Start is called before the first frame update
    void Start()
    {
        var count = FindObjectsOfType<PlayerData>().Length;
            if (count > 1)
            {
                Destroy(gameObject);
                return;
            }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPlayerName(string name){
        _playerName = name;

    }
    public string GetPlayerName(){
        if (string.IsNullOrWhiteSpace(_playerName))
            {
                _playerName = GetRandomName();
            }
        return _playerName;
    }
    public static string GetRandomName()
        {
            var rngPlayerNumber = Random.Range(0, 9999);
            return $"Player {rngPlayerNumber.ToString("0000")}";
        }
}
