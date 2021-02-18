using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.SceneManagement;

public class scr_LobbyPlayer : NetworkRoomPlayer
{
    [SyncVar] public string myRole = "None";
    public string publicIp = "";
    private Vector2 scrollPosition = Vector2.zero;

    private void Start()
    {
        base.Start();
        publicIp = new WebClient().DownloadString("http://icanhazip.com");
        CmdChangeRole("None");
    }

    private void Update()
    {
        if (!isLocalPlayer) 
        {
            return;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            CmdChangeRole("Adventurer");
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            CmdChangeRole("DM");
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            CmdChangeReadyState(!readyToBegin);
        }
    }

    [Command]
    private void CmdChangeRole(string role)
    {
        myRole = role;
    }

    [ClientRpc]
    public void RpcChangeName(string s)
    {
        name = s;
    }
    
    public override void OnGUI()
    {
        if (!isLocalPlayer)
            return;
        if (!showRoomGUI)
            return;

        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        if (!room)
            return;
            
        if (!room.showRoomGUI)
            return;

        if (SceneManager.GetActiveScene().name != room.RoomScene)
            return;

        const float gapHorizontal = 10f;
        GUILayout.BeginArea(new Rect(gapHorizontal, 180f, Screen.width / 2 - 10 * 2, Screen.height / 2));
        GUI.Box(new Rect(0, 0, Screen.width / 2 - gapHorizontal * 2, Screen.height / 2), "PLAYERS");
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, false, 
            GUILayout.Width(Screen.width / 2 - gapHorizontal * 2), GUILayout.Height(Screen.height / 2));
        
        GUILayout.BeginHorizontal("", GUIStyle.none);
        foreach (var player in room.roomSlots)
        {
            player.DrawGui();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
        
        if (NetworkClient.active && isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(gapHorizontal * 2, 180f + Screen.height / 2 - 50f, 120f, 20f));

            if (readyToBegin)
            {
                if (GUILayout.Button("Cancel"))
                    CmdChangeReadyState(false);
            }
            else
            {
                if (GUILayout.Button("Ready"))
                    CmdChangeReadyState(true);
            }

            GUILayout.EndArea();
        }
    }

    public override void DrawGui()
    {
        GUILayout.BeginVertical();

        GUILayout.Label($"Player [{index + 1}]");

        if (readyToBegin)
            GUILayout.Label("Ready");
        else
            GUILayout.Label("Not Ready");
        GUILayout.Label("Role:");
        GUILayout.Label(myRole);

        if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE"))
        {
            // This button only shows on the Host for all players other than the Host
            // Host and Players can't remove themselves (stop the client instead)
            // Host can kick a Player this way.
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }

        GUILayout.EndVertical();
    }
}