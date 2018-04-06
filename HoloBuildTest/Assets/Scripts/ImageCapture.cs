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

    private bool photoCaptureModeOn = false;

	// Use this for initialization
	void Start () {
        Debug.Log("Photo capture script started");

        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log("ImageCapture.Start: Resolution set: " + cameraResolution.ToString());

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        Debug.Log("ImageCapture.Start: targetTexture created");

        photoCaptureObject = new PhotoCapture();

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
        });
        
        // Try the above without an inline async method
        //PhotoCapture.CreateAsync(false, CameraSetup);
        //CameraSetup(photoCaptureObject);

        //Debug.Log("ImageCapture.Start: Camera setup async started");

        Debug.Log("Photo capture script Start() completed");

        Application.Quit();

        while (!photoCaptureModeOn) {
            Debug.Log("ImageCapture.Start: Waiting for photo capture mode to be on...");
        }

        Capture();
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
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
        photoCaptureModeOn = true;
        Debug.Log("ImageCapture.OnPhotoModeStarted: Photo capture mode is on");
    }

    void Capture() {
        if (photoCaptureModeOn) {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            Debug.Log("Photo capture async job started");
            photoCaptureModeOn = false;
        } else {
            Debug.Log("Photo not captured because photo capture mode is not on");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        //https://answers.unity.com/questions/42843/referencing-non-static-variables-from-another-scri.html

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

        photoCaptureModeOn = true;
    }
	
	// Update is called once per frame
	void Update () {
        //May want to make it not do it every single frame
        //Don't caputre every frame, just for now
        //Capture();
	}
}
