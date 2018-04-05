//https://answers.unity.com/questions/230269/send-rendertexture-via-tcp-or-udp.html


using UnityEngine;
using System.Collections;
using System;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

public class TCP_ImageSend : MonoBehaviour
{

    public RenderTexture sendRenderTextureMain;
    private Texture2D sendImage2DMain;

    byte[] data = new byte[0];
    int sent;
    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    void Start()
    {
        sendImage2DMain = new Texture2D(sendRenderTextureMain.width, sendRenderTextureMain.height, TextureFormat.RGB24, false);

        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }
    void Update()
    {
        RenderTexture.active = sendRenderTextureMain;
        sendImage2DMain.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
        sendImage2DMain.Apply();

        sent = SendVarData(server, sendImage2DMain.EncodeToPNG());
    }

    private static int SendVarData(Socket s, byte[] data)
    {
        int total = 0;
        int size = data.Length;
        int dataleft = size;
        int sent;

        byte[] datasize = new byte[0];
        datasize = BitConverter.GetBytes(size);
        sent = s.Send(datasize);

        while (total < size)
        {
            sent = s.Send(data, total, dataleft, SocketFlags.None);
            total += sent;
            dataleft -= sent;
        }

        return total;
    }    

    
