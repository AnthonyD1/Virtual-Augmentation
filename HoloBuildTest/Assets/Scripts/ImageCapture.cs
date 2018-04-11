//Activate this to print gratuitous debug messages
#define IMAGECAPTURE_DEBUG

using UnityEngine;

//Required for capturing images
using System.Linq;
using UnityEngine.XR.WSA.WebCam;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;

public class ImageCapture : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    public string serverAddress;
    public string serverPath;

    private Resolution cameraResolution;
    private CameraParameters cameraParameters;

    private bool photoCaptureObjectCreated = false;
    private bool cameraSetupCompleted = false;
    private bool photoCaptureModeOn = false;
    private bool cameraBusy = false;

    private GameObject holoLensCamera;

    // Use this for initialization
    void Start () {
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: Photo capture script started");
#endif

        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: Resolution set: " + cameraResolution.ToString());
#endif

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height, TextureFormat.BGRA32, false);

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: targetTexture created");
#endif

        /*
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            Debug.Log("ImageCapture.Start: PhotoCapture.CreateAsync started");
            photoCaptureObject = captureObject;
            Debug.Log("ImageCapture.Start: photoCaptureObject = captureObject");
            cameraParameters = new CameraParameters();
            Debug.Log("ImageCapture.Start: Created cameraParameters");

            cameraParameters.hologramOpacity = 0.0f;
            Debug.Log("ImageCapture.Start: hologramOpacity set");

            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            Debug.Log("ImageCapture.Start: camera resolution width set");

            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            Debug.Log("ImageCapture.Start: camera resolution height set");

            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
            Debug.Log("ImageCapture.Start: camera pixel format set. Async camera setup complete.");

            photoCaptureObjectCreated = true;
        });
        */
        
        
        PhotoCapture.CreateAsync(false, CameraSetup);
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: Camera setup async started");
#endif

        //Get the HTTPImageXfer object ready
        holoLensCamera = GameObject.Find("HoloLensCamera");
        
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: HTTPImageXfer object created");
        Debug.Log("ImageCapture.Start: Start() completed");
#endif
    }

    void CameraSetup(PhotoCapture captureObject) {
        photoCaptureObject = captureObject;
        cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
        cameraSetupCompleted = true;
        photoCaptureObjectCreated = true;
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
        photoCaptureModeOn = true;
    }

    void Capture() {
        //TODO: Theoretically we can start another capture while the network is still busy from a previous capture
        if (photoCaptureObjectCreated && cameraSetupCompleted && photoCaptureModeOn && !cameraBusy) {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);

#if IMAGECAPTURE_DEBUG
            Debug.Log("ImageCapture.Capture: Ready to capture; async capture job started");
#endif

            cameraBusy = true;
        }
#if IMAGECAPTURE_DEBUG
        else {
            if (!photoCaptureObjectCreated) {
                Debug.Log("ImageCapture.Capture: Could not start capture because photoCaptureObject is not ready");
            }
            if (!cameraSetupCompleted) {
                Debug.Log("ImageCapture.Capture: Could not start capture because CameraSetup is not complete");
            }
            if (!photoCaptureModeOn) {
                Debug.Log("ImageCapture.Capture: Could not start capture because photo capture mode is not on");
            }
            if (cameraBusy) {
                Debug.Log("ImageCapture.Capture: Could not start capture because the camera is still capturing from a previous call (cameraBusy)");
            }
        }
#endif
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        //https://answers.unity.com/questions/42843/referencing-non-static-variables-from-another-scri.html

        //Notify other functions that we are done with the camera
        cameraBusy = false;

        //Convert the raw image capture into a texture (required by unity for some reason)
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.OnCapturedPhotoToMemory: Uploaded to texture");
#endif

        //Convert into jpeg data for sending over the network
        byte[] jpegData = ImageConversion.EncodeToJPG(targetTexture, 1);

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.OnCapturedPhotoToMemory: jpeg data is " + System.Convert.ToBase64String(jpegData));
#endif

        //Send the captured image as a Texture2D over to the TCPImageSend script for processing
        CreateRequest(jpegData);
        //PostJpeg(jpegData);

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.OnCapturedPhotoToMemory: Called CreateRequest");
#endif
    }


    //TODO: Change this to use WaitForSeconds from the unity API once we're sure this works
    private int frameCount = 0;
    
	void Update () {
        
        if (frameCount >= 150) {
            frameCount = 0;

#if IMAGECAPTURE_DEBUG
            Debug.Log("ImageCapture.Update: Attempting to capture...");
#endif

            Capture();
        } else {
            frameCount++;
        }
	}

    public UnityWebRequest CreateRequest(byte[] photo) {
        DownloadHandler download = new DownloadHandlerBuffer();

        List<IMultipartFormSection> multipartFormSections = new List<IMultipartFormSection>();
        multipartFormSections.Add(new MultipartFormFileSection("img", photo, "test.jpg", "image/jpeg"));
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        UploadHandler upload = new UploadHandlerRaw(UnityWebRequest.SerializeFormSections(multipartFormSections, boundary));
        string url = "http://" + serverAddress + serverPath;
        UnityWebRequest www = new UnityWebRequest(url, "POST", download, upload);
        www.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary));
        return www;
    }
}