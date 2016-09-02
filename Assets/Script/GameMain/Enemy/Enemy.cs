/*
 エネミーの行動に関する処理を書きます。
	移動、ライフ管理の処理。ダメージ処理。
	移動に関しては別途化してタイプで分ける形にするかも、ステージ式の場合に考慮中
	爆発アイコンを最大数になった時に赤く点滅させるように
*/

using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	//エネミーライフ
	public int EnemyLife = 10;

	//オブジェクトの宣言
	private GameObject WhiteLingPrefab;
	private GameObject BomberPrefab;
	private GameObject TempWhiteLing;
	private GameObject TempBomber;
	private GameObject Explosive;
	private ChainSystem chainSystem;
	private PlayerController playerController;
	private BoxCollider2D EnemyBoxCollidar;
	private Animator EnemyAnimator;

	//カメラの端を取得
	private Vector2 MaxRightCamera;
	private Vector2 MaxLeftCamera;
	//爆弾の位置
	private Vector2 BomberPosition;

	//爆弾がどれだけ大きいか
	private int BomberStack = 0;
	//爆発はどこまで大きくなれるか
	private int BomberStackMax = 3;
	//爆弾の位置修正値
	private float BomberoffsetHeight = 2.5f;
	private float BomberoffsetWidth  = 0.4f;
	//加算されるサイズの数値
	private float WhiteRingStackSize = 0.1f;
	private float BomberStackSize = 0.05f;
	//移動速度
	private float MoveSpeed = 0.1f;
	//ジャンプ範囲までの距離
	private float Enemydistance = 0.23f;
	//地面までの距離
	private float GroundDistance = 0.15f;
	//現在のスピード
	private float TempSpeed = 0f;
	//ジャンプの初速
	private float JumpSpeed = 0.6f;
	//ジャンプ中かどうか
	private bool isJump = false;
	//ダメージを食らっているかどうか
	private bool DamageFlag = false;
	//ジャンプ中の落下速度
	private float JumpFailingSpeed = 0.032f;
	//通常時の落下速度
	private float EnemyFailingSpeed = 0.2f;
	//最初の爆発かどうか
	public bool FristBomberFlag = true;
	//爆発までの時間
	private float BomberTime = 0.2f;
	//爆発までの時間を計測
	private float BomberTimeCount = 0f;
	//エネミーパラメーターリスト
	private enum EnemyAnimatorParameters
	{
		EnemyWalk,
		EnemyJumpUp,
		EnemyFaling
	}

	// Use this for initialization
	void Start () {
		//爆発のポジションを頭上にくるように調整
		BomberPosition = new Vector2 (transform.position.x + BomberoffsetWidth, transform.position.y + BomberoffsetHeight);
		//カメラの右端を取得
		MaxRightCamera = Camera.main.ViewportToWorldPoint (Vector2.right);
		//カメラの左端を取得
		MaxLeftCamera = Camera.main.ViewportToWorldPoint (Vector2.zero);
		//プレイヤーのオブジェクトを取得
		GameObject PlayerObj = GameObject.Find ("Player");
		//チェインシステムのオブジェクトを取得
		GameObject ChainSystemObj = GameObject.Find ("ChainSystem");
		//爆発アニメーションプレハブの取得
		Explosive = (GameObject)Resources.Load ("Prefab/Explosive");
		//ホワイトリングプレハブの取得
		WhiteLingPrefab = (GameObject)Resources.Load("Prefab/WhiteRing");
		//爆発イラストプレハブの取得
		BomberPrefab = (GameObject)Resources.Load("Prefab/bomber");
		//各種オブジェクトのコンポーネントを取得
		EnemyBoxCollidar = gameObject.GetComponent<BoxCollider2D> ();
		EnemyAnimator = gameObject.GetComponent<Animator> ();
		playerController = PlayerObj.GetComponent<PlayerController> ();
		chainSystem = ChainSystemObj.GetComponent<ChainSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		//カウントダウン中は動かさない
		if (CountDawn.CountDawnflag) {
			LifeManager ();
			//EnemyMove ();
			EnemyBomberDamage ();
		}

	}

	//ダメージが入ったかどうか
	public void Damage( bool IsBulletDamage ){
		//銃のダメージか？
		if (IsBulletDamage) {
			//爆弾スタックが3以下であれば
			if (BomberStackMax > BomberStack) {
				//爆発スタックを加算
				BomberStack++;
				//現在の大きさのリングと爆弾を削除
				Destroy (TempWhiteLing);
				Destroy (TempBomber);
				//爆弾アイコンとリングの生成
				TempWhiteLing = Instantiate (WhiteLingPrefab, transform.position, 
					Quaternion.identity) as GameObject;
				TempBomber = Instantiate (BomberPrefab, BomberPosition, 
					Quaternion.identity) as GameObject;
				//爆発アイコンとリングのサイズを取得
				var Whitesize = TempWhiteLing.gameObject.GetComponent<Transform> ().localScale;
				var Bombersize = TempBomber.gameObject.GetComponent<Transform> ().localScale;
				//スタックの数だけ大きくする
				Whitesize.x += (BomberStack * WhiteRingStackSize);
				Whitesize.y += (BomberStack * WhiteRingStackSize);
				Bombersize.x += (BomberStack * BomberStackSize);
				Bombersize.y += (BomberStack * BomberStackSize);
				//反映させる
				TempWhiteLing.gameObject.GetComponent<Transform> ().localScale = Whitesize;
				TempBomber.gameObject.GetComponent<Transform> ().localScale = Bombersize;
				//ダメージフラグをON
				DamageFlag = true;
			}
			//HPにダメージを与える
			EnemyLife--;
		}
	}

	//エネミーの爆発ダメージ
	void EnemyBomberDamage(){
		//プレイヤーが爆発ボタンを押したか
		if (playerController.BomberDamageFlag) {
			//爆発スタックは3つか
			if (BomberStack >= 3) {
				//エネミーのライフを0にする
				EnemyLife = 0;
				//チェイン数を加算
				chainSystem.ChainAdd();
			}
		}
	}

	void LifeManager(){
		//エネミーライフが0以下になったら消える
		if (EnemyLife <= 0) {
			//最初の爆発のフラグがONであればそのまま違うのであれば爆発までの時間を待ってフラグをON
			if (FristBomberFlag) {
                //ゲームスコアを加算
                Score.GameScore++;
				//チェインテキストを生成
				chainSystem.ChainTextPopup(transform.position);
				//爆発アニメーションを生成
				Instantiate (Explosive, transform.position, Quaternion.identity);
                //現在出している爆発アイコンと白いリングと自身を削除する
                Destroy (TempWhiteLing);
				Destroy (TempBomber);
				Destroy (gameObject);
			} else if (BomberTimeCount >= BomberTime) {
				FristBomberFlag = true;
			}
			//爆発までの時間をカウント
			BomberTimeCount += Time.deltaTime;
		}
	}

	void EnemyMove(){
		//エネミーが左右を移動して地面がなかった場合ジャンプして飛び越えるように
		transform.Translate ( Vector2.right * MoveSpeed );
		//パラメーターの変更
		EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyWalk.ToString(), true);
		//ジャンプエリアにレイがぶつかったら
		if (JumpAreaJudge (Vector2.right, Enemydistance)) {
			//ジャンプフラグをONに
			isJump = true;
		}

		//ジャンプフラグがONになっていればジャンプするように
		if (isJump) {
			//パラメーターの変更
			EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyJumpUp.ToString(), true);
			//上方向に移動
			transform.Translate (Vector2.up * TempSpeed);
			//現在の上昇速度を設定値分減らしていく
			TempSpeed -= JumpFailingSpeed;
			//現在の上昇速度が0以下になったらジャンプフラグをOff
			if (TempSpeed < 0) {
				//パラメーターの変更
				EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyJumpUp.ToString(), false);
				//ジャンプフラグをOFFに
				isJump = false;
			}
		} else {
			//ジャンプフラグがOFFであればジャンプの初速を元に戻すように
			TempSpeed = JumpSpeed;
		}

		//画面外に出ようとしたら反転
		if (MaxRightCamera.x < transform.position.x) {
			transform.Rotate (0, 180, 0);
		}
		//画面外に出ようとしたら反転
		if (MaxLeftCamera.x > transform.position.x) {
			transform.Rotate (0, 180, 0);
		}

		//空中にいたら落下させるように
		if (!GroundJudge (-Vector2.up, GroundDistance) && !isJump) {
			//パラメーターの変更
			EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyFaling.ToString (), true);
			//設定された落下速度分移動
			transform.Translate (-Vector2.up * EnemyFailingSpeed);
		} else {
			//パラメーターの変更
			EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyFaling.ToString(), false);
		}

		//ダメージフラグがONになっていれば爆弾とリングが追従するように
		if (DamageFlag) {
			//爆発アイコンの位置を調整
			BomberPosition = new Vector2 (transform.position.x + BomberoffsetWidth, transform.position.y + BomberoffsetHeight);
			//反映
			TempBomber.gameObject.GetComponent<Transform>().position = BomberPosition;
			//白いリングは真ん中にくるように
			TempWhiteLing.gameObject.GetComponent<Transform> ().position = new Vector2 (transform.position.x, transform.position.y);
		}

	}
	// 地面に当っているか
	bool GroundJudge( Vector2 direction, float distance ) {
		//プレイヤーのColliderオフセットを取得
		float offsetWidth = EnemyBoxCollidar.offset.x;
		float offsetHeight = EnemyBoxCollidar.offset.y;
		//Colliderのサイズを取得して高さ	の設定
		float EnemySizeHeight = ( EnemyBoxCollidar.size.y * 2 ) + offsetHeight;
		//BoxColidarの座標の下段を取得
		Vector2 EnemyUnderPosition = new Vector2 (transform.position.x + offsetWidth, transform.position.y - EnemySizeHeight);

		//上部下段中央の３箇所で同じ方向に向けてRayを飛ばす。
		RaycastHit2D raycastUnder = Physics2D.Raycast( EnemyUnderPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Ground" ) );

		//どれか３箇所の内一つでもHitしていればtrueを返す
		if ( raycastUnder ) {
			return true;
		} else {
			return false;
		}
	}

	// ジャンプエリアに当っているか
	bool JumpAreaJudge( Vector2 direction, float distance ) {
		//プレイヤーのColliderオフセットを取得
		float offsetWidth = EnemyBoxCollidar.offset.x;
		float offsetHeight = EnemyBoxCollidar.offset.y;
		//BoxColidarの座標の上部下段中央を取得
		Vector2 EnemyCenterPosition = new Vector2 (transform.position.x + offsetWidth, transform.position.y + offsetHeight);

		//上部下段中央の３箇所で同じ方向に向けてRayを飛ばす。
		RaycastHit2D raycastCenter = Physics2D.Raycast( EnemyCenterPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "JumpArea" ) );

		//どれか３箇所の内一つでもHitしていればtrueを返す
		if ( raycastCenter ) {
			return true;
		} else {
			return false;
		}
	}

}
