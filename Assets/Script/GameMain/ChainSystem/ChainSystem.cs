/*
 チェインに関係する処理を書きます 
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChainSystem : MonoBehaviour {

	//チェインの時間計測
	private float ChainResetNowTime = 0f;
	//チェインのリセットまでの時間
	private float ChainResetTime = 0.3f;

	//生成するCanvasを指定
	public GameObject TargetCanvas;

	//生成する文字
	private string PopupString;

	//チェインテキストのString
	private string  ChainTextString = "Chain!";
	//文字列の長さ
	private int StringLange;

	//次の文字列の長さ(最大数から回転数分減らしていく)
	private int nextStringLange;

	//生成するテキストアニメーション付きのゲームオブジェクト
	public GameObject PopupTextObject;

	//文字生成フラグ
	private bool TextGanerateFlag = false;

	//生成する文字の場所
	private Vector3 TargetTextPos;

	//生成する文字の場所を左にずらす数値
	private float TextLeftModifyPos = 1.0f;

	//生成する文字間の数値
	private float TextPosWidthSpace = 0.4f;

	//文字生成までの時間計測
	private float TextNowTime = 0f;
	//次の文字生成までの時間
	private float TextNextTime = 0.1f;

	//現在のチェイン数
	private int ChainNumber = 0;
	//前のチェイン数
	private int UndoNumber = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//一定時間立ったらチェイン数をリセットするように
		ChainReset ();
		//チェインテキスト生成フラグがONになったら生成するように
		ChainTextCreate ();
	}
	//チェインテキストを生成
	void ChainTextCreate(){
		//生成フラグがONであれば
		if (TextGanerateFlag) {
			//時間を加算
			TextNowTime += Time.deltaTime;
			//次の文字生成時間より上になれば
			if (TextNowTime >= TextNextTime) {
				//計測タイムをリセット
				TextNowTime = 0;
				//文字列の最大の長さと次の文字列の長さを計算する
				int nowLange = StringLange - nextStringLange;
				//生成ごとに0.4fずつずらしていく
				TargetTextPos.x += TextPosWidthSpace;
				//テキストオブジェクトの生成
				GameObject ValueText = (GameObject)Instantiate (PopupTextObject, TargetTextPos, Quaternion.identity);
				//テキストコンポーネントを取得
				Text chaintext = ValueText.GetComponent<Text> ();
				//現在の位置の文字列をテキストにする　
				chaintext.text = PopupString.Substring (nowLange, 1);
				//テキストをキャンバスのワールド座標へ変換
				ValueText.transform.SetParent (TargetCanvas.transform, false);
				//次の文字列へ
				nextStringLange--;
				//この時点で0以下になった場合生成フラグをOFFに
				if (StringLange - nextStringLange == StringLange) {
					TextGanerateFlag = false;
				}
			}
		} else {
			//文字列をリセット
			PopupString = ChainTextString;
		}
	}

	//チェインテキストを指定された場所に生成するためのフラグと場所を取得
	public void ChainTextPopup( Vector3 Target ){
		//目標のポジションを保存
		TargetTextPos = Target;
		//目標の位置の真ん中を取得しているので左に修正する
		TargetTextPos.x -= TextLeftModifyPos;
		//現在のチェイン数＋設定された文字列を保存
		PopupString = ChainNumber.ToString () + ChainTextString;
		//文字列の最大数を保存
		StringLange = PopupString.Length;
		//次の文字列の長さを同じ数値に
		nextStringLange = StringLange;
		//生成時間を最初は満たしているように
		TextNowTime = TextNextTime;
		//生成フラグをON
		TextGanerateFlag = true;
	}

	void ChainReset(){
		//今のチェイン数と前のチェイン数が違うか
		if (UndoNumber != ChainNumber) {
			//秒数を加算
			ChainResetNowTime += Time.deltaTime;
			//違った状態で一定秒数立ったら
			if (ChainResetNowTime >= ChainResetTime) {
				//計測タイムをリセット
				ChainResetNowTime = 0;
				//チェイン数をリセット
				UndoNumber = 0;
				ChainNumber = 0;
			}
		}
	}

	//チェイン数の加算
	public void ChainAdd(){
		//前のチェイン数を保存
		UndoNumber = ChainNumber;
		//現在のチェイン数を加算
		ChainNumber++;
		//チェインリセットまでの時間をリセット
		ChainResetNowTime = 0;
	}

	//テキストを表示するPosを返すように
	public Vector3 getTargetTextPos(){
		return TargetTextPos;
	}


}
