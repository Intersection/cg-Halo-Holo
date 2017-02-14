using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterEffect : MonoBehaviour {

	void Awake() {
		//Debug.Log("[ ShutterEffect::Awake ]");
		//source = GetComponent<AudioSource>(); 
	}

	void PlayShutter() {
		Debug.Log("[ ShutterEffect::PlayShutter ]");
		this.gameObject.GetComponent<AudioSource> ().PlayOneShot(this.GetComponent<AudioSource>().clip);
	}
}
