using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {
    //アニメーション再生終了時に実行
	void AnimDestory(){
		Destroy(gameObject);
	}
}
