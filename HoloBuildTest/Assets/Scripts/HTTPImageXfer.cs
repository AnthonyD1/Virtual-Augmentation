using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPImageXfer : MonoBehaviour {
    public string serverAddress;
    public string serverPath;

    void PostText(string textData) {
        Debug.Log("PostText function called");

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, textData);
        Debug.Log("UnityWebRequest created");

        www.SendWebRequest();
        Debug.Log("Send web request completed");

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log("www-error: " + www.error);
        } else {
            Debug.Log("POST completed");
        }
    }

    // Use this for initialization
    void Start () {
        Debug.Log("HTTPImageXfer Started");
        PostText("Microsoft sucks");
        Debug.Log("Post text function completed");
	}
	
	// Update is called once per frame
	void Update () {
		//Nothing
	}

    public void PostJpeg(byte[] jpegData) {
        Debug.Log("PostJpeg called");
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        formData.Add(new MultipartFormDataSection("file", jpegData));
        Debug.Log("PostJpeg: FormData object created successfully");

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPath, formData);
        www.SendWebRequest();
        Debug.Log("PostJpeg: UnityWebRequest completed");

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log("PostJpeg: www-error: " + www.error);
        } else {
            Debug.Log("PostJpeg: Form upload complete!");
        }
    }

    
}
