using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountDawn : MonoBehaviour {

	private Text CountDownText;

	private int countDown = 3;
	private float CountTime = 0;
	private float TimeInterval = 1.0f;
	public static bool CountDawnflag = false;

	// Use this for initialization
	void Start () {
		CountDawnflag = false;
		CountDownText = gameObject.GetComponent<Text> ();

	}
	
	// Update is called once per frame
	void Update () {
		CountTime += Time.deltaTime;
		if (TimeInterval < CountTime) {
			CountTime = 0;
			countDown--;
			CountDownText.text = "" + countDown;
			if (countDown == 0) {
				CountDownText.text =  "Start！";
			} else if (countDown < 0) {
				CountDawnflag = true;
				Destroy (CountDownText);
				Destroy (gameObject);
			}
		}
	}
}
