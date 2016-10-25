using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountDawn : MonoBehaviour {
    //カウントダウンテキスト
	private Text CountDownText;
    //カウントダウン回数
	private int countDown = 3;
    //秒数を計測
	private float CountTime = 0;
    //１秒のインターバルを設定
	private float TimeInterval = 1.0f;
    //カウントダウンが終わったかどうか
	public static bool CountDawnflag = false;

	// Use this for initialization
	void Start () {
        //カウントダウンフラグをfalseに
		CountDawnflag = false;
        //自信のテキストを取得
		CountDownText = gameObject.GetComponent<Text> ();

	}
	
	// Update is called once per frame
	void Update () {
        //時間を加算
		CountTime += Time.deltaTime;
        //設定秒数経ったら
		if (TimeInterval < CountTime) {
            //計測秒数をリセット
			CountTime = 0;
            //カウントダウンを減らす
			countDown--;
            //カウントダウンテキストを更新
			CountDownText.text = "" + countDown;
            //カウントダウンが0であればStart!の文章に 0以下ならばカウントダウンフラグをtrueに
			if (countDown == 0) {
				CountDownText.text =  "Start！";
			} else if (countDown < 0) {
				CountDawnflag = true;
                //自身とカウントダウンテキストを削除
				Destroy (gameObject);
			}
		}
	}
}
