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

	public int PlayerLife = 3;
	//現在のジャンプ速度
	private float TempSpeed;

	private BoxCollider2D PlayerBoxCollidar;
	private Animator PlayerAnimator;
	public Canvas canvas;
	public GameObject TargetCanvas;
	public GameObject bulletPrefab;
	public GameObject LifePrefab;
	public GameObject PlayerSpawnPoint;
	public GameObject EndPrefab;
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
	[SerializeField]
	//プレイヤーが死亡状態か
	private bool PlayerDeath = false;

	//プレイヤーから地面までの下の距離
	private float groundUnderDistance = 0.1f;
	//プレイヤーが落下までの距離
	private float FalingDistance = 0.1f;
	//プレイヤーから地面までの左までの距離
	private float groundLeftDistance = 0.3f;
	//プレイヤーから地面までの右までの距離
	private float groundRightDistance = 0.3f;

	private float EnemyDistance = 0.2f;

	//無敵の時間
	private float InvincibleTime = 0;
	private float InvincibleInterval = 3.0f;

	//ダメージ状態の演出時間
	private float DamageTime = 0;
	private float DamageInterval = 0.1f;

	private float bulletTime = 0;
	//次弾が発射できるまでの間隔
	private float bulletInterval = 0.15f;

	//ライフの横幅
	private const int LifeWidth = 40;

	//爆発カウント
	public const int BomberStackMax = 3;

	public bool BomberDamageFlag = false;

	//SE
	public AudioClip GunSe;
	public AudioClip DamageSe;
	//オーディオを再生するためのコンポーネントを取得
	private AudioSource PlayerAudio;

	private enum PlayerAnimatorParameters
	{
		GunTime,
		PlayerJumpup,
		PlayerFaling,
		GunTimeIn,
		PlayerWalk,
		PlayerDeath
	}


	// Use this for initialization
	void Start () {
		//オブジェクト取得
		PlayerBoxCollidar = gameObject.GetComponent<BoxCollider2D> ();
		PlayerAnimator = gameObject.GetComponent<Animator> ();
		TempSpeed = JumpSpeed;
		PlayerAudio = gameObject.GetComponent<AudioSource> ();
		//プレイヤーライフの数を取得
		player_life_clones = new GameObject[PlayerLife];
		//ライフをUIにセット
		for ( int i = 0; i < PlayerLife; i++ ) {
			player_life_clones[i] = Instantiate( LifePrefab ) as GameObject;
			var pos = player_life_clones [i].GetComponent<RectTransform> ().position;
			pos.x += i * LifeWidth;
			player_life_clones[i].GetComponent<RectTransform> ().position = pos;
			player_life_clones[i].transform.SetParent( canvas.transform,false );
		}
			
	}
	
	// Update is called once per frame
	void Update ( ) {
		//カウントダウン中は動けないように
		if (CountDawn.CountDawnflag) {
			Move ();
			Attack ();
			PlayerLifeManager ();
		}
		//落下処理をしているジャンプ関数と地面処理をしているgroundのみ常に回す
		Jump ();
		Ground ();
	}

	void Move(){
		//左を入力して地面に触れていなければ左に移動
		if (Input.GetKey (KeyCode.LeftArrow)) {
			if (!GroundJudge (-Vector2.right, groundLeftDistance)) {
				if (!turnFlag) {
					transform.Rotate (0, 180, 0);
					turnFlag = true;
				}
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerWalk.ToString (), true);
				transform.Translate (Vector2.right * PlayerSpeed);
			}
		}
		//右を入力して地面に触れていなければ右に移動
		else if (Input.GetKey (KeyCode.RightArrow)) {
			if (!GroundJudge (Vector2.right, groundRightDistance)) {
				if (turnFlag) {
					transform.Rotate (0, 180, 0);
					turnFlag = false;
				}
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerWalk.ToString (), true);
				transform.Translate (Vector2.right * PlayerSpeed);
			}
		} else {
			PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerWalk.ToString (), false);
		}

	}

	void Ground(){
		//地面に触れていてジャンプ中でなければ落下フラグをONに
		if ( GroundJudge ( -Vector2.up, groundUnderDistance ) && !isJump ) {
			isGround = true;
		} else {
			isGround = false;
		}

		//落下時判定
		if( FallingJudge( -Vector2.up, FalingDistance ) ){
			PlayerLifeDown = true;
			transform.position = PlayerSpawnPoint.GetComponent<Transform> ().position;
		}
	}

	void Jump( ){
		//ジャンプキーが押されて地面に触れていて死亡状態でなければジャンプフラグONに
		if ( Input.GetKey ( KeyCode.C ) && isGround && !PlayerDeath ) {
			isJump = true;
		}
		//ジャンプフラグがONになっていなければジャンプ。
		//OFFになっていれば現在のジャンプ速度にジャンプ速度を代入。
		if ( isJump ) {
			//上昇アニメーション
			PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerJumpup.ToString (), true);
			transform.Translate ( Vector2.up * TempSpeed);
			TempSpeed -= JumpFailingSpeed;
			//現在のスピードが0以下になったらジャンプフラグをOff
			if (TempSpeed < 0) {
				isJump = false;
				//上昇アニメーションをオフ
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerJumpup.ToString (), false);
			}
		} else { 
			//現在の速度を初速度
			TempSpeed = JumpSpeed;
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
	}

	//攻撃処理
	void Attack(){
		if (Input.GetKey (KeyCode.Z) ) {
			//銃撃を始めたのが最初ならtrueに
			if (!GunTimeInflag) {
				PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTimeIn.ToString (), true);
				GunTimeInflag = true;
			} else {
				PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTimeIn.ToString (), false);
			}
			//攻撃時間を加算
			bulletTime += Time.deltaTime;
			//攻撃時間がインターバルより長くなってれば
			if (bulletTime > bulletInterval) {
				//攻撃時間をリセット
				bulletTime = 0.0f;
				//SE再生
				PlayerAudio.PlayOneShot(GunSe);
				//弾を生成
				Instantiate (bulletPrefab, transform.position,Quaternion.identity);
			}

			//ガンアニメーションをON
			PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTime.ToString (), true);
		} else {
			GunTimeInflag = false;
			//GunAnimationをOFF
			PlayerAnimator.SetBool (PlayerAnimatorParameters.GunTime.ToString (), false);

		}
		//Xキーを押すと爆発処理が起きるように
		if (Input.GetKeyDown (KeyCode.X)) {
			BomberDamageFlag = true;
		} else {
			BomberDamageFlag = false;
		}
	}

	//プレイヤーのライフ関連の処理
	void PlayerLifeManager(){
		if (PlayerLifeDown) {
			if (PlayerLife > 0) {
				//SE再生
				PlayerAudio.PlayOneShot(DamageSe);
				//プレイヤーを無敵状態に
				PlayerInvincible = true;
				//プレイヤーライフを下げる
				PlayerLife--;
				Destroy (player_life_clones [PlayerLife]);
				PlayerLifeDown = false;
			} else {
				//死亡状態にして死亡アニメーションの再生
				PlayerDeath = true;
				PlayerAnimator.SetBool (PlayerAnimatorParameters.PlayerDeath.ToString (), true);
				//点滅途中であれば表示されるように
				gameObject.GetComponent<Renderer> ().enabled = true;
				//カウントダウン状態に
				CountDawn.CountDawnflag = false;
				//終了処理
				GameObject EndObj = (GameObject)Instantiate (EndPrefab);
				EndObj.transform.SetParent (TargetCanvas.transform, false);
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
			InvincibleTime += Time.deltaTime;
			DamageTime += Time.deltaTime;
			if (DamageTime > DamageInterval) {
				DamageTime = 0;
				gameObject.GetComponent<Renderer> ().enabled = !gameObject.GetComponent<Renderer> ().enabled;
			}

			if (InvincibleTime > InvincibleInterval) {
				gameObject.GetComponent<Renderer> ().enabled = true;
				InvincibleTime = 0;
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
