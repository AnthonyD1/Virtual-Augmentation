using UnityEngine;
using UnityEngine.Networking;

using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Handle taking pictures, send them to the backend and process the results
/// to display annotations in the real world.
/// </summary>
public class AnnotationHandling : MonoBehaviour
{

    Resolution m_cameraResolution;
    UnityEngine.XR.WSA.WebCam.PhotoCapture m_photoCapture;

    public float m_pictureInterval;
    public LayerMask m_raycastLayer;
    public GameObject m_annotationParent;
    public GameObject m_annotationTemplate;

    /// <summary>
    /// Use the start function to start the picture capturing process
    /// </summary>
    void Start()
    {
        //Get the highest resolution
        m_cameraResolution = UnityEngine.XR.WSA.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(false, delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture captureObject)
        {
            //Assign capture object
            m_photoCapture = captureObject;

            //Configure camera
            UnityEngine.XR.WSA.WebCam.CameraParameters cameraParameters = new UnityEngine.XR.WSA.WebCam.CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = m_cameraResolution.width;
            cameraParameters.cameraResolutionHeight = m_cameraResolution.height;
            cameraParameters.pixelFormat = UnityEngine.XR.WSA.WebCam.CapturePixelFormat.JPEG;

            //Start the photo mode and start taking pictures
            m_photoCapture.StartPhotoModeAsync(cameraParameters, delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result)
            {
                Debug.Log("Photo Mode started");
                InvokeRepeating("ExecutePictureProcess", 0, m_pictureInterval);
            });
        });
    }

    /// <summary>
    /// Clean up on destroty
    /// </summary>
    void OnDestroy()
    {
        m_photoCapture.StopPhotoModeAsync(
          delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult res)
          {
              m_photoCapture.Dispose();
              m_photoCapture = null;
              Debug.Log("Photo Mode stopped");
          }
        );
    }

    /// <summary>
    /// Take a photo anbd start the backend handling
    /// </summary>
    void ExecutePictureProcess()
    {
        if (m_photoCapture != null)
        {
            //Take a picture
            m_photoCapture.TakePhotoAsync(delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result, UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame photoCaptureFrame)
            {
                List<byte> buffer = new List<byte>();

                photoCaptureFrame.CopyRawImageDataIntoBuffer(buffer);

                //Start a coroutine to handle the server request
                StartCoroutine(UploadAndHandlePhoto(buffer.ToArray()));
            });
        }
    }

    /// <summary>
    /// Create a request for the current selected platform
    /// </summary>
    UnityWebRequest CreateRequest(byte[] photo)
    {
        DownloadHandler download = new DownloadHandlerBuffer();

        List<IMultipartFormSection> multipartFormSections = new List<IMultipartFormSection>();
        multipartFormSections.Add(new MultipartFormFileSection("img", photo, "test.jpg", "image/jpeg"));
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        UploadHandler upload = new UploadHandlerRaw(UnityWebRequest.SerializeFormSections(multipartFormSections, boundary));
        string url = "http://10.200.1.183:8080/";
        UnityWebRequest www = new UnityWebRequest(url, "PUT", download, upload);
        //www.useHttpContinue = false;
        www.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary));
        return www;
    }

    /// <summary>
    /// Start the upload and pass the response to the handling function
    /// </summary>
    IEnumerator UploadAndHandlePhoto(byte[] photo)
    {
        using (UnityWebRequest www = CreateRequest(photo))
        {
            //Send the request to the clout
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Get JSON as string
                string jsonString = www.downloadHandler.text;

                //Remove all old annotations
                foreach (Transform child in m_annotationParent.transform)
                {
                    Destroy(child.gameObject);
                }

                JSONNode jsonResponse = JSON.Parse(jsonString);
                Debug.Log(jsonString);
            }
        }
    }
}
