using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerOverviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerOverviewEntryPrefab = null;
    [SerializeField] private TextMeshProUGUI _playerEXITPrefab = null;
    
    private Dictionary<PlayerRef, TextMeshProUGUI> _playerListEntries = new Dictionary<PlayerRef, TextMeshProUGUI>();
    private Dictionary<PlayerRef, string> _playerNickNames = new Dictionary<PlayerRef, string>();
    
    
    
    public void AddPlayer(PlayerRef playerRef, Player player){

        Debug.Log("Add Entry called");

        if (_playerListEntries.ContainsKey(playerRef)) return;
            if (player == null) return;

            var entry = Instantiate(_playerOverviewEntryPrefab, this.transform);
            // // Update the position so that it spawns 1 unit below the last one on the y-axis
            // lastSpawnPosition = this.transform.position;
            // entry.transform.position = lastSpawnPosition - new Vector3(0, 1, 0);

            // // Update the last spawn position for the next prefab
            // lastSpawnPosition = entry.transform.position;
            
            var name = String.Empty;
            //keep a record of player refs and theur respective UI prefab
            _playerListEntries.Add(playerRef, entry);
            // keep record of player names
            _playerNickNames.Add(playerRef, name);
            UpdatePlayerList(playerRef, entry);
    }

    public void UpdatePlayerName(PlayerRef playerRef, string name){
        Debug.Log(name + " player");
        if (_playerListEntries.TryGetValue(playerRef, out var entry) == false) return;
        Debug.Log("Player Name " + name);
        _playerNickNames[playerRef] = name;
        UpdatePlayerList(playerRef, entry);

    }
    
    public void UpdatePlayerList(PlayerRef playerRef, TextMeshProUGUI ui){
        var name = _playerNickNames[playerRef];
        Debug.Log("Setting name to UI in UpdatePlayerList " + name);
        ui.text = $"{name}";
        

    }
    public void RemoveEntry(PlayerRef playerRef)
        {
            if (_playerListEntries.TryGetValue(playerRef, out var entry) == false) return;

            if (entry != null)
            {
                Destroy(entry.gameObject);
            }

            _playerNickNames.Remove(playerRef);

            _playerListEntries.Remove(playerRef);
            
            
        }
    public void ClearEntries(PlayerRef playerRef, string name)
    {
        
        //_playerOverviewExitPrefab.SetActive(true);
        
        _playerNickNames.Clear();
        _playerListEntries.Clear();

    }

    public void DisplayEndScreen(PlayerRef playerRef, string winner){
        Debug.Log("Goal UI set active");
        _playerEXITPrefab.gameObject.SetActive(true);
        _playerEXITPrefab.text = $"{winner} won!";
    }




}
