using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultManager: MonoBehaviour {
    //リザルトスコア
	private int ResultScore;
    //リザルトテキスト
	public Text ResultText;

	private GameObject FadePrefab;
	public GameObject TargetCanvas;

	// Use this for initialization
	void Start () {
        //スコアのゲーム内撃破数を取得
		ResultScore = Score.GameScore;
		//プレハブ取得
		FadePrefab = (GameObject)Resources.Load ("Prefab/ResultFadePanel");
	}
	
	// Update is called once per frame
	void Update () {
		//撃破数によってリザルトテキストを修正する
		if (ResultScore < 10) {
			ResultText.text = "00" + ResultScore;
		} else if (ResultScore < 100) {
			ResultText.text = "0" + ResultScore;
		} else {
			ResultText.text = "" + ResultScore;
		}
        //Zを押したらフェードインしてタイトルに戻るように
		if (Input.GetKeyDown (KeyCode.Z)) {
			GameObject fadeoutObj = (GameObject)Instantiate (FadePrefab);
			fadeoutObj.transform.SetParent (TargetCanvas.transform, false);
		}
	}
}
