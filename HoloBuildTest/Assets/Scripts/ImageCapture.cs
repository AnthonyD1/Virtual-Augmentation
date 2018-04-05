using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Required for capturing images
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class ImageCapture : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    private Resolution cameraResolution;
    private CameraParameters cameraParameters;

    private bool photoCaputreModeOn = false;

	// Use this for initialization
	void Start () {
        Debug.Log("Photo capture script started");

        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            cameraParameters = new CameraParameters();

            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
        });

        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);

        Debug.Log("Photo capture script Start() completed");
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
        photoCaputreModeOn = true;
        Debug.Log("Photo capture mode is on");
    }

    void Caputre() {
        if (photoCaputreModeOn) {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            Debug.Log("Photo caputre async job started");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        //https://answers.unity.com/questions/42843/referencing-non-static-variables-from-another-scri.html

        //Convert the raw image capture into a texture (required by unity for some reason)
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        //Convert into jpeg data for sending over the network
        byte[] jpegData = targetTexture.EncodeToJPG();

        //Get to the HTTPImageXfer script
        GameObject holoLensCamera = GameObject.Find("HoloLensCamera");
        HTTPImageXfer hTTPImageXfer = holoLensCamera.GetComponent<HTTPImageXfer>();

        //Send the captured image as a Texture2D over to the TCPImageSend script for processing
        hTTPImageXfer.PostJpeg(jpegData);
    }
	
	// Update is called once per frame
	void Update () {
        //May want to make it not do it every single frame
        Caputre();
	}
}
