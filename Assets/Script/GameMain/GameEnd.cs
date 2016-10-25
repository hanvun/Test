using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour {
	//ゲーム終了までのインターバル
	private float Interval = 2.0f;
	private float EndTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		EndTimer += Time.deltaTime;
		if (EndTimer >= Interval) {
			//制限時間終了でResultへ
			SceneManager.LoadScene ("OutGame");
		}
	}
}
