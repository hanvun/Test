using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextLight : MonoBehaviour {
	public GameObject _Start;
	//点滅速度
	private float _Step = 0.02f;

	void Start()
	{
	}

	void Update(){
		// 現在のAlpha値を取得
		float toColor = _Start.GetComponent<CanvasRenderer>().GetAlpha();
		// Alphaが0 または 1になったら増減値を反転
		if (toColor < 0 || toColor > 1)
		{
			_Step = _Step * -1;
		}
		// Alpha値を増減させてセット
		_Start.GetComponent<CanvasRenderer>().SetAlpha(toColor + _Step);
	}
}
