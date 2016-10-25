using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private GameObject target;

	private bool targetTurn;
    //弾の速度
	private float BulletSpeed = 0.3f;
    //弾のダメージかどうか
	private const bool BulletDamage = true;

	//玉からエネミーまでの距離判定
	private float BulletDistance = 0.3f;

	//オフセットをずらす
	private Vector2 BulletOffset = new Vector2( 1.0f,0.3f);

	// Use this for initialization
	void Start () {
        //プレイヤーを取得
		target = GameObject.Find("Player");

		//ここでターゲットの打った瞬間の方向を取得
		targetTurn = target.GetComponent<PlayerController>().turnFlag;
        //反対を向いていれば
		if (targetTurn) {
            //offset設定
			var pos = gameObject.transform.position;
			pos.x -= BulletOffset.x;
			pos.y += BulletOffset.y;
			gameObject.transform.position = pos;
		} else {
			var pos = gameObject.transform.position;
			pos.x += BulletOffset.x;
			pos.y += BulletOffset.y;
			gameObject.transform.position = pos;
		}
	}
	
	// Update is called once per frame
	void Update () {
        //反対を向いているか
		if (targetTurn) {
            //弾の移動
			transform.Translate (-Vector2.right * BulletSpeed);
		} else {
			transform.Translate (Vector2.right * BulletSpeed);
		}

		//カメラの一番右を取得
		var Maxright = Camera.main.ViewportToWorldPoint(Vector2.right);
		//カメラの一番左を取得
		var Maxleft = Camera.main.ViewportToWorldPoint(Vector2.zero);

		//弾が打った瞬間の方向に進んでいってカメラ外にいったら消えるようにEnemyレイヤーにぶつかったら対象のエネミーにダメージ処理
		if ( gameObject.transform.position.x <= Maxleft.x  || gameObject.transform.position.x >= Maxright.x ) {
			Destroy (gameObject);
		}

		//Enemyレイヤーにレイがぶつかったら弾を消去
		if (targetTurn) {
			if ( BulletHit ( -Vector2.right ) ) {
				Destroy (gameObject);
			}
		} else {
			if ( BulletHit ( Vector2.right ) ) {
				Destroy (gameObject);
			}
		}
	}

	//弾があたったか
	bool BulletHit( Vector2 direction ) {
		//レイキャストを設定
		RaycastHit2D raycastCenter = Physics2D.Raycast (transform.position, direction, BulletDistance, 
			1 << LayerMask.NameToLayer ("Enemy"));
		if (raycastCenter) {
			//ぶつかっていたらエネミーのダメージを行うように
			raycastCenter.collider.gameObject.GetComponent<Enemy> ().Damage ( BulletDamage );
			return true;
		} else {
			return false;
		}
	}
}
