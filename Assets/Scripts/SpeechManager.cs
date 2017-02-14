using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA.Input;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpeechManager : MonoBehaviour {

    GestureRecognizer gestureRecognizer;
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords;

	public GameObject myCamera;
	public AudioSource audioSource;

    void OnDestroy()
    {
        gestureRecognizer.StopCapturingGestures();
        gestureRecognizer.TappedEvent -= GestureRecognizer_OnTappedEvent;
        gestureRecognizer.Dispose();

        keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Dispose();
    }

    void Start () {

        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.TappedEvent += GestureRecognizer_OnTappedEvent;
        gestureRecognizer.StartCapturingGestures();

        string[] scanCommands = { "scan", "scan this", "scan now", "scan again", "what is this", "help", "search", "capture now", "take a photo", "take photo", "take photo this", "take photo now" };
		string[] resetCommands = { "reset", "clear", "restart", "close", "exit", "try again" };
		string[] buyCommands = { "buy", "buy this", "buy now", "purchase", "purchase this", "purchase now", "add to cart", "place an order" };
		keywords = new Dictionary<string, System.Action>();
		foreach(string command in scanCommands)
        {
            keywords.Add(command, () =>
            {
				myCamera.SendMessage("OnReady");
				audioSource.SendMessage("PlayShutter");
				this.BroadcastMessage("OnCapture");
            });
        }
		foreach(string command in resetCommands)
		{
			keywords.Add(command, () =>
			{
				myCamera.SendMessage("OnReady");
			});
		}
		foreach(string command in buyCommands)
		{
			keywords.Add(command, () =>
			{
				myCamera.SendMessage("OnBuyNow");
			});
		}

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
	}

    private void GestureRecognizer_OnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
//		return;
//        Debug.Log("[ GestureRecognizer_OnTappedEvent ]");
//		this.BroadcastMessage("OnCapture");
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("[ KeywordRecognizer_OnPhraseRecognized ] " + args.text);

        System.Action keywordAction;
        if(keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}