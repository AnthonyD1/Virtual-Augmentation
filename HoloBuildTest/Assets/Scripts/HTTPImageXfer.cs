#define ENABLE_POST_JPEG
//#define ENABLE_GET_TEST
#define HTTPIMAGEXFER_DEBUG

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
    private bool postJpegUnityWebRequestObjectIsActive = false;
#endif

    private void Start() {
        TextChangeObject = GameObject.Find("HoloLensCamera").GetComponent<TextChange>();
#if ENABLE_GET_TEST
        getTestUnityWebRequestObject = UnityWebRequest.Get("http://icanhazip.com");
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
        if(postJpegUnityWebRequestObjectIsActive && postJpegUnityWebRequestObject.downloadHandler.isDone) {
            PostJpegCallback();
        }
#endif
    }

#if ENABLE_GET_TEST
    void GetTest() {
        getTestUnityWebRequestObject.SendWebRequest();
    }

    void GetTestCallback() {
        TextChangeObject.SetTextValue(getTestUnityWebRequestObject.downloadHandler.text);
        Debug.Log("HTTPImageXfer.GetTestCallback: Got content of " + getTestUnityWebRequestObject.downloadHandler.text);
    }
#endif

    void PostText(string textData) {
#if HTTPIMAGEXFER_DEBUG
        Debug.Log("HTTPImageXfer.PostText: PostText function called");
#endif

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, textData);

#if HTTPIMAGEXFER_DEBUG
        Debug.Log("HTTPImageXfer.PostText: UnityWebRequest created");
#endif

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

        postJpegUnityWebRequestObject = UnityWebRequest.Post("http://" + serverAddress + serverPath, "FILE:" + System.Convert.ToBase64String(jpegData));
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest created");

        postJpegUnityWebRequestObjectIsActive = true;

        postJpegUnityWebRequestObject.SendWebRequest();
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest completed");
    }

    void PostJpegCallback() {
        TextChangeObject.SetTextValue(postJpegUnityWebRequestObject.downloadHandler.text);
        Debug.Log("HTTPImageXfer.PostJpegCallback: Got content of " + postJpegUnityWebRequestObject.downloadHandler.text);

        postJpegUnityWebRequestObjectIsActive = false;
        postJpegUnityWebRequestObject.Dispose();
    }
#else
    public void PostJpeg(byte[] jpegData) {
        //This is a placeholder to allow me to disable postJpeg without modifying the rest of the program.
    }
#endif
}
