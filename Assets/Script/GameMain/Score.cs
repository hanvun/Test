using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    //敵撃破数
	public static int GameScore = 0;
    //スコアテキスト
	private Text Scoretext;

	// Use this for initialization
	void Start () {
        //撃破数を初期化
		GameScore = 0;
        //スコアテキストを取得
		Scoretext = gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
        //ゲームスコアが一定数以下であれば文章を修正する
		if (GameScore < 10) {
			Scoretext.text = "撃破数：00" + GameScore + "体";
		} else if (GameScore < 100) {
			Scoretext.text = "撃破数：0" + GameScore + "体";
		} else {
			Scoretext.text = "撃破数：" + GameScore + "体";
		}
	}

    //ゲームのスコアを返す
	public static int getScore(){
		return GameScore;
	}
}
