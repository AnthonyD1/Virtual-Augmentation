﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPImageXfer : MonoBehaviour {
    public string serverAddress;
    public string serverPath;

    void PostText(string textData) {
        Debug.Log("HTTPImageXfer.PostText: PostText function called");

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, textData);
        Debug.Log("HTTPImageXfer.PostText: UnityWebRequest created");

        www.SendWebRequest();
        Debug.Log("HTTPImageXfer.PostText: Send web request completed");

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log("HTTPImageXfer.PostText: www-error: " + www.error);
        } else {
            Debug.Log("HTTPImageXfer.PostText: POST completed");
        }
    }

    public void PostJpeg(byte[] jpegData) {
        Debug.Log("HTTPImageXfer.PostJpeg: PostJpeg called");

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, "FILE:" + System.Convert.ToBase64String(jpegData));
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest created");

        www.SendWebRequest();
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest completed");

        if (www.isNetworkError) {
            Debug.Log("HTTPImageXfer.PostJpeg: Network error: " + www.error);
        } else if(www.isHttpError) {
            Debug.Log("HTTPImageXfer.PostJpeg: HTTP error: " + www.error + ". Status code is " + www.responseCode);
        } else {
            Debug.Log("HTTPImageXfer.PostJpeg: Form upload complete!");
        }
    } 
}
