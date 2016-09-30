using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleFade : MonoBehaviour {
	float alfa;
	float speed = 0.01f;
	float red, green, blue;
	void Start () {

		//パネルの色取得
		red = GetComponent<Image>().color.r;
		green = GetComponent<Image>().color.g;
		blue = GetComponent<Image>().color.b;
	}

	void Update () {
		//カラーを設定
		GetComponent<Image>().color = new Color(red, green, blue, alfa);
		//フェード速度
		alfa += speed;
		//完全にフェードしたら
		if (alfa >= 1) {
			SceneManager.LoadScene ("GameMain");
		}
	}
}
