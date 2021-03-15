using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Mirror;

public class scr_NetworkManagerHUD : NetworkManagerHUD
{
    List<ServerInfo> serverInfos = new List<ServerInfo>();
    private bool inServerBrowser = false;
    
    public override void OnGUI()
    {
        if (!showGUI)
            return;
        
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                if (!inServerBrowser)
                {
                    GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
                    // LAN Host
                    if (Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        if (GUILayout.Button("Host (in server browser)"))
                        {
                            manager.StartHost();
                        }
                        
                        if (GUILayout.Button("Host (private)"))
                        {
                            manager.StartHost(false);
                            
                        }
                    }

                    // LAN Client + IP
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("LAN Client"))
                    {
                        manager.StartClient();
                    }

                    manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                    GUILayout.EndHorizontal();

                    // LAN Server Only
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        GUILayout.Box("(  WebGL cannot be server  )");
                    }
                    else
                    {
                        if (GUILayout.Button("LAN Server Only")) manager.StartServer();
                    }
                    
                    // Server Browser
                    if (GUILayout.Button("Server Browser"))
                    {
                        inServerBrowser = true;
                        serverInfos = RequestServersFromMatchmaker();
                    }
                    GUILayout.EndArea();
                }
                else
                {
                    GUILayout.BeginArea(new Rect(30, 20, Screen.width, Screen.height));
                    GUI.Box(new Rect(0, 30, Screen.width - 60f, Screen.height - 100f), "SERVER BROWSER");
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Back"))
                    {
                        inServerBrowser = false;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginArea(new Rect(0, 50, Screen.width, Screen.height));
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Name");
                    foreach (var server in serverInfos)
                    {
                        GUILayout.Label(server.name);
                    }
                    GUILayout.EndVertical();
                    
                    GUILayout.BeginVertical();
                    GUILayout.Label("IP");
                    foreach (var server in serverInfos)    
                    {
                        GUILayout.Label(server.ipAddress);
                    }
                    GUILayout.EndVertical();
                    
                    GUILayout.BeginVertical();
                    GUILayout.Label("-");
                    foreach (var server in serverInfos)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("JOIN"))
                        {
                            manager.networkAddress = Equals(GetComponent<scr_NetworkManager>().publicIp, server.ipAddress) ? "localhost" : server.ipAddress;
                            manager.StartClient();
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                    
                    GUILayout.EndArea();
                }
            }
            else
            {
                GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
                // Connecting
                GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
                GUILayout.EndArea();
            }
        }
        else
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
            // server / client status message
            if (NetworkServer.active)
            {
                GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
            }
            if (NetworkClient.isConnected)
            {
                GUILayout.Label("Client: address=" + manager.networkAddress);
            }
            GUILayout.EndArea();
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);

                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer();
                }
            }
            GUILayout.EndArea();
        }

        // stop
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
            if (GUILayout.Button("Stop"))
            {
                manager.StopHost();
            }
            GUILayout.EndArea();
        }
    }
    
    List<ServerInfo> RequestServersFromMatchmaker() 
    {
        print("Requesting servers");
        IPAddress ipAddr = IPAddress.Parse("79.129.139.225");
        if (Equals(GetComponent<scr_NetworkManager>().publicIp, "79.129.139.225\n")) // if the matchmaking server is local
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipHost.AddressList[1];
        }
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 7002); 

        // Creation TCP/IP Socket using  
        // Socket Class Costructor 
        Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Connect Socket to the remote  
        // endpoint using method Connect()  
        print(ipAddr.ToString());
        sender.Connect(localEndPoint);

        // We print EndPoint information  
        // that we are connected 
        Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

        // Creation of message that     
        // we will send to Server 
        byte[] messageSent = Encoding.ASCII.GetBytes("<EOF>");
        int byteSent = sender.Send(messageSent);

        
        // Data buffer 
        byte[] messageReceived = new byte[4096];
        string data = null;
        while (true)
        {
            int numByte = sender.Receive(messageReceived);
            data += Encoding.ASCII.GetString(messageReceived, 0, numByte);
            if (data.IndexOf("OVER") > -1) 
                break; 
        }
        
        List<ServerInfo> si = new List<ServerInfo>();
        String[] strings = data.Split('|');
        print("SERVERS:");
        foreach (var s in strings)
        {
            print(s);
            if (s == "OVER")
                break;
            si.Add(new ServerInfo(s));
        }

        // Close Socket using  
        // the method Close() 
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();

        return si;
    } 
}
