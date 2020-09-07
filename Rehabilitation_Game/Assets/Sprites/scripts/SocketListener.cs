﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class SocketListener : MonoBehaviour
{
    private TcpClient socketConnection;
    private Thread clientReceiveThread = null;
    [SerializeField]
    private GameHandler gameManager;
    [SerializeField]
    private Throw Throw;
    bool shotFlag = false;
  
    private bool connected = false;
    // Start is called before the first frame update
    void Start()
    {
        ConnectToTcpServer();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ConnectToTcpServer()
    {
        try
        {
            if(clientReceiveThread != null)
                clientReceiveThread.Abort();
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("127.0.0.1", 12345);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 		
                if (socketConnection.Connected)
                {
                    gameManager.ConncetedToServer();
                    connected = true;
                    Debug.Log("connected to Server");
                }

                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        var yprRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?\/[0-9]*(?:\.[0-9]*)?\/[0-9]*(?:\.[0-9]*)?\/(0|1)$");
                        if (!yprRegex.IsMatch(serverMessage))
                        {
                            Debug.Log(serverMessage);
                            continue;
                        }
                        string[] yprs = serverMessage.Split('/');
                        float yaw = float.Parse(yprs[0]);
                        float pitch = float.Parse(yprs[1]);
                        float roll = float.Parse(yprs[2]);
                        if (shotFlag == true && !yprs[3].Equals("1"))
                            gameManager.Shot();
                        shotFlag = yprs[3].Equals("1");
                        gameManager.SetServerAngle(yaw);
                    }
                }
            }
        }
        catch (Exception socketException)
        {
            gameManager.DisConnectedFromServer();
            Debug.Log("disconnected from Server");
        }
    }

    private void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}
