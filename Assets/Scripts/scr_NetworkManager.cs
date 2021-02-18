using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_NetworkManager : NetworkRoomManager
{
    public class PlayerInfo
    {
        public NetworkConnection connection;
        public GameObject mainObject;
        public string role = "";

        public PlayerInfo(NetworkConnection conn)
        {
            connection = conn;
        }
    }
    
    public bool loaded;
    public Dictionary<int, PlayerInfo> pInfo = new Dictionary<int, PlayerInfo>();

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        GameObject newPlayer = null;
        if (loaded == false)
        {
            print("Connection: " + conn.connectionId);
            newPlayer = Instantiate(spawnPrefabs[2], Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, newPlayer);
            pInfo.Add(conn.connectionId, new PlayerInfo(conn));
            pInfo[conn.connectionId].mainObject = GameObject.Find("LobbyPlayer(Clone)");
            foreach (var entry in pInfo)
            {
                entry.Value.mainObject.GetComponent<scr_LobbyPlayer>().RpcChangeName("Player" + entry.Value.connection.connectionId);
            }

            script_GameManager.playersInLobby++;
        }
        else
        {
            print("Connection: " + conn.connectionId);
            if (pInfo[conn.connectionId].role == "DM")
            {
                newPlayer = Instantiate(spawnPrefabs[1], Vector3.zero, Quaternion.identity);
                newPlayer.GetComponent<scr_Player>().connID = conn.connectionId;
                pInfo[conn.connectionId].mainObject = newPlayer;
                NetworkServer.AddPlayerForConnection(conn, newPlayer);
            }
            else if (pInfo[conn.connectionId].role == "Adventurer")
            {
                newPlayer = Instantiate(spawnPrefabs[0], Vector3.zero, Quaternion.identity);
                newPlayer.GetComponent<scr_Player>().connID = conn.connectionId;
                pInfo[conn.connectionId].mainObject = newPlayer;
                NetworkServer.AddPlayerForConnection(conn, newPlayer);
            }

            script_GameManager.myPlayers[script_GameManager.playersInGame] = newPlayer.GetComponent<scr_Player>();
            script_GameManager.playersInGame++;
        }
    }

    private void Update()
    {
        // if (!loaded)
        // {
        //     ready = true;
        //     foreach (var entry in pInfo)
        //     {
        //         if (entry.Value.mainObject.GetComponent<scr_LobbyPlayer>().isReady == false)
        //         {
        //             ready = false;
        //         }
        //     }
        //     if (ready && !loaded && pInfo.Any() && (pInfo[0] != null) & pInfo[0].mainObject.GetComponent<scr_LobbyPlayer>().isReady)
        //     {
        //         foreach (var entry in pInfo)
        //         {
        //             entry.Value.role = entry.Value.mainObject.GetComponent<scr_LobbyPlayer>().myRole;
        //         }
        //         singleton.ServerChangeScene("DM_Control2");
        //         loaded = true;
        //     }
        // }
    }
    
    public override void OnGUI()
    {
        if (!showRoomGUI)
            return;

        if (NetworkServer.active && SceneManager.GetActiveScene().name == GameplayScene)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 150f, 10f, 140f, 30f));
            if (GUILayout.Button("Return to Room"))
                ServerChangeScene(RoomScene);
            GUILayout.EndArea();
        }
        
        if (SceneManager.GetActiveScene().name == RoomScene)
        {
            const float gapHorizontal = 10f;
            GUI.Box(new Rect(Screen.width / 2 + gapHorizontal, 180f, Screen.width / 2 - gapHorizontal * 2, Screen.height / 2), "SERVER SETTINGS");
        }
    }
}