/*
	スコアに関する事を書きます。
	スコアの処理を書きます。
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	//他のシーンで使用するのでゲーム内のスコアと最大チェイン数をstaticに
	public static int GameScore;
	public static int MaxChain;

	//スコアのテキスト
	private Text Scoretext;

	// Use this for initialization
	void Start () {
		//スコアとチェイン数をリセット
		GameScore = 0;
		MaxChain = 0;
		//スコアのテキストスクリプトを取得して宣言
		Scoretext = gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		//スコアテキストの変更
		ScoreTextFix ();
	}

	void ScoreTextFix(){
		//スコアが10以下であれば
		if (GameScore < 10) {
			//文章を合わせるように
			Scoretext.text = "撃破数：00" + GameScore + "体";
			//スコアが100以下であれば
		} else if (GameScore < 100) {
			//文章を合わせるように
			Scoretext.text = "撃破数：0" + GameScore + "体";
			//スコアが100以上であれば
		} else {
			Scoretext.text = "撃破数：" + GameScore + "体";
		}
	}
		
	//スコアを返すように
	public static int getScore(){
		return GameScore;
	}

	//最大チェイン数を返すように
	public static int getMaxChain(){
		return MaxChain;
	}

}
