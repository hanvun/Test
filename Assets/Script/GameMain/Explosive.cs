/*
 爆発に関する処理を行います。
 近くにいるエネミーレイヤーのタグのライフを0にする処理をします
*/

using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour
{

    //エネミーまでの距離設定
    private float groundUnderDistance = 1.5f;

    //レイキャスト生成数
    private int RayCastMaxGanerate = 4;

    //Rayの方向を設定
    private Vector2 ExplosiveDirection;

    //chainSystemを取得
    private ChainSystem chainSystem;

    //最初の連鎖爆発か
    private bool FristChain = true;

    void Awake()
    {
        GameObject ChainSystemObj = GameObject.Find("ChainSystem");
        chainSystem = ChainSystemObj.GetComponent<ChainSystem>();
    }

    void Start()
    {
        //生成時に爆発開始時の周りをチェックして爆発させます。
        Engulfment_explosive();
    }

    void Update()
    {
    }

    //アニメーション終了時に実行します
    void AnimDestory()
    {
        Destroy(gameObject);
    }

    //周囲のエネミーを爆発させる
    void Engulfment_explosive()
    {
        //レイを設定数分生成
        for (int i = 0; i < RayCastMaxGanerate; i++)
        {
            //ループ回数でベクトルを上下左右に変更
            switch (i)
            {
                case 0:
                    //右にレイを設定
                    ExplosiveDirection = Vector2.right;
                    break;
                case 1:
                    //左にレイを設定
                    ExplosiveDirection = -Vector2.right;
                    break;
                case 2:
                    //下にレイを設定
                    ExplosiveDirection = -Vector2.up;
                    break;
                case 3:
                    //上にレイを設定
                    ExplosiveDirection = Vector2.up;
                    break;
            }
            //エネミーのみに当たるレイキャストを設定
            RaycastHit2D Raycast = Physics2D.Raycast(transform.position, ExplosiveDirection,
                groundUnderDistance, 1 << LayerMask.NameToLayer("Enemy"));
            //レイに当っていたら
            if (Raycast)
            {
                if (FristChain)
                {
                    //チェイン数を加算
                    chainSystem.ChainAdd();
                    FristChain = false;
                }
                //あたったレイの最初の爆発フラグをOFFに
                Raycast.collider.gameObject.GetComponent<Enemy>().FristBomberFlag = false;
                //当たったレイのエネミーライフを0に
                Raycast.collider.gameObject.GetComponent<Enemy>().EnemyLife = 0;
            }
        }
    }

}
