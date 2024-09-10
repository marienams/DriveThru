using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Fusion.NetworkBehaviour;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TextMeshProUGUI _playerNamePlaceholder = null;
    [SerializeField] private TMP_Text warningMessage;
   
    [SerializeField] private PlayerData playerDataPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner _runner;
    


    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        

        // Create the NetworkSceneInfo from the current scene
        //var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var scene = SceneRef.FromIndex(1);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        
        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = $"{roomName.text}",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    public void HostGame(){
        Debug.Log("Room Name: " + roomName.text);
        if(roomName.text == ""){
            warningMessage.text = "Enter Room Name";
            return;
        }
        SetPlayerData();
        StartGame(GameMode.Host);
    }
    public void JoinGame(){
        if(roomName.text == ""){
            warningMessage.text = "Enter Room Name";
            return;
        }
        SetPlayerData();
        StartGame(GameMode.Client);
    }

    private void SetPlayerData()
    {
        var _playerData = FindObjectOfType<PlayerData>();
        if(_playerData == null){
            Debug.Log("Player data instantiated");
            _playerData = Instantiate(playerDataPrefab);
        }

        if(string.IsNullOrWhiteSpace(playerName.text)){
            _playerData.SetPlayerName(_playerNamePlaceholder.text);
        }
        else 
        {
            _playerData.SetPlayerName(playerName.text);
        }
    }

    public void QuitGame() {
        Application.Quit();
    }
    // private void OnGUI()
    // {
    // if (_runner == null)
    // {
    //     if (GUI.Button(new Rect(0,0,200,40), "Host"))
    //     {
    //         StartGame(GameMode.Host);
    //     }
    //     if (GUI.Button(new Rect(0,40,200,40), "Join"))
    //     {
    //         StartGame(GameMode.Client);
    //     }
    // }
    // }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Event: OnConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Event: OnConnectFailed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Event: OnConnectRequest");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       Debug.Log("Event: OnCustomAuthenticationResponse");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
       Debug.Log("Event: OnDisconnectedFromServer");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Event: OnHostMigration");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W)){
            
            data.direction += Vector3.forward;
        }
            

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("EventOnInputMissing");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log("Event: OnObjectEnterAOI");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log("Event: OnObjectExitAOI");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Event: OnPlayerJoined" + runner.Mode);
        
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 3f, (player.RawEncoded % runner.Config.Simulation.PlayerCount) * (-3));
            // similar to instantiating a gameobject
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);

        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
       Debug.Log("Event: OnPlayerLeft");
       if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
       Debug.Log("Event: OnReliableDataProgress");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
       Debug.Log("Event: OnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Event: OnSceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Event: OnSceneLoadStart");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Event: OnSessionListUpdated");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Event: OnShutdown");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       Debug.Log("Event: OnUserSimulationMessage");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    


}
