/*
	Titleを管理する処理を行います。
	Zキーを押したらゲームメインにように、今後キーコンフィグやチュートリアルに
	飛べるようにするかも？
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Z)) {
			SceneManager.LoadScene ("GameMain");	
		}
	}
}
