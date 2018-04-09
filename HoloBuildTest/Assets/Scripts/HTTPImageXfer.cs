#define ENABLE_POST_JPEG
//#define ENABLE_GET_TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPImageXfer : MonoBehaviour {
    public string serverAddress;
    public string serverPath;

    private TextChange TextChangeObject;

#if ENABLE_GET_TEST
    private UnityWebRequest getTestUnityWebRequestObject;
#endif
#if ENABLE_POST_JPEG
    private UnityWebRequest postJpegUnityWebRequestObject;
#endif

    private void Start() {
        TextChangeObject = GameObject.Find("HoloLensCamera").GetComponent<TextChange>();
#if ENABLE_GET_TEST
        GetTest();
#endif
    }

    private void Update() {
#if ENABLE_GET_TEST
        if(getTestUnityWebRequestObject.downloadHandler.isDone) {
            GetTestCallback();
        }
#endif

#if ENABLE_POST_JPEG
        if(postJpegUnityWebRequestObject.downloadHandler.isDone) {
            PostJpegCallback();
        }
#endif
    }

#if ENABLE_GET_TEST
    void GetTest() {
        getTestUnityWebRequestObject = UnityWebRequest.Get("http://icanhazip.com");
        getTestUnityWebRequestObject.SendWebRequest();
    }

    void GetTestCallback() {
        TextChangeObject.SetTextValue(getTestUnityWebRequestObject.downloadHandler.text);
        Debug.Log("HTTPImageXfer.GetTestCallback: Got content of " + getTestUnityWebRequestObject.downloadHandler.text);
    }
#endif

    void PostText(string textData) {
        Debug.Log("HTTPImageXfer.PostText: PostText function called");

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, textData);
        Debug.Log("HTTPImageXfer.PostText: UnityWebRequest created");

        //Set the HUD text to the response of the HTTP request
        TextChangeObject.SetTextValue(www.downloadHandler.text);

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log("HTTPImageXfer.PostText: www-error: " + www.error);
        } else {
            Debug.Log("HTTPImageXfer.PostText: POST completed");
        }
    }

#if ENABLE_POST_JPEG
    public void PostJpeg(byte[] jpegData) {
        Debug.Log("HTTPImageXfer.PostJpeg: PostJpeg called");

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, "FILE:" + System.Convert.ToBase64String(jpegData));
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest created");

        www.SendWebRequest();
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest completed");

        while (!www.downloadHandler.isDone) {
            Debug.Log("HTTPImageXfer.PostText: Waiting for response... " + www.downloadProgress);
        }

        Debug.Log("HTTPImageXfer.PostText: Web Request Response: " + www.downloadHandler.data.ToString());

        if (www.isNetworkError) {
            Debug.Log("HTTPImageXfer.PostJpeg: Network error: " + www.error);
        } else if(www.isHttpError) {
            Debug.Log("HTTPImageXfer.PostJpeg: HTTP error: " + www.error + ". Status code is " + www.responseCode);
        } else {
            Debug.Log("HTTPImageXfer.PostJpeg: Form upload complete!");
        }
    }

    void PostJpegCallback() {
        TextChangeObject.SetTextValue(postJpegUnityWebRequestObject.downloadHandler.text);
        Debug.Log("HTTPImageXfer.GetTestCallback: Got content of " + postJpegUnityWebRequestObject.downloadHandler.text);
    }
#else
    public void PostJpeg(byte[] jpegData) {
        //This is a placeholder to allow me to disable postJpeg without modifying the rest of the program.
    }
#endif
}
