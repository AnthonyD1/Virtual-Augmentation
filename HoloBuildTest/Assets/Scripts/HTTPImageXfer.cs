using UnityEngine;
using UnityEngine.Networking;

public class HTTPImageXfer : MonoBehaviour {
    public string serverAddress;
    public string serverPath;

    private TextChange TextChangeObject;
    private UnityWebRequest unityWebRequestObject;
    private int frameCount = 0;

    private void Start() {
        TextChangeObject = GameObject.Find("HoloLensCamera").GetComponent<TextChange>();
        unityWebRequestObject = UnityWebRequest.Get("http://" + serverAddress + serverPath);
        GetRequest();
    }

    private void Update() {
        if(frameCount >= 150) {
            GetRequest();
            frameCount = 0;
        }
        frameCount++;

        if(unityWebRequestObject.downloadHandler.isDone) {
            GetCallback();
        }
    }

    void GetRequest() {
        unityWebRequestObject.SendWebRequest();
    }

    void GetCallback() {
        TextChangeObject.SetTextValue(unityWebRequestObject.downloadHandler.text);
        Debug.Log("HTTPImageXfer.GetTestCallback: Got content of " + unityWebRequestObject.downloadHandler.text);
    }
}
