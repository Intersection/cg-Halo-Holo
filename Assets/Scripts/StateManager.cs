using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

public class StateManager : MonoBehaviour {

	private Canvas processing, preview, buyNow, tryAgain, voiceHelp;
	public Text helpLabel;
	public TextToSpeechManager tts;

	void Start()
	{
		processing = GameObject.Find("Processing").GetComponent<Canvas>();
		preview = GameObject.Find("Preview").GetComponent<Canvas>();
		buyNow = GameObject.Find("BuyNow").GetComponent<Canvas>();
		tryAgain = GameObject.Find("TryAgain").GetComponent<Canvas>();
		voiceHelp = GameObject.Find("VoiceHelp").GetComponent<Canvas>();
			
		processing.enabled = false;
		preview.enabled = false;
		buyNow.enabled = false;
		tryAgain.enabled = false;
		helpLabel.text = "\"Scan Now\"";
		voiceHelp.enabled = true;
	}

	void OnReady()
	{
		StopCoroutine("GoReady");
		
		processing.enabled = false;
		preview.enabled = false;
		buyNow.enabled = false;
		tryAgain.enabled = false;
		helpLabel.text = "\"Scan Now\"";
		voiceHelp.enabled = true;
	}

	void OnCapture()
	{
		processing.enabled = true;
		preview.enabled = false;
		buyNow.enabled = false;
		tryAgain.enabled = false;
		voiceHelp.enabled = false;
	}

	void OnPreview(RecognizedObject recognizedObject)
	{
		processing.enabled = false;
		preview.SendMessage("Display", recognizedObject);

		if (tts != null) {
			tts.SpeakText(recognizedObject.name + " found.");
		}

		preview.enabled = true;
		buyNow.enabled = false;
		tryAgain.enabled = false;
		helpLabel.text = "\"Buy Now\"";
		voiceHelp.enabled = true;
	}

	void OnBuyNow()
	{
		if (!preview.enabled) {
			return;
		}
		processing.enabled = false;
		preview.enabled = false;
		buyNow.enabled = true;
		tryAgain.enabled = false;
		voiceHelp.enabled = false;

		if (tts != null) {
			tts.SpeakText("Your order has been place. thank you!");
		}

		StartCoroutine ("GoReady");
	}

	void OnError()
	{
		processing.enabled = false;
		preview.enabled = false;
		buyNow.enabled = false;
		tryAgain.enabled = true;
		voiceHelp.enabled = false;

		if (tts != null) {
			tts.SpeakText("try again");
		}

		StartCoroutine ("GoReady");
	}

	IEnumerator GoReady()
	{
		yield return new WaitForSeconds(3.0f);
		OnReady();
	}
}