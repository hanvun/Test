/*
プレイヤーのアクションに関する事を書きます
攻撃、移動、ジャンプ、ライフ管理を行います。
爆発を連打できないように一回押したら待つようにゲージを入れる予定
*/
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
	//プレイヤーの移動速度
	public float PlayerSpeed = 0.15f;
	//プレイヤーの通常時落下速度
	public float PlayerFailingSpeed = 0.1f;
	//プレイヤーのジャンプ中の落下速度
	public float JumpFailingSpeed = 0.03f;
	//ジャンプ時の初速度
	public float JumpSpeed = 0.1f;
	//プレイヤーのライフ
	public int PlayerLife = 3;
	//現在のジャンプ速度
	private float TempSpeed;

	//プレイヤーで使用するオブジェクトの宣言
	private BoxCollider2D PlayerBoxCollidar;
	private Animator PlayerAnimator;
	public Canvas canvas;
	public GameObject bulletPrefab;
	public GameObject LifePrefab;
	public GameObject PlayerSpawnPoint;
	private GameObject[ ] player_life_clones;

	//左を向いているか
	public bool turnFlag = false;
	//地面に触れているか
	[SerializeField]
	private bool isGround = true;
	//ジャンプ中か
	[SerializeField]
	private bool isJump = false;
	//銃を打った始めか
	[SerializeField]
	private bool GunTimeInflag = false;
	//プレイヤーのライフを減らすか
	[SerializeField]
	private bool PlayerLifeDown = false;
	//プレイヤーが無敵状態か
	[SerializeField]
	private bool PlayerInvincible = false;

	//プレイヤーから地面までの下の距離
	private float groundUnderDistance = 0.1f;
	//プレイヤーが落下までの距離
	private float FalingDistance = 0.1f;
	//プレイヤーから地面までの左までの距離
	private float groundLeftDistance = 0.3f;
	//プレイヤーから地面までの右までの距離
	private float groundRightDistance = 0.3f;
    //エネミーの当たり判定の距離
	private float EnemyDistance = 0.2f;

	//無敵の時間
	private float InvincibleTime = 0;
	private float InvincibleInterval = 3.0f;

	//ダメージ状態の演出時間
	private float DamageTime = 0;
	private float DamageInterval = 0.1f;

	//銃撃時間
	private float bulletTime = 0;
	//次弾が発射できるまでの間隔
	private float bulletInterval = 0.15f;

    //プレイヤーのライフをずらしていく数値
	private const int LifeWidth = 40;

	//エネミーの爆発を行うか
	public bool BomberDamageFlag = false;

	//プレイヤーで再生するSEの宣言
	public AudioClip GunSe;
	private AudioSource PlayerAudio;

	//アニメーションで使用するパラメーターリスト
	private enum PlayerAnimatorParameters
	{
		GunTime,
		PlayerJumpup,
		PlayerFaling,
		GunTimeIn,
		PlayerWalk
	}


	// 初期設定
	void Start () {
		//弾を打ってない間はインターバルの数値を初期値に
		bulletTime = bulletInterval;
		//現在の速度を初期速度に
		TempSpeed = JumpSpeed;
		PlayerBoxCollidar = gameObject.GetComponent<BoxCollider2D> ();
		PlayerAnimator = gameObject.GetComponent<Animator> ();
		PlayerAudio = gameObject.GetComponent<AudioSource> ();
		//ライフの数だけ配列を設定
		player_life_clones = new GameObject[PlayerLife];
		//ライフのUIを設置
		for ( int i = 0; i < PlayerLife; i++ ) {
			//ライフのUIを生成
			player_life_clones[i] = Instantiate( LifePrefab ) as GameObject;
			var pos = player_life_clones [i].GetComponent<RectTransform> ().position;
			//ライフを横にずらす
			pos.x += i * LifeWidth;
			//変更を適用
			player_life_clones[i].GetComponent<RectTransform> ().position = pos;
			//canvas内部にライフを移動
			player_life_clones[i].transform.SetParent( canvas.transform,false );
		}
			
	}
	
	// Update is called once per frame
	void Update ( ) {
		//カウントダウンの最中は動かないように
		if (CountDawn.CountDawnflag) {
			ground ();
			move ();
			jump ();
			Attack ();
			PlayerLifeManager ();
		}
	}

	void move(){
		//左を入力して地面に触れていなければ左に移動
		if (Input.GetKey (KeyCode.LeftArrow)) {
			//地面に触れていなければプレイヤーの移動
			if (!GroundJudge (-Vector2.right, groundLeftDistance)) {
				//ターンフラグがFALSEであれば反対に
				if (!turnFlag) {
					transform.Rotate (0, 180, 0);
					turnFlag = true;
				}
				//アニメーションのパラメーターを変更
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerWalk.ToString (), true);
				//プレイヤーのスピード分ずらす
				transform.Translate (Vector2.right * PlayerSpeed);
			}
		}
		//右を入力して地面に触れていなければ右に移動
		else if (Input.GetKey (KeyCode.RightArrow)) {
			//地面に触れていなければプレイヤーの移動
			if (!GroundJudge (Vector2.right, groundRightDistance)) {
				//ターンフラグがFALSEであれば反対に
				if (turnFlag) {
					transform.Rotate (0, 180, 0);
					turnFlag = false;
				}
				//アニメーションのパラメーターを変更
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerWalk.ToString (), true);
				//プレイヤーのスピード分ずらす
				transform.Translate (Vector2.right * PlayerSpeed);
			}
		} else {
			//アニメーションのパラメーターを変更
			PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerWalk.ToString (), false);
		}
		//地面に触れていなくてジャンプフラグをONになってなければ落下
		if ( !isGround && !isJump ) {
			transform.Translate (-Vector2.up * PlayerFailingSpeed);
			//落下アニメーション
			PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerFaling.ToString (), true);
		} else {
			//落下アニメーションをオフ
			PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerFaling.ToString (), false);
		}

		//落下ポイントに触れていれば
		if( FallingJudge( -Vector2.up, FalingDistance ) ){
			//プレイヤーのライフを減らすフラグをONに
			PlayerLifeDown = true;
			//プレイヤーのスポーンポイントに移動
			transform.position = PlayerSpawnPoint.GetComponent<Transform> ().position;
		}

	}

	void ground(){
		//地面に触れていてジャンプ中でなければ落下フラグをONに
		if ( GroundJudge ( -Vector2.up, groundUnderDistance ) && !isJump ) {
			isGround = true;
		} else {
			isGround = false;
		}
			
	}

	void jump( ){
		//ジャンプキーが押されて地面に触れていればジャンプフラグONに
		if ( Input.GetKey ( KeyCode.C ) && isGround ) {
			isJump = true;
		}
		//ジャンプフラグがONになっていなければジャンプ。
		//OFFになっていれば現在のジャンプ速度にジャンプ速度を代入。
		if ( isJump ) {
			//上昇アニメーション
			PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerJumpup.ToString (), true);
			//空中に現在の速度分移動
			transform.Translate ( Vector2.up * TempSpeed);
			//現在の速度をジャンプの減少速度分減らす
			TempSpeed -= JumpFailingSpeed;
			//現在のスピードが0以下になったらジャンプフラグをOff
			if (TempSpeed < 0) {
				//ジャンプフラグをオフ
				isJump = false;
				//上昇アニメーションをオフ
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerJumpup.ToString (), false);
			}

		} else { 
			//現在の速度を最初の速度に
			TempSpeed = JumpSpeed;
		}
	}

	//攻撃
	void Attack(){
		if (Input.GetKey (KeyCode.Z) ) {
			//銃撃を始めたのが最初ならtrueに
			if (!GunTimeInflag) {
				//パラメーターを変更
				PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTimeIn.ToString (), true);
				//ガンタイムフラグをTrueにして入らないように
				GunTimeInflag = true;
			} else {
				//パラメーターを変更
				PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTimeIn.ToString (), false);
			}

			//射撃時間を加算
			bulletTime += Time.deltaTime;
			//射撃時間がインターバル以上か
			if (bulletTime > bulletInterval) {
				//射撃時間をリセット
				bulletTime = 0.0f;
				//射撃SEを再生
				PlayerAudio.PlayOneShot(GunSe);
				//銃弾を生成
				Instantiate (bulletPrefab, transform.position,Quaternion.identity);
			}

			//ガンアニメーションをON
			PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTime.ToString (), true);
		} else {
			//銃撃開始フラグをOFFに
			GunTimeInflag = false;
			//GunAnimationをOFF
			PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTime.ToString (), false);
			//弾を打ってない間はインターバルの数値を初期値に
			bulletTime = bulletInterval;
		}

		//Xを押したら爆発するように
		if (Input.GetKeyDown (KeyCode.X)) {
			BomberDamageFlag = true;
		} else {
			//5秒ほど待つようにゲージなどを入れる予定
			BomberDamageFlag = false;
		}
	}

	//プレイヤーのライフ関連の処理
	void PlayerLifeManager(){
		if (PlayerLifeDown) {
			if (PlayerLife > 0) {
				PlayerInvincible = true;
				PlayerLife--;
				Destroy (player_life_clones [PlayerLife]);
				PlayerLifeDown = false;
			} else {
				SceneManager.LoadScene ("OutGame");
			}
		}

		//プレイヤーが無敵状態じゃなければエネミーにぶつかるように
		if (!PlayerInvincible) {
			if (EnemyJudge (Vector2.up, EnemyDistance)) {
				PlayerLifeDown = true;
				PlayerInvincible = true;
			} else if (EnemyJudge (Vector2.right, EnemyDistance)) {
				PlayerLifeDown = true;
				PlayerInvincible = true;
			} else if (EnemyJudge (-Vector2.right, EnemyDistance)) {
				PlayerLifeDown = true;
				PlayerInvincible = true;
			}
		}

		//プレイヤーがダメージ状態であれば
		if (PlayerInvincible) {
			//無敵時間に経過秒数を加算
			InvincibleTime += Time.deltaTime;
			//点滅のための反転秒数を加算
			DamageTime += Time.deltaTime;
			//点滅の時間に入ったか
			if (DamageTime > DamageInterval) {
				//反転秒数をリセット
				DamageTime = 0;
				//現在の表示状態を逆に
				gameObject.GetComponent<Renderer> ().enabled = !gameObject.GetComponent<Renderer> ().enabled;
			}

			//現在の無敵時間が設定された無敵時間以上か
			if (InvincibleTime > InvincibleInterval) {
				//非表示になった場合に確実に表示になるように
				gameObject.GetComponent<Renderer> ().enabled = true;
				//経過時間のリセット
				InvincibleTime = 0;
				//無敵タイムをリセット
				PlayerInvincible = false;
			}
		}
	}

	// 地面に当っているか
	bool GroundJudge( Vector2 direction, float distance ) {
		//プレイヤーのColliderオフセットを取得
		float offsetWidth = PlayerBoxCollidar.offset.x;
		float offsetHeight = PlayerBoxCollidar.offset.y;
		//Colliderのサイズを取得して高さ	の設定
		float PlayerSizeHeight = ( PlayerBoxCollidar.size.y * 2 ) + offsetHeight;
		//BoxColidarの座標の上部下段中央を取得
		Vector2 PlayerUpPosition = new Vector2 ( transform.position.x + offsetWidth, transform.position.y + PlayerSizeHeight );
		Vector2 PlayerUnderPosition = new Vector2 (transform.position.x + offsetWidth, transform.position.y - PlayerSizeHeight);
		Vector2 PlayerCenterPosition = new Vector2 (transform.position.x + offsetWidth, transform.position.y + offsetHeight);

		//上部下段中央の３箇所で同じ方向に向けてRayを飛ばす。
		RaycastHit2D raycastCenter = Physics2D.Raycast( PlayerCenterPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Ground" ) );
		RaycastHit2D raycastUp = Physics2D.Raycast( PlayerUpPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Ground" ) );
		RaycastHit2D raycastUnder = Physics2D.Raycast( PlayerUnderPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Ground" ) );

		//どれか３箇所の内一つでもHitしていればtrueを返す
		if ( raycastCenter || raycastUp || raycastUnder ) {
				return true;
		} else {
			return false;
		}
	}

	//敵に当っているか
	bool EnemyJudge( Vector2 direction, float distance ) {
		//プレイヤーのColliderオフセットを取得
		float offsetWidth = PlayerBoxCollidar.offset.x;
		float offsetHeight = PlayerBoxCollidar.offset.y;
		//Colliderのサイズを取得して高さ	の設定
		float PlayerSizeHeight = ( PlayerBoxCollidar.size.y * 2 ) + offsetHeight;
		//BoxColidarの座標の上部下段中央を取得
		Vector2 PlayerUpPosition = new Vector2 ( transform.position.x + offsetWidth, transform.position.y + PlayerSizeHeight );
		Vector2 PlayerUnderPosition = new Vector2 (transform.position.x + offsetWidth, transform.position.y - PlayerSizeHeight);
		Vector2 PlayerCenterPosition = new Vector2 (transform.position.x + offsetWidth, transform.position.y + offsetHeight);

		//上部下段中央の３箇所で同じ方向に向けてRayを飛ばす。
		RaycastHit2D raycastCenter = Physics2D.Raycast( PlayerCenterPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Enemy" ) );
		RaycastHit2D raycastUp = Physics2D.Raycast( PlayerUpPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Enemy" ) );
		RaycastHit2D raycastUnder = Physics2D.Raycast( PlayerUnderPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "Enemy" ) );

		//どれか３箇所の内一つでもHitしていればtrueを返す
		if ( raycastCenter || raycastUp || raycastUnder ) {
			return true;
		} else {
			return false;
		}
	}

	//落下ゾーンに入ってるかどうか
	bool FallingJudge( Vector2 direction, float distance ) {
		//プレイヤーのColliderオフセットを取得
		float offsetWidth = PlayerBoxCollidar.offset.x;
		float offsetHeight = PlayerBoxCollidar.offset.y;
		//Colliderのサイズを取得して高さ	の設定
		float PlayerSizeHeight = ( PlayerBoxCollidar.size.y * 2 ) + offsetHeight;
		//BoxColidarの座標の上部下段中央を取得
		Vector2 PlayerUpPosition = new Vector2 ( transform.position.x + offsetWidth, transform.position.y + PlayerSizeHeight );
		//上部下段中央の３箇所で同じ方向に向けてRayを飛ばす。
		RaycastHit2D raycastUp = Physics2D.Raycast( PlayerUpPosition, direction, distance, 
			1 << LayerMask.NameToLayer( "FallingZone" ) );

		//どれか３箇所の内一つでもHitしていればtrueを返す
		if ( raycastUp  ) {
			return true;
		} else {
			return false;
		}
	}

}
