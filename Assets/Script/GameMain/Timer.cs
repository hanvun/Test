using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

	public float GameTimer = 3;
	private float timer = 0;
	private Text TimerText;
	private float LimitTimer = 1;
	private float CurrentTimer = 0;

	// Use this for initialization
	void Start () {
		TimerText = gameObject.GetComponent<Text> ();
		TimerText.text = "Time:0" + GameTimer + ":00";
	}
	
	// Update is called once per frame
	void Update () {
		if (CountDawn.CountDawnflag) {
			if (GameTimer >= 0) {
				CurrentTimer += Time.deltaTime;
				if (CurrentTimer >= LimitTimer) {
					timer--;
					CurrentTimer = 0;

					if (timer < 0) {
						GameTimer--;
						timer = 59;
					}
				}
			} else {
				SceneManager.LoadScene ("OutGame");
			}

			if (timer < 10) {
				TimerText.text = "Time:0" + GameTimer + ":0" + timer;
			} else {
				TimerText.text = "Time:0" + GameTimer + ":" + timer;
			}
		}
	}

	public float Gettimer(){
		return timer;
	}
}
