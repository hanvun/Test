/*
チェインのテキストの数字や処理を書きます。 
現在カメラの位置とオブジェクトの位置を相対的に判断して移動する処理を書いています。
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChainText : MonoBehaviour {
	//RectTranformを取得
	private RectTransform myRectTrans;
	//カメラを取得
	private Camera uiCamera;

    //フィニッシュしたかどうかを取得
    private bool IsStartAnimationfinish = false;

    //フェードアウトまでの時間計測
    private float FadeoutNowTime = 0f;
    //フェードアウトまでの時間
    private float FadeoutNextime = 0.3f;
	//キャンバスのRectTransform
	private RectTransform canvasRect;
	//ターゲットのオブジェクトの場所
	private Vector3 TargetObjectPos;

    void Awake(){
		//チェインシステムを取得
		GameObject ChainSystemObj = GameObject.Find ("ChainSystem");
		//目標のオブジェクトのポジションを取得
		//TargetObjectPos = ChainSystemObj.GetComponent<ChainSystem> ().getTargetTextPos();
		Debug.Log (TargetObjectPos);
		//自身のRectTransFormを取得
		myRectTrans = gameObject.GetComponent<RectTransform>();
		//キャンバスを取得
		GameObject canvasObj = GameObject.Find("Canvas");
		//キャンバスのRectTranformを取得
		canvasRect = canvasObj.GetComponent<RectTransform> ();
		//キャンバスのワールド座標を取得
		uiCamera = Camera.main;
		//自身のRectTranformとCanvasのワールド座標を取得出来ていれば実行
		if (myRectTrans != null && uiCamera != null) {
			//座標を補正
			UpdateUiLocalPosFromTargetPos ();
		}
		//テキストアニメーション
		gameObject.GetComponent<Animator> ().SetTrigger ("AnimationStart");
	}

    void Update()
    {
        //自身のRectTranformとCanvasのワールド座標を取得出来ていれば実行
        if (myRectTrans != null  && uiCamera != null){
			//補正
            UpdateUiLocalPosFromTargetPos();
        }
		if (IsStartAnimationfinish){
            //時間を加算
            FadeoutNowTime += Time.deltaTime;
            //次の文字生成時間より上になれば
            if (FadeoutNowTime >= FadeoutNextime){
				//フェードアウトアニメーション
				gameObject.GetComponent<Animator> ().SetTrigger ("Fadeout");
            }
        }
    }
	//
    void UpdateUiLocalPosFromTargetPos(){
		var localPos = Vector2.zero;
		var worldCamera = Camera.main;
		//カメラのワールド座標から対象の座標の位置を取得
		var screenPos = RectTransformUtility.WorldToScreenPoint (worldCamera, TargetObjectPos);
		//キャンバスの座標に対象の座標を変換して保存
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out localPos);
		//保存された座標を加算
		myRectTrans.localPosition = localPos;
	}

	//文字登場時のアニメーションにて実行
	public void OnAnimationFinish(){
		//はじめのアニメーションの終了フラグを取得
		IsStartAnimationfinish = true;
	}

	//フェードアウトアニメーション終了時に実行
	public void OnDestoryObject(){
		Destroy (gameObject);
	}
}
