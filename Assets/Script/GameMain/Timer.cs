using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {
    //ゲーム時間（分数）
	public float GameTimer = 3;
    //現在秒数保存用時間
	public float timer = 0;
    //タイマーテキスト
	private Text TimerText;
    //1秒を設定
	private const float LimitCurrentTimer = 1;
    //1分（秒数）を設定
    private const float LimitTimer = 59;
    //1秒を計測
	private float CurrentTimer = 0;

	//ジャンプエリアの最大数
	private int JumpAreaMax = 2;
	//ジャンプエリアを取得
	public GameObject[] JumpAreas = new GameObject[2];

	//終了時の処理を行うオブジェクトのプレハブを取得
	private GameObject EndPrefab;

	//キャンバスの取得
	public GameObject TargetCanvas;

	//終了処理をしているか
	private bool EndFlag;

	// Use this for initialization
	void Start () {

		//終了処理をflaseに
		EndFlag = false;

		//ジャンプエリアを非表示に
		for (int i = 0; i < JumpAreaMax; i++) {
			JumpAreas [i].SetActive(false);
		}

		//プレハブのロード
		EndPrefab = (GameObject)Resources.Load ("Prefab/EndText");

        //タイマーテキストを取得
		TimerText = gameObject.GetComponent<Text> ();

        //タイマーテキストを設定
		if (timer < 10) {
			TimerText.text = "Time:0" + GameTimer + ":0" + timer;
		} else {
			TimerText.text = "Time:0" + GameTimer + ":" + timer;
		}
	}
	
	// Update is called once per frame
	void Update () {
        //カウントダウンが終了しているか
		if (CountDawn.CountDawnflag) {
            //ゲーム時間が0分以上であれば
			if (GameTimer >= 0) {
                //計測時間を加算
				CurrentTimer += Time.deltaTime;
                //1秒以上になったら
				if (CurrentTimer >= LimitCurrentTimer) {
					//秒数を減らす
					timer--;
					//計測時間をリセット
					CurrentTimer = 0;
					//秒数が0以下であれば
					if (timer < 0) {
						//ゲーム時間を減らす
						GameTimer--;
						//現在秒数を設定
						timer = LimitTimer;
					}
				}
			} else {
				//カウントダウン状態に戻す
				CountDawn.CountDawnflag = false;
				EndFlag = true;
			}

			//ゲーム時間が1分以下になればジャンプエリアを開放する
			if (GameTimer <= 0) {
				//ジャンプエリアを表示に
				for (int i = 0; i < JumpAreaMax; i++) {
					JumpAreas [i].SetActive(true);
				}
			}

            //秒数が10秒以下であればテキストを修正
			if (timer < 10) {
				TimerText.text = "Time:0" + GameTimer + ":0" + timer;
			} else {
				TimerText.text = "Time:0" + GameTimer + ":" + timer;
			}
		} 
		if (EndFlag) {
			//終了オブジェクトを生成してキャンバスの中に
			GameObject EndObj = (GameObject)Instantiate (EndPrefab);
			EndObj.transform.SetParent (TargetCanvas.transform, false);
			EndFlag = false;
		}
	}
    //現在の秒数を返す
	public float Gettimer(){
		return timer;
	}
}
