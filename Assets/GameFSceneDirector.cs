using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;

public class GameFSceneDirector: MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectMassPlane;//選択されたマスの上に表示するPlane

    //内部データ
    GameObject[,] tiles;
    GameObject[,] unitCPU;
    UnitController[,] units;

    //UI関連
    GameObject ButtonDecision;
    GameObject txtReloadNum;


    // private int thisSceneY = 2;

    static public int myPlayerNumber = 0;

    int nowPlayer;
    static public int unitsNum = 7;
    //残りの置けるユニット数
    int myUnitsNum;
    int enemyUnitsNum;
    //残りの置けるタイル数
    int myRestTiles = GameSceneDirector.TILE_X * 2;
    int enemyRestTiles = GameSceneDirector.TILE_X * 2;
    int reloadNum = 3;

    // private int coreOne;

    private int player;
    private int type;
    // private bool nuclearExistence;

    //タイルのプレハブ
    public GameObject[] prefabTile;
    //カーソルのプレハブ
    public GameObject prefabCursor;

    //コアロボットの座標を保存
    GameObject coreWhiteUnitPos;
    GameObject coreBlackUnitPos;
    Vector2Int coreBlackPos;

    //ユニットのプレハブ（色ごと）
    public List<GameObject> prefabWhiteUnits;
    public List<GameObject> prefabBlackUnits;

    //選択中のユニット
    UnitController selectUnit;

    //移動関連
    List<Vector2Int> movableTiles;
    //移動可能範囲のタイルに色を付けるため
    List<GameObject> cursors;

    RaycastHit raycastHit;//Rayに接触したオブジェクト情報を取得して代入

    public bool preparationCheckWhite;
    public bool preparationCheckBlack;

//    // 1 = ポーン 2 = ルーク 3 = ナイト 4 = ビショップ 5 = クイーン 6 = キング
    static public int[,] unitType = new int[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z];
    // { //左が手前側でコマを配置する
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    //     {8, 8, 0, 0, 0, 0, 0, 9, 9},
    // };

    static public int[,] unitTypeCopy = unitType;

    // static public int[,] firstSetUnits =
    // { //左が手前側でコマを配置する
    //     {0, 0, 0, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 1, 0},
    //     {0, 0, 0, 0},
    // };

    void Start()
    {
        //リロード回数の表示
        txtReloadNum = GameObject.Find("TextReloadNum");
        txtReloadNum.GetComponent<Text>().text = reloadNum + "/3";

        //配置可能エリア
        ArrangeableArea();

        // //ランダムに　いるかな、
        // UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        //unitTypeをリセット
        // unitType = unitTypeCopy;

        GameSceneDirector.startHPSlider = false;

        preparationCheckWhite = false;
        preparationCheckBlack = false;

        //内部データ
        tiles = new GameObject[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        units = new UnitController[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        movableTiles = new List<Vector2Int>();

        //移動関連
        cursors = new List<GameObject>();  //初期化

        //初期化
        myUnitsNum = unitsNum;
        enemyUnitsNum = unitsNum;

        //自分のユニットの初期配置をランダムにする
        ArrangedRandomlyMyUnits();

        //敵のユニットの初期配置をランダムにする
        ArrangedRandomlyEnemyUnits();


        // //ユニット番号を反映させるため
        // int unitNumberW = 0;
        // int unitNumberB = 0;

        //unitTypeを元に盤にセット
        UnitsSet();


        // if(coreWhiteUnitPos == null) coreWhiteUnitPos = GameObject.FindGameObjectsWithTag("NucleusWhite");
        // coreWhiteUnitPos = GameObject.FindGameObjectsWithTag("NucleusWhite");
        // Vector3 coreWhiteUnitPos = GameObject.Find("NucleusWhite").transform.position;


        // nuclearExistence = false;
        // mainCamera = Camera.main;
        // raycastHit = new RaycastHit();
    }

    void Update()
    {
        GameObject coordinateWhite = null;
        GameObject coordinateBlack = null;
        GameObject tile = null;
        UnitController unitCore = null;
        UnitController unitCPUselected = null;

        //自分のコアのユニットの座標
        // if(null != GameObject.FindWithTag("NucleusWhite")) 
        coreWhiteUnitPos = GameObject.FindWithTag("NucleusWhite").transform.gameObject;
        if(null == coreWhiteUnitPos) return;

        //相手のコアのユニットの座標
        // if(null != GameObject.FindWithTag("NucleusBlack")) 
        coreBlackUnitPos = GameObject.FindWithTag("NucleusBlack").transform.gameObject;
        if(null == coreBlackUnitPos) return;

        //プレイヤー
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //  TODO  全てのオブジェクト情報を取得   敵ユニットは選択できないようにする
            foreach(RaycastHit hit in Physics.RaycastAll(ray))
            {
                if(hit.transform.name.Contains("StoneW") && preparationCheckWhite == false && myPlayerNumber == 0)
                {
                    //一度保存
                    coordinateWhite = hit.transform.gameObject;
                    break;
                }

                if(hit.transform.name.Contains("StoneB") && myPlayerNumber == 1) //  TODO
                {
                    //一度保存
                    coordinateBlack = hit.transform.gameObject;
                    break;
                }
            }
        }

        //CPU  ランダム選択
        if(TitleSceneDirector.PlayerCount < myPlayerNumber+2 && coordinateBlack == null && !preparationCheckBlack)
        {
            //相手の全ユニット
            List<UnitController> tmpunits = getUnits(myPlayerNumber+1);
            //ランダムで一体選ぶ
            UnitController tmp = tmpunits[Random.Range(0, tmpunits.Count)];
            //ユニットがいるタイルを選択
            tile = tiles[tmp.Pos.x, tmp.Pos.y];
            //先に値の入れ替えを行う
            coordinateBlack = tile;
            CorePosChange(coordinateWhite, coordinateBlack);


            //タイルが押されていなければ処理しない
            if(null == tile) return;

            //選んだタイルからユニット取得
            Vector2Int CPUtilepos = new Vector2Int((int)tile.transform.position.x + GameSceneDirector.TILE_X / 2, (int)tile.transform.position.z + GameSceneDirector.TILE_Z / 2);
            Vector2Int CPUcoreTilepos = new Vector2Int((int)coreBlackUnitPos.transform.position.x + GameSceneDirector.TILE_X / 2, (int)coreBlackUnitPos.transform.position.z + GameSceneDirector.TILE_Z / 2);

            //選択されたタイルに乗っているユニット
            unitCPUselected = units[CPUtilepos.x, CPUtilepos.y];
            //コアのユニット
            unitCore = units[CPUcoreTilepos.x, CPUcoreTilepos.y];
            //入れ替える
            // Debug.Log(CPUtilepos.x + "   " + CPUtilepos.y + "       " + CPUcoreTilepos.x + "   " + CPUcoreTilepos.y);
            if(CPUtilepos.x != CPUcoreTilepos.x && CPUtilepos.y != CPUcoreTilepos.y)    //  TODO
            {
                if(CPUtilepos != null && CPUcoreTilepos != null && unitCore != null && unitCPUselected != null)
                moveUnit(unitCore, CPUtilepos);
                moveUnit(unitCPUselected, CPUcoreTilepos);
            }
            //コアロボットの選択を完了
            preparationCheckBlack = true;
            // Debug.Log("black true");
        }
        else CorePosChange(coordinateWhite, coordinateBlack);
    }

    //初期位置の配置可能エリア
    public void ArrangeableArea()
    {
        for(int tileX = 0; tileX < GameSceneDirector.TILE_X; tileX++)
        {
            for(int tileZ = 0; tileZ < GameSceneDirector.TILE_Z; tileZ++)
            {
                if(tileZ == 0 || tileZ == 1)
                {
                    unitType[tileX, tileZ] = 8;
                }
                else if(tileZ == GameSceneDirector.TILE_Z-2 || tileZ == GameSceneDirector.TILE_Z-1)
                {
                    unitType[tileX, tileZ] = 9;
                }
                else unitType[tileX, tileZ] = 0;
            }
        }
    }

    //自分のユニットの配置可能エリアをリセット
    public void MyArrangeableAreaReset()
    {
        for(int tileX = 0; tileX < GameSceneDirector.TILE_X; tileX++)
        {
            for(int tileZ = 0; tileZ < GameSceneDirector.TILE_Z; tileZ++)
            {
                //手前２列は８に変更し、敵側２列は触らない
                if(tileZ == 0 || tileZ == 1)
                {
                    unitType[tileX, tileZ] = 8;
                }
                else if(tileZ == GameSceneDirector.TILE_Z-2 || tileZ == GameSceneDirector.TILE_Z-1)
                {
                    
                }
                else unitType[tileX, tileZ] = 0;
            }
        }
    }

    //自分のユニットの初期配置をランダムにする
    public void ArrangedRandomlyMyUnits()
    {
        // UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        // unitType = unitTypeCopy;

        // // preparationCheckWhite = false;
        // // preparationCheckBlack = false;

        // //内部データ
        // tiles = new GameObject[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        // units = new UnitController[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        // movableTiles = new List<Vector2Int>();

        // //移動関連
        // cursors = new List<GameObject>();  //初期化


        //一体にコアを持たせる
        for(int i = 0; i < GameSceneDirector.TILE_X; i++)
        {
            for(int j = 0; j < GameSceneDirector.TILE_Z/*thisSceneY*/; j++)
            {
                //二次元配列の8の場所にランダムでロボットの位置を決める（コア：１, その他：２, 置かない：０）
                if(unitType[i, j] % 10 == 8)
                {
                    //既にユニットが存在していたら消す
                    if(null != units[i, j]) Destroy(units[i, j].gameObject);

                    //0のところには何も置かない
                    if(myUnitsNum <= 0)
                    {
                        unitType[i, j] = 0;
                    }
                    //残りの配置可能マス数が、残りの表示させなければならないユニット数と同じならば全て１にする
                    else if(myRestTiles == myUnitsNum)
                    {
                        if(myUnitsNum == unitsNum) unitType[i, j] = 1;
                        if(myUnitsNum != unitsNum) unitType[i, j] = 2;
                        
                        myUnitsNum--;
                    }
                    else
                    {
                        if(Random.Range(0.0f, 1.0f) <= 0.35f)
                        // if(UnityEngine.Random.value <= 0.35f)
                        {
                            if(myUnitsNum == unitsNum) unitType[i, j] = 1;
                            if(myUnitsNum != unitsNum) unitType[i, j] = 2;
                            myUnitsNum--;
                        }
                        else
                        {
                            unitType[i, j] = 0;
                        }
                    }
                    myRestTiles--;
                }
            }
        }
    }

    //相手のユニットの初期配置をランダムにする
    public void ArrangedRandomlyEnemyUnits()
    {
        //一体にコアを持たせる
        for(int i = 0; i < GameSceneDirector.TILE_X; i++)
        {
            for(int j = 0; j < GameSceneDirector.TILE_Z/*thisSceneY*/; j++)
            {
                if(unitType[i, j] % 10 == 9)
                {
                    if(enemyUnitsNum <= 0)
                    {
                        unitType[i, j] = 0;
                    }
                    //残りの配置可能マス数が、残りの表示させなければならないユニット数と同じならば全て１にする
                    else if(enemyRestTiles == enemyUnitsNum)
                    {
                        if(enemyUnitsNum == unitsNum) unitType[i, j] = 11;
                        if(enemyUnitsNum != unitsNum) unitType[i, j] = 12;
                        enemyUnitsNum--;
                    }
                    else
                    {
                        if(Random.Range(0.0f, 1.0f) <= 0.35f)
                        {
                            if(enemyUnitsNum == unitsNum) unitType[i, j] = 11;
                            if(enemyUnitsNum != unitsNum) unitType[i, j] = 12;
                            enemyUnitsNum--;
                        }
                        else
                        {
                            unitType[i, j] = 0;
                        }
                    }
                    enemyRestTiles--;
                }
            }
        }

        // for(int i = 0; i < 9; i++)
        // {
        //     for(int j = 6; j <= 8; j++)
        //     {
        //         Debug.Log("unitType[" + i + ", " + j + "]   =>   " + unitType[i, j]);
        //     }
        // }
    }

    //ユニットを盤にセット
    public void UnitsSet()
    {
        // unitType = unitTypeCopy;

        // // preparationCheckWhite = false;
        // // preparationCheckBlack = false;

        // //内部データ
        // tiles = new GameObject[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        // units = new UnitController[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        // movableTiles = new List<Vector2Int>();

        // //移動関連
        // cursors = new List<GameObject>();  //初期化



        //ユニット番号を反映させるため
        int unitNumberW = 0;
        int unitNumberB = 0;


        //unitTypeを元に盤にセット
        for(int i = 0; i < GameSceneDirector.TILE_X; i++)
        {
            for(int j = 0; j < GameSceneDirector.TILE_Z/*thisSceneY*/; j++)
            {
                GameObject prefab;

                float x = i - GameSceneDirector.TILE_X / 2; //真ん中に来るように引いて調節
                float y = j - GameSceneDirector.TILE_Z/*thisSceneY*/ / 2; //上に同じ

                Vector3 pos = new Vector3(x, 0, y);
                // Vector3 hpPos = new Vector3(x, 5, y);

                //作成
                int idx = 0/*(i + j) % 2*/;
                GameObject setTile = Instantiate(prefabTile[idx], pos, Quaternion.identity);

                tiles[i, j] = setTile;

                //ユニットの作成
                type   = unitType[i, j] % 10;  //最初に指定された盤面から初期位置をこれで読み取る
                player = unitType[i, j] / 10;

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

                GameObject unit = null;
                UnitController ctrl = null;
                // GameObject hpSlider = null;///
                // UnitController gauge = null;///

                if(null == prefab) continue;  //prefabが０つまりnullになることもあるのでnullチェックしてこれ以上先に行かないようにする

                pos.y += 1.5f;
                unit = Instantiate(prefab);
                // hpSlider = Instantiate(prefab);///

                //初期設定
                ctrl = unit.GetComponent<UnitController>();
                ctrl.SetUnit(player, (UnitController.TYPE)type, setTile);
                // gauge = hpSlider.GetComponent<UnitController>();///
                // gauge.SetUnit(player, (UnitController.TYPE)type, tile);///

                //内部データをセット
                units[i,j] = ctrl;
                // hpSliders[i,j] = gauge;///

                // if(player == 1) Debug.Log(units[i,j].transform.position);
            }
        }
    }

    //自分のユニットのみを盤に再セット
    public void MyUnitsReset()
    {
        // unitType = unitTypeCopy;

        // preparationCheckWhite = false;
        // preparationCheckBlack = false;

        //内部データ
        // tiles = new GameObject[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        // units = new UnitController[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z/*thisSceneY*/];
        // movableTiles = new List<Vector2Int>();

        // //移動関連
        // cursors = new List<GameObject>();  //初期化



        //ユニット番号を反映させるため
        int unitNumberW = 0;
        // int unitNumberB = 0;

        //unitTypeを元に盤にセット
        for(int i = 0; i < GameSceneDirector.TILE_X; i++)
        {
            for(int j = 0; j < GameSceneDirector.TILE_Z/*thisSceneY*/; j++)
            {
                if(j == 0 || j == 1)
                {
                    GameObject prefab;

                    float x = i - GameSceneDirector.TILE_X / 2; //真ん中に来るように引いて調節
                    float y = j - GameSceneDirector.TILE_Z/*thisSceneY*/ / 2; //上に同じ

                    Vector3 pos = new Vector3(x, 0, y);
                    // Vector3 hpPos = new Vector3(x, 5, y);

                    //既にタイルが存在していたら消す
                    if(null != tiles[i, j]) Destroy(tiles[i, j].gameObject);

                    //作成
                    int idx = 0/*(i + j) % 2*/;
                    GameObject setTile = Instantiate(prefabTile[idx], pos, Quaternion.identity);

                    tiles[i, j] = setTile;

                    //ユニットの作成
                    type   = unitType[i, j] % 10;  //最初に指定された盤面から初期位置をこれで読み取る
                    player = unitType[i, j] / 10;

                    if(player == 0 && type == 2)
                    {
                        prefab = getPrefubUnit(player, type + unitNumberW);
                        // Debug.Log(unitNumberW);
                        unitNumberW++;
                    }
                    // else if(player == 1 && type == 2)
                    // {
                    //     prefab = getPrefubUnit(player, type + unitNumberB);
                    //     unitNumberB++;
                    // }
                    else
                    {
                        prefab = getPrefubUnit(player, type);
                    }

                    GameObject unit = null;
                    UnitController ctrl = null;
                    // GameObject hpSlider = null;///
                    // UnitController gauge = null;///

                    if(null == prefab) continue;  //prefabが０つまりnullになることもあるのでnullチェックしてこれ以上先に行かないようにする

                    pos.y += 1.5f;
                    unit = Instantiate(prefab);

                    //初期設定
                    ctrl = unit.GetComponent<UnitController>();
                    ctrl.SetUnit(player, (UnitController.TYPE)type, setTile);
                    // gauge = hpSlider.GetComponent<UnitController>();///
                    // gauge.SetUnit(player, (UnitController.TYPE)type, tile);///

                    //内部データをセット
                    units[i,j] = ctrl;
                    // hpSliders[i,j] = gauge;///
                }
            }
        }
    }

    //各試合n回まで初期位置を変更可能にする
    public void InitialPositionReset()
    {
        if(reloadNum != 0)
        {
            //二次元配列の初期化
            // unitType = new int[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z];


            //配置可能エリア
            MyArrangeableAreaReset();

            //自分のユニットの分のみ初期化
            myUnitsNum = unitsNum;
            myRestTiles = GameSceneDirector.TILE_X * 2;



            //自分のユニットの初期位置だけランダムに準備
            ArrangedRandomlyMyUnits();

            //自分のユニットを盤に再セット
            MyUnitsReset();
            // TODO 相手の分も作成

            //回数制限のカウントを減らす
            reloadNum--;
            txtReloadNum.GetComponent<Text>().text = reloadNum + "/3";
        }
    }

    //座標のunitTypeの値入れ替え、ユニット自体の入れ替え
    void CorePosChange(GameObject coordinateWhite, GameObject coordinateBlack)
    {
        //ユニットが押されていなければ処理しない
        if(null == coordinateWhite && null == coordinateBlack)
        {
            return;
        }
        else if(null != coordinateWhite/* && null == coordinateBlack*/)
        {
            //自分側の配列の数値を入れ替える
            int switchUniW = unitType[(int)Mathf.Round(coordinateWhite.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                                      (int)Mathf.Round(coordinateWhite.transform.position.z) + GameSceneDirector.TILE_Z / 2];
            int switchCoreW = unitType[(int)Mathf.Round(coreWhiteUnitPos.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                                       (int)Mathf.Round(coreWhiteUnitPos.transform.position.z) + GameSceneDirector.TILE_Z / 2];
            (switchUniW, switchCoreW) = (switchCoreW, switchUniW);
            unitType[(int)Mathf.Round(coordinateWhite.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                     (int)Mathf.Round(coordinateWhite.transform.position.z) + GameSceneDirector.TILE_Z / 2] = switchUniW;
            unitType[(int)Mathf.Round(coreWhiteUnitPos.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                     (int)Mathf.Round(coreWhiteUnitPos.transform.position.z) + GameSceneDirector.TILE_Z / 2] = switchCoreW;

            //自分のユニットの座標を入れ替える
            (coordinateWhite.transform.position, coreWhiteUnitPos.transform.position) = (coreWhiteUnitPos.transform.position, coordinateWhite.transform.position);
            coordinateWhite.transform.position = new Vector3((int)Mathf.Round(coordinateWhite.transform.position.x), 1, (int)Mathf.Round(coordinateWhite.transform.position.z));
            coreWhiteUnitPos.transform.position = new Vector3((int)Mathf.Round(coreWhiteUnitPos.transform.position.x), 1, (int)Mathf.Round(coreWhiteUnitPos.transform.position.z));
        }
        else if(/*null == coordinateWhite && */null != coordinateBlack)
        {
            //相手側の配列の数値を入れ替える
            int switchUniB = unitType[(int)Mathf.Round(coordinateBlack.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                                      (int)Mathf.Round(coordinateBlack.transform.position.z) + GameSceneDirector.TILE_Z / 2];
            int switchCoreB = unitType[(int)Mathf.Round(coreBlackUnitPos.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                                       (int)Mathf.Round(coreBlackUnitPos.transform.position.z) + GameSceneDirector.TILE_Z / 2];
            (switchUniB, switchCoreB) = (switchCoreB, switchUniB);
            unitType[(int)Mathf.Round(coordinateBlack.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                     (int)Mathf.Round(coordinateBlack.transform.position.z) + GameSceneDirector.TILE_Z / 2] = switchUniB;
            unitType[(int)Mathf.Round(coreBlackUnitPos.transform.position.x) + GameSceneDirector.TILE_X / 2, 
                     (int)Mathf.Round(coreBlackUnitPos.transform.position.z) + GameSceneDirector.TILE_Z / 2] = switchCoreB;

            // 相手のユニットの座標を入れ替える
            if(TitleSceneDirector.PlayerCount == myPlayerNumber+2)
            {
                (coordinateBlack.transform.position, coreBlackUnitPos.transform.position) = (coreBlackUnitPos.transform.position, coordinateBlack.transform.position);
                coordinateBlack.transform.position = new Vector3((int)Mathf.Round(coordinateBlack.transform.position.x), 1, (int)Mathf.Round(coordinateBlack.transform.position.z));
                coreBlackUnitPos.transform.position = new Vector3((int)Mathf.Round(coreBlackUnitPos.transform.position.x), 1, (int)Mathf.Round(coreBlackUnitPos.transform.position.z));
            }
        }
        else {Debug.Log($"SELECTED: {"GameFSceneDirector"}");}
    }

    //選択、非選択状態
    void setSelectNucleus(UnitController unit = null, bool setNucleus = true)
    {
        // //カーソル解除
        // foreach(var v in cursors)
        // {
        //     Destroy(v);
        // }

        //選択ユニットの非選択状態
        if(null != selectUnit)  //すでに選択していた場合
        {
            selectUnit.SelectUnit(false);  //セレクトしているユニットのフォーカスを外す
            selectUnit = null;
        }

        //なにもセットしていないなら終わり
        if(null == unit) return;

        // //カーソル作成
        // foreach(var v in getMovableTiles(unit))
        // {
        //     Vector3 pos = tiles[v.x, v.y].transform.position;
        //     pos.y += 0.51f;
        //     GameObject obj = Instantiate(prefabCursor, pos, Quaternion.identity);
        //     cursors.Add(obj);
        // }

        //選択状態
        if(setNucleus)
        {
            selectUnit = unit;
            selectUnit.SelectUnit(setNucleus);
        }
    }

    //ユニットのプレハブを返す
    GameObject getPrefubUnit(int mPlayer, int mType)
    {
        int idx = mType - 1;

        if(0 > idx) return null;

        GameObject prefab = prefabWhiteUnits[idx];  //デフォルトでwhiteを返す
        if(1 == mPlayer) prefab = prefabBlackUnits[idx];  //もしプレイヤー番号が１ならblackのprefabを返す

        return prefab;
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

    //ユニット移動
    bool moveUnit(UnitController unit, Vector2Int tilepos)
    {
        Vector2Int unitpos = unit.Pos;

        // //移動先に誰かがいたら消す
        // if(null != units[tilepos.x, tilepos.y])
        // {
        //     Destroy(units[tilepos.x, tilepos.y].gameObject);
        //     prevDestroyTurn = 0;  //消えてからのターン数をリセット
        // }

        //新しい場所へ移動
        unit.MoveUnit(tiles[tilepos.x, tilepos.y]);
        // this.gameObject.layer = LayerMask.NameToLayer("MovedUnit");

        //配列データの更新（元の場所）
        units[unitpos.x, unitpos.y] = null;

        //配列データの更新（新しい場所）
        units[tilepos.x, tilepos.y] = unit;

        return true;
    }





    public void btnDecisionW()
    {
        preparationCheckWhite = true;
        if(preparationCheckWhite)
        {
            // Debug.Log("white true");
            if(preparationCheckBlack)
            {
                // for(int i = 0; i < prefabWhiteUnits.Count; i++)
                // {
                //     Destroy(prefabWhiteUnits[i].gameObject);
                //     Destroy(units[tilepos.x, tilepos.y].gameObject);
                // }
                // for(int i = 0; i < prefabBlackUnits.Count; i++)
                // {
                //     Destroy(prefabBlackUnits[i].gameObject);
                // }
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    public void btnDecisionB()
    {
        preparationCheckBlack = true;
        if(preparationCheckBlack)
        {
            // Debug.Log("black true");
            if(preparationCheckWhite)
            {
                // for(int i = 0; i < prefabWhiteUnits.Count; i++)
                // {
                //     Destroy(prefabWhiteUnits[i].gameObject);
                // }
                // for(int i = 0; i < prefabBlackUnits.Count; i++)
                // {
                //     Destroy(prefabBlackUnits[i].gameObject);
                // }
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    public void Title()
    {
        TitleSceneDirector.titleSceneCamera = 3;
        SceneManager.LoadScene("TitleScene");
    }


    // public void Reload()
    // {
    //     SceneManager.LoadScene(2);
    // }
}
