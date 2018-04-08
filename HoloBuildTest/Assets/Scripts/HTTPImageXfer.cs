using System.Collections;
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

    // Use this for initialization
    void Start () {
        Debug.Log("HTTPImageXfer.Start: HTTPImageXfer Started");
        PostText("Microsoft sucks");
        PostText("Anthony is smart");
        PostText("Third");
        Debug.Log("HTTPImageXfer.Start: Post text function completed");
	}

    public /*IEnumerator*/ void PostJpeg(byte[] jpegData) {
        Debug.Log("HTTPImageXfer.PostJpeg: PostJpeg called");
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        //formData.Add(new MultipartFormDataSection("file", jpegData));
        Debug.Log("HTTPImageXfer.PostJpeg: FormData object created successfully");

        //UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, formData);
        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, "FILE:" + System.Convert.ToBase64String(jpegData));
        Debug.Log("HTTPImageXfer.PostJpeg: UnityWebRequest created");

        /*yield return*/ www.SendWebRequest();
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
