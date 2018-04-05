using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Needed for networking
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
#if !UNITY_EDITOR
    using Windows.Networking.Sockets.StreamSocket;
#endif

public class TCPImageSend : MonoBehaviour {

    //The texture to convert to an image and send
    public Texture2D texture;

    //The IP address of the server to connect to in human-readable form
    public string server = "127.0.0.1";

    //The TCP port on the server to connect to
    public int port = 55555;

    //Internal use
    private IPEndPoint ipEndpoint;
    #if UNITY_EDITOR
        private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    #else
        private Windows.Networking.Sockets.StreamSocket sock = new Windows.Networking.Sockets.StreamSocket();
    #endif

    // Use this for initialization
    void Start () {
        ipEndpoint = new IPEndPoint(IPAddress.Parse(server), port);

        try {
            sock.Connect(ipEndpoint);
        } catch (SocketException e) {
            Debug.Log(e.ToString());
        }
    }

    void Update () {
        //TODO: We can get the return code of this function and use it to check for errors
        SendVarData(sock, texture.EncodeToJPG());
	}

    #if UNITY_EDITOR
        //Taken from https://answers.unity.com/questions/230269/send-rendertexture-via-tcp-or-udp.html
        private static int SendVarData(Socket s, byte[] data) {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[0];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);


            while (total < size) {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }

            return total;
        }
    #else
        private async void ConnectUWP(string host, string port) {
            try {
                if (exchangeTask != null) StopExchange();
        
                socket = new Windows.Networking.Sockets.StreamSocket();
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);
                await socket.ConnectAsync(serverHost, port);

                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                writer = new StreamWriter(streamOut) { AutoFlush = true };

                Stream streamIn = socket.InputStream.AsStreamForRead();
                reader = new StreamReader(streamIn);

                RestartExchange();
                successStatus = "Connected!";
            } catch (Exception e) {
                errorStatus = e.ToString();
            }
        }
    #endif
}