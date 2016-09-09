using UnityEngine;
using System.Collections;
using System;

public class Camera2DController : MonoBehaviour {

	public GameObject Player;
	public Transform Target;
	public Transform StopPosition;
	public Transform StartPosition;

	private Vector2 cameraStartPostion;

	// Use this for initialization
	void Start () {
		//カメラの初期位置の中心を取得
		cameraStartPostion = Camera.main.ViewportToWorldPoint(Vector2.one * 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		//カメラの一番右を取得
		var Maxright = Camera.main.ViewportToWorldPoint(Vector2.right);
		//カメラの一番右を半分にして中心のXを取得
		var center =  Camera.main.ViewportToWorldPoint(Vector2.one * 0.5f);

		//右に移動する場合カメラを中心座標に合わせる。
		if (StopPosition.position.x - Maxright.x < 0){
				Debug.Log ("カメラ端");
		} else if (center.x < Target.position.x)
		{
			var pos = Camera.main.transform.position;

			if (Math.Abs(pos.x - Target.position.x) >= 0.0000001f)
			{
				Camera.main.transform.position = new Vector3(Target.position.x, pos.y, pos.z);
			}
		}
		//初期位置の真ん中より左であれば処理をせずそれ以外で左に移動する場合はカメラを中心座標に合わせる
		if (cameraStartPostion.x > Target.position.x) {
		} else if (center.x > Target.position.x){
			var pos = Camera.main.transform.position;

			if (Math.Abs(pos.x - Target.position.x) >= 0.0000001f)
			{
				Camera.main.transform.position = new Vector3(Target.position.x, pos.y, pos.z);
			}
		}
			

	}
}
