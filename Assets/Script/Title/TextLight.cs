using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextLight : MonoBehaviour {

	public float timeOut;
	private float timeElapsed;

	private float RedColor = 0.01f;
	//private float GreenColor = 0;
	//private float BlueColor = 0;
//	private float ColorMax = 256;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float fade = this.GetComponent<Text> ().color.r;
		timeElapsed += Time.deltaTime;
		if(timeElapsed >= timeOut) {
			// Do anything
			fade += RedColor;
			timeElapsed = 0.0f;
		}
		Debug.Log (fade);
		this.GetComponent<Text> ().color = new Color( fade, 1f, 1f, 255f);
	}
}
