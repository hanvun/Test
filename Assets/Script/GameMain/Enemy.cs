using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	//エネミーライフ
	public int EnemyLife = 10;

	private GameObject TempWhiteLing;
	private GameObject TempBomber;
	private GameObject Explosive;
	private GameObject Player;
	private PlayerController playerController;
	//爆弾がどれだけ大きいか
	private int BomberStack = 0;
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

	//カメラの端を取得
	private Vector2 MaxRightCamera;
	private Vector2 MaxLeftCamera;
	//爆弾の位置
	private Vector2 BomberPosition;

	private BoxCollider2D EnemyBoxCollidar;
	private Animator EnemyAnimator;
	//エネミーパラメーターリスト
	private enum EnemyAnimatorParameters
	{
		EnemyWalk,
		EnemyJumpUp,
		EnemyFaling
	}

	// Use this for initialization
	void Start () {
        //カメラの右端を取得
		MaxRightCamera = Camera.main.ViewportToWorldPoint (Vector2.right);
        //カメラの左端を取得
		MaxLeftCamera = Camera.main.ViewportToWorldPoint (Vector2.zero);
        //プレイヤーを取得
		Player = GameObject.Find ("Player");
        //プレハブ取得
		Explosive = (GameObject)Resources.Load ("Prefab/Explosive");
        //各種コンポーネント取得
		EnemyBoxCollidar = gameObject.GetComponent<BoxCollider2D> ();
		EnemyAnimator = gameObject.GetComponent<Animator> ();
		playerController = Player.GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
        //カウントダウンが終わっていれば動く
		if (CountDawn.CountDawnflag) {
			LifeManager ();
			EnemyMove ();
			EnemyBomberDamage ();
			BomberColorChange ();
		}

	}

    //ダメージ処理
	public void Damage( bool IsBulletDamage ){
        //銃撃ダメージか？
		if (IsBulletDamage) {
            //爆弾スタックが最大でなければ
			if (PlayerController.BomberStackMax > BomberStack) {
				//爆弾の初期ポジションを自身の上に設定
				BomberPosition = new Vector2 (transform.position.x + BomberoffsetWidth, transform.position.y + BomberoffsetHeight);
				//爆発スタックを加算
				BomberStack++;
				//現在の大きさのリングと爆弾を削除
				Destroy (TempWhiteLing);
				Destroy (TempBomber);
				//爆弾アイコンとリングの生成
				TempWhiteLing = Instantiate ((GameObject)Resources.Load("Prefab/WhiteRing"), transform.position, 
					Quaternion.identity) as GameObject;
				TempBomber = Instantiate ((GameObject)Resources.Load("Prefab/bomber"), BomberPosition, 
					Quaternion.identity) as GameObject;
                //サイズを取得
				var Whitesize = TempWhiteLing.gameObject.GetComponent<Transform> ().localScale;
				var Bombersize = TempBomber.gameObject.GetComponent<Transform> ().localScale;
                //爆弾の大きさに合わせてサイズを変更
				Whitesize.x += (BomberStack * WhiteRingStackSize);
				Whitesize.y += (BomberStack * WhiteRingStackSize);
				Bombersize.x += (BomberStack * BomberStackSize);
				Bombersize.y += (BomberStack * BomberStackSize);
                //サイズを適用
				TempWhiteLing.gameObject.GetComponent<Transform> ().localScale = Whitesize;
				TempBomber.gameObject.GetComponent<Transform> ().localScale = Bombersize;
				//ダメージフラグをON
				DamageFlag = true;
			}
		}
		//HPにダメージを与える
		EnemyLife--;
	}

    //エネミーの爆発ダメージ
	void EnemyBomberDamage(){
        //プレイヤーが爆発フラグをtrueにしているか
		if (playerController.BomberDamageFlag) {
            //爆発スタックは3か？
			if (BomberStack == 3) {
                //エネミーのライフを0に
				EnemyLife = 0;
                //プレイヤーの爆発フラグをfalseに
				playerController.BomberDamageFlag = false;
			}
		}
	}

	//エネミーが爆発出来るのであれば爆弾が赤くなるように
	void BomberColorChange(){
		if (BomberStack == 3) {
			//赤色にマテリアルを変える
			TempBomber.GetComponent<Renderer> ().material.color = Color.red;
		}
	}

	void LifeManager(){
		//エネミーライフが0以下になったら消える
		if (EnemyLife <= 0) {
            //撃破数を加算
			Score.GameScore++;
            //爆発アニメーション生成
			Instantiate (Explosive, transform.position, Quaternion.identity);
            //各種生成オブジェクトを削除
			Destroy (TempWhiteLing);
			Destroy (TempBomber);
			Destroy (gameObject);
		}
	}

	void EnemyMove(){
		//エネミーが左右を移動して地面がなかった場合ジャンプして飛び越えるように
		transform.Translate ( Vector2.right * MoveSpeed );
        //移動アニメーションを再生
		EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyWalk.ToString(), true);
        //目の前にジャンプエリアがあれば
		if (JumpAreaJudge (Vector2.right, Enemydistance)) {
            //ジャンプフラグをtrueに
			isJump = true;
		}

		//ジャンプフラグがtrueになっていればジャンプするように
		if (isJump) {
            //ジャンプ上昇アニメーションを再生
			EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyJumpUp.ToString(), true);
            //上へ速度分移動
			transform.Translate (Vector2.up * TempSpeed);
            //速度を設定分下げていく
			TempSpeed -= JumpFailingSpeed;
			//現在の速度が0以下になったらジャンプフラグをOff
			if (TempSpeed < 0) {
                //ジャンプ上昇アニメーションを停止
				EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyJumpUp.ToString(), false);
                //ジャンプフラグをfalse
				isJump = false;
			}
		} else {
			//ジャンプフラグがfalseであればジャンプの初速を元に戻すように
			TempSpeed = JumpSpeed;
		}

		//画面外に出ようとしたら反転
		if (MaxRightCamera.x < transform.position.x) {
            //180度回転
			transform.Rotate (0, 180, 0);
		}
		if (MaxLeftCamera.x > transform.position.x) {
            //180度回転
			transform.Rotate (0, 180, 0);
		}

		//空中にいたら落下させるように
		if (!GroundJudge (-Vector2.up, GroundDistance) && !isJump) {
            //落下アニメーション再生
			EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyFaling.ToString (), true);
            //落下速度分移動
			transform.Translate (-Vector2.up * EnemyFailingSpeed);
		} else {
            //下降アニメーションを停止
			EnemyAnimator.SetBool (EnemyAnimatorParameters.EnemyFaling.ToString(), false);
		}

		//ダメージフラグがONになっていれば爆弾とリングが追従するように
		if (DamageFlag) {
            //爆弾のポジションを自身の上に設定
			BomberPosition = new Vector2 (transform.position.x + BomberoffsetWidth, transform.position.y + BomberoffsetHeight);
            //適用
			TempBomber.gameObject.GetComponent<Transform>().position = BomberPosition;
            //リングも移動
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
