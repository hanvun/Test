using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
	private GameObject FadePrefab;
	public GameObject TargetCanvas;
	// Use this for initialization
	void Start () {
		//プレハブ取得
		FadePrefab = (GameObject)Resources.Load ("Prefab/TitleFadePanel");
	}
	
	// Update is called once per frame
	void Update () {
        //Ｚを押したらフェードアウトパネルを出す
		if (Input.GetKeyDown (KeyCode.Z)) {	
			GameObject fadeoutObj = (GameObject)Instantiate (FadePrefab);
			fadeoutObj.transform.SetParent (TargetCanvas.transform, false);
		}
	}
}
