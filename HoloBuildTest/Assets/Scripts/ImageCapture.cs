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

	// Use this for initialization
	void Start () {
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
	}

    void Caputre() {
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();

            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
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
        hTTPImageXfer.Post(jpegData);
    }
	
	// Update is called once per frame
	void Update () {
        //May want to make it not do it every single frame
        this.Caputre();
	}
}
