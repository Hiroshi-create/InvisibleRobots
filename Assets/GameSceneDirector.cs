using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;

public class GameSceneDirector : MonoBehaviourPunCallbacks
{
    //ゲーム設定
    public const int TILE_X = 9;
    public const int TILE_Z = 9;
    const int PLAYER_MAX = 2;

    private int unitsAttack = 334;  //<=
    private int coreUnitsAttack = 400;  //<=
    // //ユニットの数
    // private int numberOfUnits = 7;

    //タイルのプレハブ
    public GameObject[] prefabTile;
    //カーソルのプレハブ
    public GameObject prefabCursor;
    public GameObject prefabUnitsCursor;
    public GameObject coreUnitCursor;
    //Diceのプレハブ
    public GameObject prefabDice;
    // static public GameObject dice;

    // private float speed = 0.5f;
    [SerializeField] Transform target;
    [SerializeField] List<GameObject> targets;


    //移動後の座標を共有するため
    static public int beforeMoving_X;
    static public int beforeMoving_Z;


    //移動後の座標を共有するため
    static public int moved_X;
    static public int moved_Z;

    //残機
    static public int myRemainingAircraftNum;
    static public int enemyRemainingAircraftNum;
    static public int enemyRemainingAircraftNumCopy;

    //バトル開始時のみアニメーションをするため
    private bool battleStart;
    // private bool selectOK;
    //
    static public bool diceModeStop;
    static public bool diceModeOneLoop;
    private bool unitsCursorsOneLoop;

    //バトル開始時にHPバーを見えない様にする
    static public bool startHPSlider; 
    // //攻撃
    // private bool attackable;
    //倒されたユニット名
    static public string dieUnit;

    //バトル開始アニメーションのワンループ
    public bool startAnimationOneRoop;

    //移動完了したか
    public bool moveGradually;
    public bool oneLoop;
    //HPの処理が完了したか
    static public bool hpProcessingCompleted; 
    //移動時の体の向き
    private Vector3 latestPos;
    // static public bool moveCompleted;
    // static public Vector2 unitposFloat;
    // static public Vector2 tileposFloat;
    // //オンライン戦かどうか
    // public bool IsOnline {get; set;}  //{外部から取得できます; 外部からセットできます;}

    //内部データ
    GameObject[,] tiles;
    GameObject[,] hpSliders;
    UnitController[,] units;

    //ユニットのプレハブ（色ごと）
    public List<GameObject> prefabWhiteUnits;
    public List<GameObject> prefabBlackUnits;

    // // 1 = ポーン 2 = ルーク 3 = ナイト 4 = ビショップ 5 = クイーン 6 = キング
    // static public int[,] unitType =
    // { //左が手前側でコマを配置する
    //     // {0, 0, 0, 0, 0, 0, 0, 0, 0},
    //     // {0, 1, 0, 0, 0, 0, 0, 0, 0},
    //     // {0, 2, 0, 0, 0, 0, 0, 16, 0},
    //     // {0, 3, 0, 0, 0, 0, 0, 15, 0},
    //     // {0, 4, 0, 0, 0, 0, 0, 14, 0},
    //     // {0, 5, 0, 0, 0, 0, 0, 13, 0},
    //     // {0, 6, 0, 0, 0, 0, 0, 12, 0},
    //     // {0, 0, 0, 0, 0, 0, 0, 11, 0},
    //     // {0, 0, 0, 0, 0, 0, 0, 0, 0},

    //     {0, 0, 0, 0, 0, 0, 0, 0, 0},
    //     {0, 1, 0, 0, 0, 0, 0, 11, 0},
    //     {0, 1, 0, 0, 0, 0, 0, 16, 0},
    //     {0, 1, 0, 0, 0, 0, 0, 11, 0},
    //     {0, 1, 0, 0, 0, 0, 0, 11, 0},
    //     {0, 1, 0, 0, 0, 0, 0, 11, 0},
    //     {0, 6, 0, 0, 0, 0, 0, 11, 0},
    //     {0, 1, 0, 0, 0, 0, 0, 11, 0},
    //     {0, 0, 0, 0, 0, 0, 0, 0, 0},
    // };

    //UI関連
    GameObject txtBattleStart;  //<=
    GameObject txtTurnInfo;
    GameObject txtTurnInfoDelay;  //<=
    GameObject txtResultInfo;
    // GameObject txtMyRemainingAircraft;
    static public GameObject[] txtEnemyRemain;
    GameObject btnApply;
    GameObject btnCancel;
    GameObject btnTitle;
    GameObject turnInfoPanel;
    GameObject movePatternPanel;
    GameObject diceNumUI;
    GameObject enemyRemainingAircraftUI;
    GameObject frameRemainingAircraftUI;
    
    // Slider slider;

    //選択中のユニット
    UnitController selectUnit;

    //移動関連
    List<Vector2Int> movableTiles;
    // List<Vector2Int> movableHpSliders;///
    List<GameObject> cursors;
    List<GameObject> unitsCursors;

    //モード
    enum MODE
    {
        NONE,
        CHECK_MATE,
        DICE,
        NORMAL,
        STATUS_UPDATE,
        TURN_CHANGE,
        RESULT,
    }

    MODE nowMode, nextMode;
    static public int nowPlayer;

    private int player;
    private int type;

    // //攻撃
    // static public bool inAttackRange;
    // private Camera mainCamera;
    // [SerializeField]
    // private int attackPower = 500;

    //ゲーム開始からの経過ターン
    int prevTurn;
    //前回ユニット削除からの経過ターン
    int prevDestroyTurn;

    //前回の盤面
    List<UnitController[,]> prevUnits;

    //毎ターンランダムで行動パターンを付与する為
    static public UnitController.TYPE selectedBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        battleStart = false;
        startHPSlider = false;
        diceModeOneLoop = false;
        unitsCursorsOneLoop = false;
        diceModeStop = false;
        startAnimationOneRoop = false;
        dieUnit = "battleStart";
        hpProcessingCompleted = false;
        AnimationManager.onAttackAnimation = false;
        enemyRemainingAircraftNumCopy = GameFSceneDirector.unitsNum;
        // DiceDuplication.returnDiceMode = false;
        // selectOK = false;
        // AnimationManager.nextTurn = true;
        // moveCompleted = false;

        //UIオブジェクトの取得
        txtBattleStart = GameObject.Find("TextBattleStart");  //<=
        txtTurnInfo = GameObject.Find("TextTurnInfo");
        txtTurnInfoDelay = GameObject.Find("TextTurnInfoDelay");  //<=
        txtResultInfo = GameObject.Find("TextResultInfo");
        // txtMyRemainingAircraft = GameObject.Find("TextMyRemainingAircraft");
        //敵の残機
        txtEnemyRemain = new GameObject[GameFSceneDirector.unitsNum];
        txtEnemyRemain[0] = GameObject.Find("TextEnemyRemain1");
        txtEnemyRemain[1] = GameObject.Find("TextEnemyRemain2");
        txtEnemyRemain[2] = GameObject.Find("TextEnemyRemain3");
        txtEnemyRemain[3] = GameObject.Find("TextEnemyRemain4");
        txtEnemyRemain[4] = GameObject.Find("TextEnemyRemain5");
        txtEnemyRemain[5] = GameObject.Find("TextEnemyRemain6");
        txtEnemyRemain[6] = GameObject.Find("TextEnemyRemain7");

        btnApply = GameObject.Find("ButtonApply");
        btnCancel = GameObject.Find("ButtonCancel");
        btnTitle = GameObject.Find("ButtonTitle");
        turnInfoPanel = GameObject.Find("TurnInfoPanel");
        movePatternPanel = GameObject.Find("MovePatternPanel");
        diceNumUI = GameObject.Find("DiceNum");
        enemyRemainingAircraftUI = GameObject.Find("EnemyRemainingAircraft");
        frameRemainingAircraftUI = GameObject.Find("FrameRemainingAircraft");

        // slider = GameObject.Find("HPSlider").GetComponent<Slider>();

        // //スライダーのゲージを１にする
        // slider.value = 1;

        //リザルト系は消しておく
        btnApply.SetActive(false);
        btnCancel.SetActive(false);
        btnTitle.SetActive(false);
        turnInfoPanel.SetActive(false);
        movePatternPanel.SetActive(false);
        diceNumUI.SetActive(false);
        enemyRemainingAircraftUI.SetActive(false);
        frameRemainingAircraftUI.SetActive(false);

        //移動関連
        cursors = new List<GameObject>();  //初期化
        //ユニットに常駐でつけるカーソル
        unitsCursors = new List<GameObject>();  //初期化

        //内部データ
        tiles = new GameObject[TILE_X, TILE_Z];
        units = new UnitController[TILE_X, TILE_Z];
        prevUnits = new List<UnitController[,]>();
        movableTiles = new List<Vector2Int>();

        // hpSliders = new UnitController[TILE_X, TILE_Z];///
        // prevHpSliders = new List<UnitController[,]>();///
        // movableHpSliders = new List<Vector2Int>();///

        // surfaceBordRead();

        //ユニット番号を反映させるため
        int unitNumberW = 0;
        int unitNumberB = 0;

        //残機
        myRemainingAircraftNum = GameFSceneDirector.unitsNum;
        enemyRemainingAircraftNum = GameFSceneDirector.unitsNum;

        int targetIdx = 0;

        for(int i = 0; i < TILE_X; i++)
        {
            for(int j = 0; j < TILE_Z; j++)
            {
                GameObject prefab;

                float x = i - TILE_X / 2; //真ん中に来るように引いて調節
                float y = j - TILE_Z / 2; //上に同じ

                Vector3 pos = new Vector3(x, 0, y);
                // Vector3 hpPos = new Vector3(x, 5, y);

                //作成
                // int idx = (i + j) % 2;
                int idx = 0;
                GameObject tile = Instantiate(prefabTile[idx], pos, Quaternion.identity);

                tiles[i, j] = tile;

                Vector3 startTargetPos = new Vector3(x, 0.82f, y);
                Vector3 unitsPos = new Vector3(x, 50f, y);

                pos.y = 100.5f;

                //ユニットの作成
                type   = GameFSceneDirector.unitType[i, j] % 10;  //最初に指定された盤面から初期位置をこれで読み取る
                player = GameFSceneDirector.unitType[i, j] / 10;

                //通常ユニットをそれぞれ差別化
                if(player == 0 && type == 2)
                {
                    prefab = getPrefubUnit(player, type + unitNumberW);
                    // Debug.Log(unitNumberW);
                    unitNumberW++;
                }
                else if(player == 1 && type == 2)
                {
                    prefab = getPrefubUnit(player, type + unitNumberB);
                    unitNumberB++;
                }
                else
                {
                    prefab = getPrefubUnit(player, type);
                }

                if(prefab != null)
                {
                    targetIdx++;
                }


                GameObject unit = null;
                UnitController ctrl = null;
                // GameObject hpSlider = null;///
                // UnitController gauge = null;///

                if(null == prefab) continue;  //prefabが０つまりnullになることもあるのでnullチェックしてこれ以上先に行かないようにする

                // Debug.Log(targetIdx);
                GameObject targetPrefab = Instantiate(targets[targetIdx-1], startTargetPos, Quaternion.identity);
                unit = Instantiate(prefab);
                // hpSlider = Instantiate(prefab);///
                
                //盤面にセットされたらゲーム開始のアニメーション
                // StartCoroutine(StartGradually(unit, tilepos, unitpos, moveGradually));

                //初期設定
                ctrl = unit.GetComponent<UnitController>();
                ctrl.SetUnit(player, (UnitController.TYPE)type, tile);
                // gauge = hpSlider.GetComponent<UnitController>();///
                // gauge.SetUnit(player, (UnitController.TYPE)type, tile);///

                ctrl.StartPos();

                //内部データをセット
                units[i,j] = ctrl;
                // hpSliders[i,j] = gauge;///
            }
        }

        //初期モード
        nowPlayer = -1;
        nowMode = MODE.NONE;
        nextMode = MODE.TURN_CHANGE;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(battleStart == false && startAnimationOneRoop == false)
        {
            //バトル開始時の登場エフェクトアニメーションを開始
            StartCoroutine(AnimationReset());
            // battleStart = true;
            startAnimationOneRoop = true;
            // Debug.Log(AnimationManager.onGameStartAttackAnimation);
        }
        else if(battleStart == true && startAnimationOneRoop == true)
        {
            if(battleStart == true) txtBattleStart.GetComponent<Text>().text = "";  //<=
            if(unitsCursorsOneLoop == false) setUnitsCursors();
            // Debug.Log(battleStart + "    毎ターン頭");
            if(MODE.CHECK_MATE == nowMode)
            {
                if(dieUnit != null)
                {
                    checkMateMode();
                }
                // else
                // {
                //     Debug.Log("null");
                //     // nextMode = MODE.CHECK_MATE;
                // }
            }
            else if(MODE.DICE == nowMode)
            {
                // if()
                // StartCoroutine(diceMode());
                diceMode();
                
                // slider.value -= 0.1f;
            }
            else if(MODE.NORMAL == nowMode)
            { 
                // if(selectOK == true)
                // {
                    normalMode();
                // }
                
            }
            else if(MODE.STATUS_UPDATE == nowMode)
            {
                if(TitleSceneDirector.IsOnline == false)
                {
                    statusUpdateMode();
                }
                if(TitleSceneDirector.IsOnline == true)
                {
                    photonView.RPC(nameof(RPCstatusUpdateMode), RpcTarget.All);
                }
            }
            else if(MODE.TURN_CHANGE == nowMode)
            {
                turnChangeMode();
            }

            //モード変更
            if(MODE.NONE != nextMode)
            {
                nowMode = nextMode;
                nextMode = MODE.NONE;
            }
        }
    }

    //ターン開始時のモード
    void checkMateMode()
    {
        // Debug.Log(dieUnit + "    チェックメイトの頭");
        // //次のモード
        // nextMode = MODE.DICE;
        Text info = txtResultInfo.GetComponent<Text>();
        info.text = "";

        // Debug.Log(nowPlayer);

        
        
        // if(myRemainingAircraftNum == GameFSceneDirector.unitsNum && myRemainingAircraftNum)
        // {

        // }
        // txtResultInfo.GetComponent<Text>().text = "◆" * 
        

        //勝敗を決める
        if(null == getUnit(nowPlayer))  //TagのNucleusがいなかったら
        {
            if(getNextPlayer() == 0)
            {
                info.color = new Color(0.25f, 0.79f, 0.5f);
                info.text = "コアを破壊しました \nW I N N E R ! !";
            }
            else if(getNextPlayer() == 1)
            {
                info.color = new Color(0.79f, 0.25f, 0.4f);
                info.text = "コアが破壊されました \nL O S E...";
            }
            
            nextMode = MODE.RESULT;
        }
        //１ターン目 & 登場アニメーションが終わったら次へ
        else if(dieUnit != null && dieUnit == "battleStart")
        {
            //次のモード
            nextMode = MODE.DICE;
        }
        //２ターン目以降のバトル継続の確認 & HPの変動が完了したら
        else if(dieUnit != null && dieUnit != "battleStart" && hpProcessingCompleted == true)
        {
            //次のモード
            nextMode = MODE.DICE;
        }

        // Debug.Log(dieUnit + "    チェックメイトの２");

        //---------------------------------
        //ドローのチェック（簡易版）
        //---------------------------------
        // 1 vs 1 になったら引き分け
        //ユニットの組み合わせでチェックメイトできない状態がある
        if(3 > getUnits().Count)  //全部のユニットが３未満だったら
        {
            info.text = "---- D R A W ----";
            nextMode = MODE.RESULT;
        }

        //今回の盤面をコピー
        UnitController[,] copyunits = new UnitController[units.GetLength(0), units.GetLength(1)];
        Array.Copy(units, copyunits, units.Length);  //Array.Copy(コピー元, コピー先, サイズ)
        prevUnits.Add(copyunits);

        //次のモードの準備
        if(MODE.RESULT == nextMode)
        {
            btnApply.SetActive(true);
            btnCancel.SetActive(true);
        }
        

        // //次のモード
        // nextMode = MODE.DICE;
    }

    //１から６までの数をランダムに出力
    void diceMode()
    {
        // Debug.Log(dieUnit + "    " + hpProcessingCompleted + "    ダイスの頭");
        
        //サイコロの目が読み込まれるまでサイコロを表示させる処理をさせない
        if(diceModeOneLoop == false && (hpProcessingCompleted == true || dieUnit == "battleStart")/* && DiceDuplication.returnDiceMode == true*/)
        {
            // Debug.Log(dieUnit + "    ダイスが呼ばれる");
            //サイコロを表示させる初期位置
            Vector3 pos = new Vector3(0, 7, -3);
            //サイコロのprefabを呼び出す
            GameObject dice = Instantiate(prefabDice, pos, Quaternion.identity);
            diceModeOneLoop = true;
            // dieUnit = null;
        }

        //サイコロの目が読み込まれたら次のモードへ
        if(diceModeStop == true)
        {
            hpProcessingCompleted = false;
            diceModeOneLoop = false;
            diceModeStop = false;
            nextMode = MODE.NORMAL;
        }
    }


    void normalMode()
    {
        GameObject tile = null;
        UnitController unit = null;

        //プレイヤー
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //  TODO  全てのオブジェクト情報を取得   敵ユニットは選択できないようにする
            foreach(RaycastHit hit in Physics.RaycastAll(ray))
            {
                if(hit.transform.name.Contains("Tile"))
                {
                    tile = hit.transform.gameObject;
                    break;
                }
            }
        }

        //CPU
        //PlayerCountがプレイヤー番号を越えていたら、selectUnitがnullの時かtileがnullの時     ループする
        while(TitleSceneDirector.PlayerCount <= nowPlayer && (null == selectUnit || null == tile) && oneLoop == false /*       && null != unit && selectUnit != unit && nowPlayer == unit.Playe*/)
        {
            //ユニット選択   一回通る
            if(null == selectUnit)
            {
                //今回の全ユニット
                List<UnitController> tmpunits = getUnits(nowPlayer);
                //ランダムで一体選ぶ
                UnitController tmp = tmpunits[Random.Range(0, tmpunits.Count)];
                //ユニットがいるタイルを選択
                tile = tiles[tmp.Pos.x, tmp.Pos.y];

                //一旦処理を流すためにbreak
                break;
            }

            //ここからはselectUnitが入った状態で返ってくる
            if(1 > movableTiles.Count)
            {
                //移動できないユニットを選んでしまった場合もう１回
                setSelectCursors();
                break;
            }

            ///ここに到達すればselectUnitとmovableTilesにデータが入っている
            //移動可能範囲があればランダムで移動
            int rnd = Random.Range(0, movableTiles.Count);
            tile = tiles[movableTiles[rnd].x, movableTiles[rnd].y];
            oneLoop = true;
        }

        //タイルが押されていなければ処理しない
        if(null == tile) return;

        

        //選んだタイルからユニット取得
        Vector2Int tilepos = new Vector2Int((int)tile.transform.position.x + TILE_X / 2, (int)tile.transform.position.z + TILE_Z / 2);

        //タイルに乗っているユニット
        unit = units[tilepos.x, tilepos.y];

        //ユニットを選択
        if(null != unit && selectUnit != unit && nowPlayer == unit.Player)  //自分のターンしか触れないようにする
        {
            //攻撃・移動アニメーション用  移動前の座標
            beforeMoving_X = (int)tile.transform.position.x;
            beforeMoving_Z = (int)tile.transform.position.z;
            AnimationManager.posCheckAttackAnimation = true;

            unit.Type = selectedBehaviour; // <- Add
            // Debug.Log($"SELECTED: {selectedBehaviour}"); // <- Add
            //移動可能範囲をセット
            List<Vector2Int> tiles = getMovableTiles(unit);
            // foreach(Vector2Int v in tiles)
            // {
            //     Debug.Log(v);
            // }

            //選択不可能
            if(1 > tiles.Count) return;  //タイルが一つもなかったら選択不可
            if(nowPlayer == 0)
            {
                if(GameFSceneDirector.myPlayerNumber != unit.Player) return;  //常に相手ユニットは選択不可
            }

            movableTiles = tiles;
            setSelectCursors(unit);
        }
        // else if(null != unit && selectUnit != unit && nowPlayer == unit.Player)
        // {

        // }
        //移動     ループする
        else if(null != selectUnit && movableTiles.Contains(tilepos))
        {
            //ループしてる
            //攻撃用 移動先の座標
            moved_X = tilepos.x;
            moved_Z = tilepos.y;

            moveUnit(selectUnit, tilepos);  //選んでいるユニットと場所を渡す
        }
    }

    //移動後の処理
    void statusUpdateMode()
    {
        oneLoop = false;
        // //見えるか否か
        // if(selectUnit.Player == 0 && selectUnit != null) UnitDistance.changeTransparencyMT = true;

        // if(selectUnit.Player == 1) UnitDistance.changeTransparencyMT = true;
        // if(selectUnit.Player == 1) UnitDistance.changeTransparencyYT = true;
        // Debug.Log(selectUnit.Player);


        //攻撃・動かしたユニットの四方に敵ユニットがいるか
        /*if(attackable == true) */AttackScript.inAttackRange = true;

        //ターン経過
        foreach(var v in getUnits(nowPlayer))
        {
            v.ProgressTurn();
        }

        // AnimationManager.nextTurn = false;

        // //カーソルを消す
        // setSelectCursors();
        if(unitsCursorsOneLoop == false) setUnitsCursors();  //<=

        nextMode = MODE.TURN_CHANGE;
    }

    [PunRPC]
    private void RPCstatusUpdateMode()
    {
        //ターン経過
        foreach(var v in getUnits(nowPlayer))
        {
            v.ProgressTurn();
        }

        //カーソルを消す
        setSelectCursors();

        nextMode = MODE.TURN_CHANGE;
    }

    //相手のターン変更
    void turnChangeMode()
    {
        //ターンの処理
        nowPlayer = getNextPlayer();
        AttackScript.nowPlayer = nowPlayer;

        //Info更新 オンライン戦かその他かでRPCを使用  //<=
        if(TitleSceneDirector.IsOnline == false)
        {
            txtTurnInfo.GetComponent<Text>().text = "YOUR turn";
            txtTurnInfoDelay.GetComponent<Text>().text = "YOUR turn";
            txtTurnInfoDelay.GetComponent<Text>().color = new Color(0.25f, 0.79f, 0.5f);
            if(nowPlayer == 1) 
            {
                txtTurnInfo.GetComponent<Text>().text = "Wait for Move Completed";
                txtTurnInfoDelay.GetComponent<Text>().text = "Wait for Move Completed";
                txtTurnInfoDelay.GetComponent<Text>().color = new Color(0.79f, 0.25f, 0.4f);
            }
        }
        if(TitleSceneDirector.IsOnline == true)
        {
            photonView.RPC(nameof(RpcTurnMessage), RpcTarget.All, "Wait for Move Completed");
            if(nowPlayer == 1) photonView.RPC(nameof(RpcTurnMessage), RpcTarget.All, "YOUR turn");
        }

        //経過ターン（１P側に来たら＋１）
        if(0 == nowPlayer)
        {
            prevDestroyTurn++;
            prevTurn++;
        }

        unitsCursorsOneLoop = false;

        nextMode = MODE.CHECK_MATE;
    }

    //オンライン戦 ターンのテキストの同期
    [PunRPC]
    private void RpcTurnMessage(string message) {
        txtTurnInfo.GetComponent<Text>().text = message;
        txtTurnInfoDelay.GetComponent<Text>().text = message;  //<=
    }


    //次のプレイヤー番号を取得
    int getNextPlayer()
    {
        int next = nowPlayer + 1;
        if(PLAYER_MAX <= next) next = 0;

        return next;
    }

    //コアユニットが残っているか探す
    UnitController getUnit(int player)
    {
        foreach(var v in getUnits(player))
        {
            //ターン開始時に移動させる側のユニットをチェック
            if(player != v.Player) continue;
            // if(nucleusWhite == v.transform.gameObject.tag || nucleusBlack == v.transform.gameObject.tag) return v;
            // Debug.Log(dieUnit);
            

            if(dieUnit != "NucleusWhite" && dieUnit != "NucleusBlack")
            {
                // Debug.Log("in");
                return v;
            }
            else 
            {
                // Debug.Log("勝敗     " + dieUnit);
                return null;
            }
        }
        // Debug.Log(dieUnit);
        // Debug.Log("~~~DIE~~~");
        return null;
    }

    //指定されたプレイヤーのユニットを取得
    List<UnitController> getUnits(int player = -1)
    {
        List<UnitController> ret = new List<UnitController>();

        foreach(var v in units)
        {
            if(null == v) continue;

            if(player == v.Player)
            {
                ret.Add(v);
            }
            else if(0 > player)
            {
                ret.Add(v);
            }
        }
        return ret;
    }

    //移動可能範囲の取得
    List<Vector2Int> getMovableTiles(UnitController unit)
    {
        //--------------------------------------------------------------------------
        //そこをどくとチェックされてしまうか
        UnitController[,] copyunits = new UnitController[units.GetLength(0), units.GetLength(1)];
        Array.Copy(units, copyunits, units.Length);  //Array.Copy(コピー元, コピー先, サイズ)
        copyunits[unit.Pos.x, unit.Pos.y] = null;

        //チェックされているかどうか
        List<UnitController> checkunits = GetCheckUnits(copyunits, unit.Player);

        //チェックを回避できるタイルを返す
        if(0 < checkunits.Count)
        {
            //移動可能タイル
            List<Vector2Int> ret = new List<Vector2Int>();

            //移動可能範囲
            List<Vector2Int> movetilts = unit.GetMovableTiles(units);

            //チェック中のユニットを邪魔できる場所を探す
            foreach(var v in movetilts)
            {
                //移動した状態を作る
                UnitController[,] copyunits2 = new UnitController[units.GetLength(0), units.GetLength(1)];
                Array.Copy(units, copyunits2, units.Length);
                //移動してみる
                copyunits2[unit.Pos.x, unit.Pos.y] = null;
                copyunits2[v.x, v.y] = unit;

                int checkcount = GetCheckUnits(copyunits2, unit.Player, false).Count;

                if(1 > checkcount) ret.Add(v);
            }

            return ret;
        }
        //--------------------------------------------------------------------------
        //通常移動可能範囲を返す
        return unit.GetMovableTiles(units);
    }

    //ユニットが指定されていたらセットして、指定されていなかったらカーソルを全て消す
    void setSelectCursors(UnitController unit = null, bool setunit = true)
    {
        //カーソル解除
        foreach(var v in cursors)
        {
            Destroy(v);
        }

        //選択ユニットの非選択状態
        if(null != selectUnit)  //すでに選択していた場合
        {
            selectUnit.SelectUnit(false);  //セレクトしているユニットのフォーカスを外す
            selectUnit = null;
        }

        //なにもセットしていないなら終わり
        if(null == unit) return;

        //カーソル作成
        if(GameFSceneDirector.myPlayerNumber == unit.Player)
        {
            foreach(var v in getMovableTiles(unit))
            {
                Vector3 pos = tiles[v.x, v.y].transform.position;
                pos.y += 0.51f;
                GameObject obj = Instantiate(prefabCursor, pos, Quaternion.identity);
                cursors.Add(obj);
            }
        }

        //選択状態
        if(setunit)
        {
            selectUnit = unit;
            selectUnit.SelectUnit(setunit);
        }
    }

    //ユニットの周囲にカーソルを配置して、移動したら元のカーソルを消して移動先にカーソルを配置
    void setUnitsCursors()
    {
        GameObject tileCursor = null;
        UnitController unitCursor = null;
        GameObject obj;  //<=

        //カーソル解除
        foreach(var v in unitsCursors)
        {
            Destroy(v);
        }

        //今回の全ユニット
        List<UnitController> tmpunitsCursors = getUnits(GameFSceneDirector.myPlayerNumber);
        ///１つずつにカーソルを配置
        for(int i = 0; i < tmpunitsCursors.Count; i++)
        {
            UnitController tmp = tmpunitsCursors[i];
            //ユニットがいるタイルを選択
            tileCursor = tiles[tmp.Pos.x, tmp.Pos.y];

            //選んだタイルからユニット取得
            Vector2Int tilepos = new Vector2Int((int)tileCursor.transform.position.x + TILE_X / 2, (int)tileCursor.transform.position.z + TILE_Z / 2);

            //タイルに乗っているユニット
            unitCursor = units[tilepos.x, tilepos.y];

            //カーソルを順番に配置する
            foreach(var v in unitCursor.UnitsCursorsTiles(units))
            {
                Vector3 pos = tiles[v.x, v.y].transform.position;
                
                pos.y += 0.51f;
                obj = Instantiate(prefabUnitsCursor, pos, Quaternion.identity);

                // Vector3 coreCursorPos = new Vector3(GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.x, 0.51f, GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.z);

                // Debug.Log(GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.x + "   " + GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.z);

                // obj = Instantiate(coreUnitCursor, coreCursorPos, Quaternion.identity);
                unitsCursors.Add(obj);
            }

            


            // Debug.Log(unitCursor.Type);
            // //コアユニットの場合の足場の色
            // if(unitCursor.Type == UnitController.TYPE.KING)
            // {
            //     Vector3 pos = new Vector3(0, 0.51f, 0);
            //     GameObject obj = Instantiate(coreUnitCursor, pos, Quaternion.identity);
            //     unitsCursors.Add(obj);
            // }
            // //その他の足場の色
            // else
            // {
            //     Vector3 pos = new Vector3(0, 0.51f, 0);
            //     GameObject obj = Instantiate(prefabUnitsCursor, pos, Quaternion.identity);
            //     unitsCursors.Add(obj);
            // }
        }
        Vector3 coreCursorPos = new Vector3(GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.x, 0.51f, GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.z);  //<=
        obj = Instantiate(coreUnitCursor, coreCursorPos, Quaternion.identity);
        unitsCursors.Add(obj);

        // Debug.Log(GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.x + "   " + GameObject.FindWithTag("NucleusWhite").transform.gameObject.transform.position.z);

        unitsCursorsOneLoop = true;
    }

    //ユニット移動
    void moveUnit(UnitController unit, Vector2Int tilepos)
    {
        unitsCursorsOneLoop = false;  //<=
        moveGradually = false;
        //コアユニットとその他の
        if(unit.transform.gameObject.tag == "NucleusWhite" || unit.transform.gameObject.tag == "NucleusBlack") 
        {
            // attackable = false;
            //コアロボットの攻撃力
            AttackScript.attackPower = coreUnitsAttack;
        }
        else
        {
            // attackable = true;
            //ロボットの攻撃力
            AttackScript.attackPower = unitsAttack;
        }

        Vector2Int unitpos = unit.Pos;

        // //移動先に誰かがいたら消す
        // if(null != units[tilepos.x, tilepos.y])
        // {
        //     Destroy(units[tilepos.x, tilepos.y].gameObject);
        //     prevDestroyTurn = 0;  //消えてからのターン数をリセット
        // }


        UnitController unitCopy = unit;
        // MoveGradually(unit, tilepos);
        StartCoroutine(MoveGradually(unit, tilepos, unitpos, moveGradually));
        StartCoroutine(VisibleOrInvisible());
    }

    //移動時のアニメーション
    IEnumerator MoveGradually(UnitController unitPosition, Vector2Int tilepos, Vector2Int unitpos, bool moveGradually)
    {
        float distance = Mathf.Sqrt(Mathf.Round(Mathf.Abs(tilepos.x - GameSceneDirector.TILE_X / 2 - unitPosition.transform.position.x)) * 
                                    Mathf.Round(Mathf.Abs(tilepos.x - GameSceneDirector.TILE_X / 2 - unitPosition.transform.position.x)) + 
                                    Mathf.Round(Mathf.Abs(tilepos.y - GameSceneDirector.TILE_Z / 2 - unitPosition.transform.position.z)) * 
                                    Mathf.Round(Mathf.Abs(tilepos.y - GameSceneDirector.TILE_Z / 2 - unitPosition.transform.position.z)));
        float speed = distance / 0.8f;
        Vector3 targetPos = new Vector3(tilepos.x - GameSceneDirector.TILE_X / 2, 0.82f, tilepos.y - GameSceneDirector.TILE_Z / 2);
        target.transform.position = targetPos;

        //攻撃のエフェクトアニメーションを開始
        // AnimationManager.onAttackAnimation = true;
        // while(AnimationManager.onAttackAnimation == false)
        // {
        //     if(AnimationManager.onAttackAnimation == true) break;
        // }
        //アニメーションスタート
        if(AnimationManager.onAttackAnimation == false) StartCoroutine(AnimationReset());

        int oneDelay = 0;
        
        //アニメーション
        while(unitPosition.transform.position != target.position)
        {
            //体の向き
            Vector3 diff = new Vector3(unitPosition.transform.position.x - latestPos.x, 0f, unitPosition.transform.position.z - latestPos.z);
            latestPos = unitPosition.transform.position;
            //ベクトルの大きさが0.01以上の時に向きを変える
            if(diff.magnitude > 0.01f)
            {
                unitPosition.transform.rotation = Quaternion.LookRotation(diff);
            }

            if(oneDelay == 1) yield return new WaitForSeconds(0.5f);

            //移動
            unitPosition.transform.position = Vector3.MoveTowards(unitPosition.transform.position, target.position, speed * Time.deltaTime);

            //移動が完了したら
            if(unitPosition.transform.position == target.position)
            {
                //インデックスの更新
                unitPosition.MoveUnit(tiles[tilepos.x, tilepos.y]);

                //配列データの更新（元の場所）
                units[unitpos.x, unitpos.y] = null;
                //配列データの更新（新しい場所）
                units[tilepos.x, tilepos.y] = unitPosition;

                yield return new WaitForSeconds(0.7f);

                //体の向きを元に戻す
                unitPosition.transform.rotation = Quaternion.Euler(0, 0, 0);
                if(unitPosition.Player == 1) unitPosition.transform.rotation = Quaternion.Euler(0, 180f, 0);

                // //攻撃のエフェクトアニメーションをリセット
                // AnimationManager.onAttackAnimation = false;

                nextMode = MODE.STATUS_UPDATE;
                // oneLoop = false;
                break;
            }
            else yield return new WaitForSeconds(0.001f);




            oneDelay++;
        }
    }

    //見えるか否か
    IEnumerator VisibleOrInvisible()
    {
        // AttackScript.inAttackRange = true;
        yield return new WaitForSeconds(1.4f);
        if(selectUnit.Player == 0 && selectUnit != null) UnitDistance.changeTransparencyMT = true;
        // if(/*selectUnit.Player == 0 && */selectUnit != null) UnitDistance.changeTransparencyMT = true;
        if(selectUnit.Player == 1 && selectUnit != null) UnitDistance.changeTransparencyYT = true;
        //カーソルを消す
        setSelectCursors();
    }

    //指定された配置でチェックされているかチェック
    static public List<UnitController> GetCheckUnits(UnitController[,] units, int player, bool checkking = true)  //staticにすることで他でも使用できる
    {
        List<UnitController> ret = new List<UnitController>();

        //敵プレイヤーの移動可能範囲にキングがいるかどうか
        foreach(var v in units)
        {
            if(null == v) continue;
            if(player == v.Player) continue;

            //敵の一体の移動可能範囲
            List<Vector2Int> enemytiles = v.GetMovableTiles(units, checkking);

            foreach(var t in enemytiles)
            {
                if(null == units[t.x, t.y]) continue;
                if(UnitController.TYPE.KING == units[t.x, t.y].Type)  //移動可能範囲にKINGがいた場合
                {
                    ret.Add(v);
                }
            }
        }
        return ret;
    }

    //ユニットのプレハブを返す
    GameObject getPrefubUnit(int mPlayer, int mType)
    {
        // int targetIdx = 0;
        int idx = mType - 1;

        if(0 > idx) return null;

        // GameObject targetPrefab = targets[targetIdx];
        // GameObject targetPrefab = Instantiate(targets[targetIdx], startTargetPos, Quaternion.identity);
        // GameObject tile = Instantiate(prefabTile[idx], pos, Quaternion.identity);

        GameObject prefab = prefabWhiteUnits[idx];  //デフォルトでwhiteを返す
        // GameObject prefab = Instantiate(prefabWhiteUnits[idx], unitsPos, Quaternion.identity);
        if(1 == mPlayer) prefab = prefabBlackUnits[idx];  //もしプレイヤー番号が１ならblackのprefabを返す

        // targetIdx++;
        return prefab;
    }

    IEnumerator AnimationReset()
    {
        //ゲーム開始時に一度呼ぶ
        if(battleStart == false)
        {
            yield return new WaitForSeconds(0.5f);

            //アニメーションスタート
            AnimationManager.onGameStartAttackAnimation = true;

            //バトル開始アニメーションが終了
            // yield return new WaitForSeconds(3f);
        }
        //二週目以降
        else if(battleStart == true)
        {
            // yield return new WaitForSeconds(0.15f);   //0.f
            
            // while(AnimationManager.confirmation == false)
            // {
                AnimationManager.onAttackAnimation = true;
                // yield return new WaitForSeconds(0.1f);
            // }
            
            // Debug.Log(AnimationManager.onAttackAnimation + "     1");
            yield return new WaitForSeconds(0.35f);   //0.5f
        }

        yield return new WaitForSeconds(1f);
        //攻撃のエフェクトアニメーションをリセット
        
        AnimationManager.onAttackAnimation = false;
        AnimationManager.posCheckAttackAnimation = false;

        if(!battleStart)
        {
            yield return new WaitForSeconds(5f);  //ここでゲーム開始までの時間を設定

            AnimationManager.onGameStartAttackAnimation = false;
            battleStart = true;

            //UIの表示
            btnTitle.SetActive(true);
            turnInfoPanel.SetActive(true);
            movePatternPanel.SetActive(true);
            diceNumUI.SetActive(true);
            enemyRemainingAircraftUI.SetActive(true);
            frameRemainingAircraftUI.SetActive(true);
            TransChangeWcore.transChangeWcore = true;
            TransChangeW1.transChangeW1 = true;
            TransChangeW2.transChangeW2 = true;
            TransChangeW3.transChangeW3 = true;
            TransChangeW4.transChangeW4 = true;
            TransChangeW5.transChangeW5 = true;
            TransChangeW6.transChangeW6 = true;
            startHPSlider = true;
            // HpUIManager.onHPSlider = true;
            // HpUIManager.hpSliderUI.SetActive(true);
            // HpUIManager.bulkHPSliderUI.SetActive(true);
        }
        
        // AnimationManager.confirmation = false;
        // selectOK = true;
    }

    public static void EnemyRemainingAircraftDisplay()
    {
        //敵の残機を表示
        if(enemyRemainingAircraftNumCopy != enemyRemainingAircraftNum)
        {
            for(int i = 0; i < GameFSceneDirector.unitsNum - enemyRemainingAircraftNum; i++)
            {
                txtEnemyRemain[i].GetComponent<Text>().color = new Color(0.9f, 0.9f, 0.9f, 0.3f);
            }
            enemyRemainingAircraftNumCopy = enemyRemainingAircraftNum;
        }
    }



    public void Retry()
    {
        SceneManager.LoadScene("GameFirstScene");
    }

    public void Title()
    {
        TitleSceneDirector.titleSceneCamera = 3;
        SceneManager.LoadScene("TitleScene");
    }
}
