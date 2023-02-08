using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyHPManager : MonoBehaviour
{
    //最大HP
    [SerializeField]
    private int maxHP;
    //徐々に減らしていくhp計測に使う
    [SerializeField]
    private int hp;
    //最終的なhp計測に使う
    private int finalHP;
    //HPを一度減らしてからの経過時間
    private float countTime = 0f;
    //次にHPを減らすまでの時間
    [SerializeField]
    private float nextCountTime = 0f;
    //HP表示用スライダー
    private Slider hpSlider;
    //一括HP表示用スライダー
    private Slider bulkHPSlider;
    //現在のダメージ量
    private int damage = 0;
    //一回に減らすダメージ量
    // [SerializeField]
    // private int amountOfDamageAtOneTime = 100;
    //HPを減らしているかどうか
    private bool isReducing;
    //HP用表示スライダーを減らすまでの待機時間
    [SerializeField]
    private float delayTime = 1f;

    // //残機
    // static public int myRemainingAircraftNum;
    // static public int enemyRemainingAircraftNum;

    // GameObject[] txtEnemyRemain;


    static public bool processingCompleted;

    void Start()
    {
        // controller = GetComponent<CharacterController>();
        // if(SceneManager.GetActiveScene().name == "GameScene")
        // {
        //     // //残機
        //     // myRemainingAircraftNum = GameFSceneDirector.unitsNum;
        //     // enemyRemainingAircraftNum = GameFSceneDirector.unitsNum;
        //     //敵の残機
        //     txtEnemyRemain[0] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain1");
        //     txtEnemyRemain[1] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain2");
        //     txtEnemyRemain[2] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain3");
        //     txtEnemyRemain[3] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain4");
        //     txtEnemyRemain[4] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain5");
        //     txtEnemyRemain[5] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain6");
        //     txtEnemyRemain[6] = GameObject.Find("EnemyRemainingAircraft/TextEnemyRemain7");
        // }
        

        hpSlider = transform.Find("Canvas/HPSlider").GetComponent<Slider>();
        bulkHPSlider = transform.Find("Canvas/BulkHPSlider").GetComponent<Slider>();
        hp = maxHP;
        finalHP = maxHP;
        hpSlider.value = 1;
        bulkHPSlider.value = 1;

        processingCompleted = false;
    }

    void Update()
    {
        //ダメージなければ何もしない
        if (!isReducing)
        {
            return;
        }
        //次に減らす時間がきたら
        if (countTime >= nextCountTime)
        {
            int tempDamage;
            //決められた量よりも残りダメージ量が小さければ小さい方を1回のダメージに設定
            tempDamage = Mathf.Min(AttackScript.attackPower, damage);
            hp -= tempDamage;

            //全体の比率を求める
            hpSlider.value = (float)hp / maxHP;

            //全ダメージ量から1回で減らしたダメージ量を減らす
            damage -= tempDamage;

            //全ダメージ量が0より下になったら0を設定
            damage = Mathf.Max(damage, 0);

            countTime = 0f;

            //ダメージがなくなったらHPバーの変更処理をしないようにする
            if(damage <= 0)
            {
                isReducing = false;
            }

            //HPが0以下になったら敵を削除
            if(hp <= 0)
            {
                // Debug.Log(transform.gameObject.tag);
                if(GameSceneDirector.dieUnit != "NucleusWhite" && GameSceneDirector.dieUnit != "NucleusBlack") GameSceneDirector.dieUnit = transform.gameObject.tag;

                if(transform.gameObject.tag == "PlayerB1" || transform.gameObject.tag == "PlayerB2" ||
                   transform.gameObject.tag == "PlayerB3" || transform.gameObject.tag == "PlayerB4" ||
                   transform.gameObject.tag == "PlayerB5" || transform.gameObject.tag == "PlayerB6" ||
                   transform.gameObject.tag == "NucleusBlack")
                {
                    GameSceneDirector.enemyRemainingAircraftNum--;
                    GameSceneDirector.EnemyRemainingAircraftDisplay();
                    // Debug.Log(GameSceneDirector.enemyRemainingAircraftNum);
                    // txtEnemyRemain[GameFSceneDirector.unitsNum - enemyRemainingAircraftNum].GetComponent<Text>().text = "";
                }
                
                // if(transform.gameObject.tag == "PlayerW") myRemainingAircraftNum--;
                // if(GameSceneDirector.nowPlayer == 0 && GameSceneDirector.myRemainingAircraftNum != null) GameSceneDirector.myRemainingAircraftNum--;
                // if(GameSceneDirector.nowPlayer == 1 && GameSceneDirector.enemyRemainingAircraftNum != null) GameSceneDirector.enemyRemainingAircraftNum--;
                Destroy(gameObject);
            }
            else
            {
                if(GameSceneDirector.dieUnit != "NucleusWhite" && GameSceneDirector.dieUnit != "NucleusBlack") GameSceneDirector.dieUnit = "unDestroy";
            }
            if(GameSceneDirector.hpProcessingCompleted == false) GameSceneDirector.hpProcessingCompleted = true;
            //ここで消えなかったらnullを返して、消えたら名前を保管（コア名を優先）
        }
        countTime += Time.deltaTime;
    }

    //ダメージ値を追加するメソッド
    public void TakeDamage(int damage)
    {
        //ダメージを受けた時に一括HP用のバーの値を変更する
        var tempHP = Mathf.Max(finalHP -= damage, 0);
        bulkHPSlider.value = (float)tempHP / maxHP;
        this.damage += damage;
        countTime = 0f;
        //一定時間後にHPバーを減らすフラグを設定
        Invoke("StartReduceHP", delayTime);
    }

    //徐々にHPバーを減らすのをスタート
    public void StartReduceHP()
    {
        isReducing = true;
    }
}
