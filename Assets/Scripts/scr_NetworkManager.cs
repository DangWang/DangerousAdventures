using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    public string publicIp = "";

    public override void Start()
    {
        base.Start();
        publicIp = new WebClient().DownloadString("http://icanhazip.com");
        print(publicIp);
    }

    public override void OnStartHost(bool inMatchmaker = true)
    {
        base.OnStartHost();
        if (inMatchmaker)
        {
            SendServerInfoToMatchmaker();
        }

    }

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
        // else
        // {
        //     print("Connection: " + conn.connectionId);
        //     if (pInfo[conn.connectionId].role == "DM")
        //     {
        //         newPlayer = Instantiate(spawnPrefabs[1], Vector3.zero, Quaternion.identity);
        //         newPlayer.GetComponent<scr_Player>().connID = conn.connectionId;
        //         pInfo[conn.connectionId].mainObject = newPlayer;
        //         NetworkServer.AddPlayerForConnection(conn, newPlayer);
        //     }
        //     else if (pInfo[conn.connectionId].role == "Adventurer")
        //     {
        //         newPlayer = Instantiate(spawnPrefabs[0], Vector3.zero, Quaternion.identity);
        //         newPlayer.GetComponent<scr_Player>().connID = conn.connectionId;
        //         pInfo[conn.connectionId].mainObject = newPlayer;
        //         NetworkServer.AddPlayerForConnection(conn, newPlayer);
        //     }
        //
        //     script_GameManager.myPlayers[script_GameManager.playersInGame] = newPlayer.GetComponent<scr_Player>();
        //     script_GameManager.playersInGame++;
        // }
    }

    public override bool OnRoomServerSceneLoadedForPlayer(GameObject roomPlayer, GameObject gamePlayer)
    {
        script_GameManager.myPlayers[script_GameManager.playersInGame] = gamePlayer.GetComponent<scr_Player>();
        script_GameManager.playersInGame++;
        return base.OnRoomServerSceneLoadedForPlayer(roomPlayer, gamePlayer);
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        if (roomPlayer.GetComponent<scr_LobbyPlayer>().myRole == "DM")
        {
            return Instantiate(spawnPrefabs[1], Vector3.zero, Quaternion.identity);
        }
        else
        {
            return Instantiate(spawnPrefabs[0], Vector3.zero, Quaternion.identity);
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
        
        if (SceneManager.GetActiveScene().name == RoomScene && (NetworkServer.active || NetworkClient.isConnected))
        {
            const float gapHorizontal = 10f;
            GUI.Box(new Rect(Screen.width / 2 + gapHorizontal, 180f, Screen.width / 2 - gapHorizontal * 2, Screen.height / 2), "SERVER SETTINGS");
            GUILayout.BeginArea(new Rect(Screen.width / 2 + gapHorizontal * 2, 200f, Screen.width / 2 - gapHorizontal * 2, Screen.height / 2 - 20f));
            GUILayout.Label("Server IP:");
            GUILayout.Label(publicIp);
            GUILayout.Label("Players:");
            GUILayout.Label(minPlayers.ToString() + "-" + maxConnections.ToString());
            GUILayout.EndArea();
        }
    }
    
    void SendServerInfoToMatchmaker() 
    {
        IPAddress ipAddr = IPAddress.Parse("ip.ip.ip.ip");
        if (Equals(GetComponent<scr_NetworkManager>().publicIp, "ip.ip.ip.ip\n")) // if the matchmaking server is local
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipHost.AddressList[1];
        }
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 7002);

        // Creation TCP/IP Socket using  
        // Socket Class Constructor 
        Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Connect Socket to the remote  
        // endpoint using method Connect() 
        sender.Connect(localEndPoint);

        // We print EndPoint information  
        // that we are connected 
        Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

        // Creation of message that 
        // we will send to Server 
        byte[] messageSent = Encoding.ASCII.GetBytes("test server" + "-" + publicIp + "-" + "<EOF>");
        int byteSent = sender.Send(messageSent);

        // Data buffer 
        byte[] messageReceived = new byte[1024];

        // We receive the messagge using  
        // the method Receive(). This  
        // method returns number of bytes 
        // received, that we'll use to  
        // convert them to string 
        while (true)
        {
            int byteRecv = sender.Receive(messageReceived);
            Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));
            if (Encoding.ASCII.GetString(messageReceived, 0, byteRecv) == "OVER")
            {
                break;
            }
        }

        // Close Socket using  
        // the method Close() 
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    } 
}
