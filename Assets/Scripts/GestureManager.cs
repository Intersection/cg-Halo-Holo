using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour {

    GestureRecognizer gestureRecognizer;

    void Start()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.TappedEvent += (source, tapCount, ray) =>
        {
            Debug.Log("TappedEvent!!!");
			this.BroadcastMessage("OnCapture");
        };
        gestureRecognizer.StartCapturingGestures();

        Debug.Log("[ GestureRecognizer ] Start Capturing Gestures... ");
    }
	
	void OnDestroy () {
        gestureRecognizer.StopCapturingGestures();
	}
}
