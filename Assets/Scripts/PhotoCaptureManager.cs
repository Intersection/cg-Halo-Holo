using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class PhotoCaptureManager : MonoBehaviour {

    //PhotoCapture photoCaptureObject = null;
    Resolution cameraResolution;

	public GameObject myCamera;
	public TextToSpeechManager tts;

    private string url;
	private PhotoCapture mPhotoCapture;
	private bool isCameraReady;

	void OnDestroy()
	{
		mPhotoCapture.StopPhotoModeAsync(onPhotoModeStopped);
	}

    void Start()
    { 
        List<Resolution> resolutions = new List<Resolution>(PhotoCapture.SupportedResolutions);
        cameraResolution = resolutions[0];

        url = "http://ec2-54-82-199-39.compute-1.amazonaws.com:5000/base64"; // compute-optimized. no-GPU

		initCamera();
    }

	void initCamera()
	{
		mPhotoCapture = null;
		isCameraReady = false;

		// Create PhotoCapture object
		PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject)
		{
				mPhotoCapture = captureObject;
				CameraParameters cameraParameters = new CameraParameters();
				cameraParameters.hologramOpacity = 0.0f;
				cameraParameters.cameraResolutionWidth = cameraResolution.width;
				cameraParameters.cameraResolutionHeight = cameraResolution.height;
				cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

				// Activate camera
				mPhotoCapture.StartPhotoModeAsync(cameraParameters, delegate(PhotoCapture.PhotoCaptureResult result)
					{
						if(result.success)
						{
							isCameraReady = true;
						}
						else
						{
							Debug.LogError("failed to StartPhotoModeAsync");
							mPhotoCapture.StopPhotoModeAsync(onPhotoModeStopped);
						}
					});
			});
	}

    void OnCapture()
    {
		if (!isCameraReady) {
			Debug.LogError("Camera is not ready!");
			return;
		}

		// Take a picture
		mPhotoCapture.TakePhotoAsync(onCapturedPhotoToDisk);
    }

    private void onCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if(result.resultType == PhotoCapture.CaptureResultType.Success)
        {
			myCamera.SendMessage("OnCapture");
            Texture2D tex = new Texture2D(cameraResolution.width, cameraResolution.height, TextureFormat.RGB24, false);
            photoCaptureFrame.UploadImageDataToTexture(tex);
            StartCoroutine(postImage(tex));
        }

        // Deactivate camera
        //photoCaptureObject.StopPhotoModeAsync(onPhotoModeStopped);
    }

    private void onPhotoModeStopped(PhotoCapture.PhotoCaptureResult result)
    {
		mPhotoCapture.Dispose();
		mPhotoCapture = null;
    }

    public IEnumerator postImage(Texture2D tex)
    {
		if (tts != null) {
			tts.SpeakText("Analyzing..");
		}

        byte[] bytes = tex.EncodeToJPG();
        var encoding = new System.Text.UTF8Encoding();

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/jpeg");

        WWW www = new WWW(url, encoding.GetBytes(System.Convert.ToBase64String(bytes)), headers);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            // {"score": "0.630731", "name": "shellac"}
            Debug.Log(www.text);
            RecognizedObject recognizedObject = RecognizedObject.CreateFromJSON(www.text);

			// score
			if (float.Parse (recognizedObject.score) < 0.85) {
				// try again
				//managers.SendMessage("OnCapture");
				Debug.Log ("Try again....");
				myCamera.SendMessage ("OnError");
			} else {
				myCamera.SendMessage("OnPreview", recognizedObject);
			}
        }
        else
        {
            Debug.LogError("[errorrrrr] " + www.error);
			myCamera.SendMessage ("OnError");
        }

        Destroy(tex);
    }
}