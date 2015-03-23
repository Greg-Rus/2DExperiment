using UnityEngine;
using System.Collections;

public class CamaraResize : MonoBehaviour {

	private Camera mainCam;
	// Use this for initialization
	void Awake () {
		mainCam = GetComponent<Camera>();
		float height = Screen.height;
		float width = Screen.width;
		mainCam.aspect = width / height;
	}
	
}
