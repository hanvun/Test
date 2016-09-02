using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResultManager: MonoBehaviour {

	private int ResultScore;
	private Text ResultText;

	// Use this for initialization
	void Start () {
		ResultScore = Score.GameScore;
		ResultText = gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ResultScore < 10) {
			ResultText.text = "00" + ResultScore + "体でした！";
		} else if (ResultScore < 100) {
			ResultText.text = "0" + ResultScore + "体でした！";
		} else {
			ResultText.text = ResultScore + "体でした！";
		}

		if (Input.GetKey (KeyCode.Z)) {
			SceneManager.LoadScene ("Title");	
		}
	}
}
