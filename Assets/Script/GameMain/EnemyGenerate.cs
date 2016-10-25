using UnityEngine;
using System.Collections;

public class EnemyGenerate : MonoBehaviour {
	public GameObject EnemyPrefab;
	private float SpawnTime;
	public float SpawnnextTime = 5.0f;

	//カメラの端を取得
	private Vector2 MaxRightCamera;
	private Vector2 MaxLeftCamera;
    //配置修正
	private float SpawnWidthOffset = 1.0f;
	private float RightSpawnHeightOffset = 3.9f;
	private float LeftSpawnHeightOffset = 3.9f;
    //エネミーの回転角度
	private Quaternion EnemyRotation;

	// Use this for initialization
	void Start () {
        //180度回転するように設定
		EnemyRotation.eulerAngles = new Vector3 (0, 180, 0);
        //右端を取得
		MaxRightCamera = Camera.main.ViewportToWorldPoint (Vector2.right);
        //左端を取得
		MaxLeftCamera = Camera.main.ViewportToWorldPoint (Vector2.zero);
        //配置修正を適用
		MaxRightCamera.y += RightSpawnHeightOffset;
		MaxRightCamera.x -= SpawnWidthOffset;
		MaxLeftCamera.x += SpawnWidthOffset;
		MaxLeftCamera.y += LeftSpawnHeightOffset;
        //初期エネミー生成
		Instantiate (EnemyPrefab, MaxLeftCamera, Quaternion.identity);
		Instantiate (EnemyPrefab, MaxRightCamera, EnemyRotation);
	}
	
	// Update is called once per frame
	void Update () {
        //カウントダウンが終わっていれば
		if (CountDawn.CountDawnflag) {
            //時間を加算
			SpawnTime += Time.deltaTime;
            //加算された時間が設定時間より多ければ
			if (SpawnTime >= SpawnnextTime) {
                //計測時間をリセット
				SpawnTime = 0;
                //エネミー生成
				Instantiate (EnemyPrefab, MaxLeftCamera,Quaternion.identity);
				Instantiate (EnemyPrefab, MaxRightCamera, EnemyRotation);
			}
		}
	}
}
