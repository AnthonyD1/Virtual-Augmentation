//From https://foxypanda.me/tcp-client-in-a-uwp-unity-app-on-hololens/#thecode

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

public class USTrackingTcpClient : MonoBehaviour {

    public USTrackingManager TrackingManager;
    public USStatusTextManager StatusTextManager;

    private bool _useUWP = true;
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task exchangeTask;

    private Byte[] bytes = new Byte[256];
    private StreamWriter writer;
    private StreamReader reader;

    public void Connect(string host, string port) {
        ConnectUWP(host, port);
    }

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

    private bool exchanging = false;
    private bool exchangeStopRequested = false;
    private string lastPacket = null;

    private string errorStatus = null;
    private string warningStatus = null;
    private string successStatus = null;
    private string unknownStatus = null;

    public void RestartExchange() {
        if (exchangeTask != null) StopExchange();
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ExchangePackets());
    }

    public void Update() {
        if (lastPacket != null) {
            ReportDataToTrackingManager(lastPacket);
        }

        if (errorStatus != null) {
            StatusTextManager.SetError(errorStatus);
            errorStatus = null;
        }

        if (warningStatus != null) {
            StatusTextManager.SetWarning(warningStatus);
            warningStatus = null;
        }

        if (successStatus != null) {
            StatusTextManager.SetSuccess(successStatus);
            successStatus = null;
        }

        if (unknownStatus != null) {
            StatusTextManager.SetUnknown(unknownStatus);
            unknownStatus = null;
        }
    }

    public void ExchangePackets() {
        while (!exchangeStopRequested) {
            if (writer == null || reader == null) continue;
            exchanging = true;

            writer.Write("X\n");
            Debug.Log("Sent data!");
            string received = null;

            received = reader.ReadLine();

            lastPacket = received;
            Debug.Log("Read data: " + received);

            exchanging = false;
        }
    }

    private void ReportDataToTrackingManager(string data) {
        if (data == null) {
            Debug.Log("Received a frame but data was null");
            return;
        }

        var parts = data.Split(';');
        foreach (var part in parts) {
            ReportStringToTrackingManager(part);
        }
    }

    private void ReportStringToTrackingManager(string rigidBodyString) {
        var parts = rigidBodyString.Split(':');
        var positionData = parts[1].Split(',');
        var rotationData = parts[2].Split(',');

        int id = Int32.Parse(parts[0]);
        float x = float.Parse(positionData[0]);
        float y = float.Parse(positionData[1]);
        float z = float.Parse(positionData[2]);
        float qx = float.Parse(rotationData[0]);
        float qy = float.Parse(rotationData[1]);
        float qz = float.Parse(rotationData[2]);
        float qw = float.Parse(rotationData[3]);

        Vector3 position = new Vector3(x, y, z);
        Quaternion rotation = new Quaternion(qx, qy, qz, qw);

        TrackingManager.UpdateRigidBodyData(id, position, rotation);
    }

    public void StopExchange() {
        exchangeStopRequested = true;

        if (exchangeTask != null) {
            exchangeTask.Wait();
            socket.Dispose();
            writer.Dispose();
            reader.Dispose();

            socket = null;
            exchangeTask = null;
        }

        writer = null;
        reader = null;
    }

    public void OnDestroy() {
        StopExchange();
    }
}