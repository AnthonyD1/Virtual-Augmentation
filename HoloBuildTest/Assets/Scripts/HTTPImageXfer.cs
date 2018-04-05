using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPImageXfer : MonoBehaviour {
    public string serverAddress = "127.0.0.1";
    public int serverPort = 80;

	// Use this for initialization
	void Start () {
        //Nothing
	}
	
	// Update is called once per frame
	void Update () {
		//Nothing
	}

    public IEnumerator Post(byte[] jpegData) {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        formData.Add(new MultipartFormDataSection(jpegData));

        UnityWebRequest www = UnityWebRequest.Post("http://" + serverAddress + serverPort.ToString(), formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log("Form upload complete!");
        }
    }
}
