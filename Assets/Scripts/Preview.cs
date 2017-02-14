using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour {

	public GameObject managers;
	public Sprite imgCorrectionFluid, imgPower, imgShellac, imgWemo, imgFig, imgLego, imgCrunch;
	public GameObject myCamera;

	private Canvas container;
	private Image previewImage;
	private Text price;
	private Text title;

	private bool isVisible;

	void Start () {
		container = this.gameObject.GetComponent<Canvas>();
		previewImage = GameObject.Find ("mImage").GetComponent<Image> ();
		price = GameObject.Find("Price").GetComponent<Text>();
		title = GameObject.Find("Title").GetComponent<Text>();

		container.enabled = false;
	}

	void OnReady()
	{
		container.enabled = false;
	}
	
	void Display(RecognizedObject recognizedObject)
	{
		if(recognizedObject != null)
		{
			Debug.Log("[ Display ] " + recognizedObject.name + " found ( " + recognizedObject.score + " )");

			string objName = recognizedObject.name;
			if (objName == "white out") {
				previewImage.sprite = imgCorrectionFluid;
				price.text = "$11.90";
				title.text = "BIC Multipurpose Correction Fluid";
			}
			else if(objName == "power") {
				previewImage.sprite = imgPower;
				price.text = "$100.00";
				title.text = "Apple OEM Original MacBook MagSafe 2 Power Charging Wall Charger Adapter";
			}
			else if(objName == "shellac") {
				previewImage.sprite = imgShellac;
				price.text = "$7.47";
				title.text = "Zinsser Bulls Eye Shellac Aerosol";
			}
			else if(objName == "wemo") {
				previewImage.sprite = imgWemo;
				price.text = "$47.99";
				title.text = "Belkin WeMo Switch Home Remote";
			}
			else if(objName == "lego") {
				previewImage.sprite = imgLego;
				price.text = "$14.99";
				title.text = "LEGO City Space Port Space Starter Set, 60077";
			}
			else if(objName == "fig") {
				previewImage.sprite = imgFig;
				price.text = "$1.39";
				title.text = "Nature's Bakery Apple Cinnamon Stone Ground Whole Wheat Fig Bar, 2oz";
			}
			else if(objName == "crunch") {
				previewImage.sprite = imgCrunch;
				price.text = "$2.98";
				title.text = "Cinnamon Toast Crunch Crispy Sweetened Whole Wheat and Rice Cereal 12.2oz Box";
			}
			else {
				price.text = "Not trained yet...";
				title.text = recognizedObject.name;
			}

			container.enabled = true;

			//StartCoroutine ("close");
		}
		else
		{
			Debug.Log("[ Preview::Display ] recognizedObject is null!");
		}
	}

	IEnumerator close() {
		yield return new WaitForSeconds(30.0f);
		if (container.enabled) {
			container.enabled = false;
			myCamera.SendMessage("OnReady");
		}
	}
}