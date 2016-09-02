/*
 エネミーの生成に関する処理を書きます
 */
using UnityEngine;
using System.Collections;

public class EnemyGenerate : MonoBehaviour {
	public GameObject EnemyPrefab;
	private float SpawnTime;
	public float SpawnnextTime = 5.0f;

	//カメラの端を取得
	private Vector2 MaxRightCamera;
	private Vector2 MaxLeftCamera;
	private float SpawnWidthOffset = 1.0f;
	private float RightSpawnHeightOffset = 4.0f;
	private float LeftSpawnHeightOffset = 5.0f;
	private Quaternion EnemyRotation;

	// Use this for initialization
	void Start () {
		EnemyRotation.eulerAngles = new Vector3 (0, 180, 0);
		MaxRightCamera = Camera.main.ViewportToWorldPoint (Vector2.right);
		MaxLeftCamera = Camera.main.ViewportToWorldPoint (Vector2.zero);
		MaxRightCamera.y += RightSpawnHeightOffset;
		MaxRightCamera.x -= SpawnWidthOffset;
		MaxLeftCamera.x += SpawnWidthOffset;
		MaxLeftCamera.y += LeftSpawnHeightOffset;
		Instantiate (EnemyPrefab, MaxLeftCamera,Quaternion.identity);
		Instantiate (EnemyPrefab, MaxRightCamera, EnemyRotation);
	}
	
	// Update is called once per frame
	void Update () {
		if (CountDawn.CountDawnflag) {
			SpawnTime += Time.deltaTime;
			if (SpawnTime >= SpawnnextTime) {
				SpawnTime = 0;
				Instantiate (EnemyPrefab, MaxLeftCamera,Quaternion.identity);
				Instantiate (EnemyPrefab, MaxRightCamera, EnemyRotation);
			}
		}
	}
}
