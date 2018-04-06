using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Required for capturing images
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

using System.Threading;

public class ImageCapture : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    private Resolution cameraResolution;
    private CameraParameters cameraParameters;

    private bool photoCaptureObjectCreated = false;
    private bool cameraSetupCompleted = false;
    private bool photoCaptureModeOn = false;
    private bool cameraBusy = false;
    private bool networkBusy = true;

	// Use this for initialization
	void Start () {
        Debug.Log("Photo capture script started");

        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log("ImageCapture.Start: Resolution set: " + cameraResolution.ToString());

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        Debug.Log("ImageCapture.Start: targetTexture created");

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
        
        PhotoCapture.CreateAsync(false, CameraSetup);
        //CameraSetup(photoCaptureObject);
        Debug.Log("ImageCapture.Start: Camera setup async started");

        Debug.Log("ImageCapture.Start: Start() completed");
    }

    void CameraSetup(PhotoCapture captureObject) {
        photoCaptureObject = captureObject;
        Debug.Log("CameraSetup: photoCaptureObject defined");

        cameraParameters = new CameraParameters();
        Debug.Log("CameraSetup: cameraParameters set up correctly");

        cameraParameters.hologramOpacity = 0.0f;
        Debug.Log("ImageCapture.CameraSetup: hologramOpacity set");
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        Debug.Log("ImageCapture.CameraSetup: Camera resolution (width) set");
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        Debug.Log("ImageCapture.CameraSetup: Camera resolution (height) set");
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
        Debug.Log("ImageCapture.CameraSetup: pixelFormat set");

        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
        Debug.Log("ImageCapture.CameraSetup: StartPhotoMode async started");

        Debug.Log("ImageCapture.CameraSetup: Async camera setup completed");

        cameraSetupCompleted = true;
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
        photoCaptureModeOn = true;
        Debug.Log("ImageCapture.OnPhotoModeStarted: Photo capture mode is on");
    }

    void Capture() {
        //TODO: Theoretically we can start another capture while the network is still busy from a previous capture
        if (photoCaptureObjectCreated && cameraSetupCompleted && photoCaptureModeOn && !cameraBusy && !networkBusy) {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            Debug.Log("ImageCapture.Capture: Ready to capture; async capture job started");
            cameraBusy = true;
        } else {
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
            //TODO: Possibly remove networkBusy; see above
            if (networkBusy) {
                Debug.Log("ImageCapture.Capture: Could not start capture because the network is still busy from a previous call (networkBusy)");
            }
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        //https://answers.unity.com/questions/42843/referencing-non-static-variables-from-another-scri.html

        //Notify other functions that we are done with the camera
        cameraBusy = false;

        //Notify other functions that we are making a network request
        networkBusy = true;

        //Convert the raw image capture into a texture (required by unity for some reason)
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        //Convert into jpeg data for sending over the network
        byte[] jpegData = targetTexture.EncodeToJPG();
        Debug.Log("ImageCapture.OnCapturedPhotoToMemory: jpeg data is " + jpegData.ToString());

        //Get to the HTTPImageXfer script
        GameObject holoLensCamera = GameObject.Find("HoloLensCamera");
        HTTPImageXfer hTTPImageXfer = holoLensCamera.GetComponent<HTTPImageXfer>();

        //Send the captured image as a Texture2D over to the TCPImageSend script for processing
        hTTPImageXfer.PostJpeg(jpegData);

        //Notify other functions that we are done with the network
        networkBusy = false;
    }


    //TODO: Change this to use WaitForSeconds from the unity API once we're sure this works
    private int frameCount = 0;
    
	void Update () {
        if (frameCount >= 150) {
            frameCount = 0;
            Capture();
        }
	}
}
