using System;
using System.Collections.Generic;
using System.Net; 
using System.Net.Sockets; 
using System.Text;

class ServerInfo
{
    public string name;
    public string ipAddress;

    public ServerInfo(string data)
    {
        String[] strings = data.Split('-');
        name = strings[0];
        ipAddress = strings[1];
        Console.WriteLine(name + ipAddress);
    }

    public override string ToString()
    {
        return name + "-" + ipAddress;
    }
}

class Program 
{
    public static List<ServerInfo> serverInfos = new List<ServerInfo>();
  
    // Main Method 
    public static void Main(string[] args) 
    { 
        Console.WriteLine("Hello World!");
        ExecuteServer(); 
    } 
  
    public static void ExecuteServer() 
    { 
        // Establish the local endpoint  
        // for the socket. Dns.GetHostName 
        // returns the name of the host  
        // running the application. 
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName()); 
        IPAddress ipAddr = ipHost.AddressList[1];
        foreach (var addr in ipHost.AddressList)
        {
            Console.WriteLine(addr.ToString());
        }
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 7002); 
      
        // Creation TCP/IP Socket using  
        // Socket Class Costructor 
        Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
      
        try 
        {
            // Using Bind() method we associate a 
            // network address to the Server Socket 
            // All client that will connect to this  
            // Server Socket must know this network 
            // Address 
            listener.Bind(localEndPoint); 
      
            // Using Listen() method we create  
            // the Client list that will want 
            // to connect to Server 
            listener.Listen(10); 
      
            while (true) { 
                  
                Console.WriteLine("Waiting connection ... "); 
      
                // Suspend while waiting for 
                // incoming connection Using  
                // Accept() method the server  
                // will accept connection of client 
                Socket clientSocket = listener.Accept();
                
                // Data buffer 
                byte[] bytes = new byte[1024]; 
                string data = null;
                bool requestForInfo = false;
                while (true) 
                {
                    int numByte = clientSocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numByte);
                    if (data == "<EOF>")
                        requestForInfo = true;
                    if (data.IndexOf("<EOF>") > -1) 
                        break; 
                }
                
                if (!requestForInfo)
                    serverInfos.Add(new ServerInfo(data));
                
                Console.WriteLine("Text received -> {0} ", data);

                byte[] message;
                foreach (var serverInfo in serverInfos)
                {
                    message = Encoding.ASCII.GetBytes(serverInfo.ToString() + "|");
                    clientSocket.Send(message); 
                }
                message = Encoding.ASCII.GetBytes("OVER"); 
                clientSocket.Send(message); 
                Console.WriteLine("OVER sent");

                // Close client Socket using the 
                // Close() method. After closing, 
                // we can use the closed Socket  
                // for a new Client Connection 
                clientSocket.Shutdown(SocketShutdown.Both); 
                clientSocket.Close(); 
            } 
        }
        catch (Exception e) 
        { 
            Console.WriteLine(e.ToString()); 
        } 
    } 
} 

