//Activate this to print gratuitous debug messages
//#define IMAGECAPTURE_DEBUG

/*
using UnityEngine;

//Required for capturing images
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class ImageCapture : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    private Resolution cameraResolution;
    private CameraParameters cameraParameters;

    private bool photoCaptureObjectCreated = false;
    private bool cameraSetupCompleted = false;
    private bool photoCaptureModeOn = false;
    private bool cameraBusy = false;
    private bool networkBusy = false;

    private GameObject holoLensCamera;
    private HTTPImageXfer hTTPImageXfer;

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
        
        
        PhotoCapture.CreateAsync(false, CameraSetup);
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: Camera setup async started");
#endif

        //Get the HTTPImageXfer object ready
        holoLensCamera = GameObject.Find("HoloLensCamera");
        hTTPImageXfer = holoLensCamera.GetComponent<HTTPImageXfer>();
        
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.Start: HTTPImageXfer object created");
        Debug.Log("ImageCapture.Start: Start() completed");
#endif
    }

    void CameraSetup(PhotoCapture captureObject) {
        photoCaptureObject = captureObject;

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: photoCaptureObject defined");
#endif

        cameraParameters = new CameraParameters();
        
#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: cameraParameters set up correctly");
#endif

        cameraParameters.hologramOpacity = 0.0f;

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: hologramOpacity set");
#endif

        cameraParameters.cameraResolutionWidth = cameraResolution.width;

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: Camera resolution (width) set");
#endif

        cameraParameters.cameraResolutionHeight = cameraResolution.height;

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: Camera resolution (height) set");
#endif

        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: pixelFormat set");
#endif

        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.CameraSetup: StartPhotoMode async started");
        Debug.Log("ImageCapture.CameraSetup: Async camera setup completed");
#endif

        cameraSetupCompleted = true;
        photoCaptureObjectCreated = true;
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
        photoCaptureModeOn = true;

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.OnPhotoModeStarted: Photo capture mode is on");
#endif
    }

    void Capture() {
        //TODO: Theoretically we can start another capture while the network is still busy from a previous capture
        if (photoCaptureObjectCreated && cameraSetupCompleted && photoCaptureModeOn && !cameraBusy && !networkBusy) {
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
            //TODO: Possibly remove networkBusy; see above
            if (networkBusy) {
                Debug.Log("ImageCapture.Capture: Could not start capture because the network is still busy from a previous call (networkBusy)");
            }
        }
#endif
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        //https://answers.unity.com/questions/42843/referencing-non-static-variables-from-another-scri.html

        //Notify other functions that we are done with the camera
        cameraBusy = false;

        //Notify other functions that we are making a network request
        networkBusy = true;

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
        hTTPImageXfer.PostJpeg(jpegData);
        //PostJpeg(jpegData);

#if IMAGECAPTURE_DEBUG
        Debug.Log("ImageCapture.OnCapturedPhotoToMemory: Called HTTPImageXfer");
#endif

        //Notify other functions that we are done with the network
        networkBusy = false;
    }


    //TODO: Change this to use WaitForSeconds from the unity API once we're sure this works
    private int frameCount = 0;
    
	void Update () {
        
        if (frameCount >= 300) {
            frameCount = 0;

#if IMAGECAPTURE_DEBUG
            Debug.Log("ImageCapture.Update: Attempting to capture...");
#endif

            Capture();
        } else {
            frameCount++;
        }
	}
}
*/