using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneDirector : MonoBehaviour
{
    //ゲーム全体で使用するプレイヤー数
    static public int PlayerCount = 1;

    //カメラのタイトル用
    static public int titleSceneCamera = 3;

    //オンライン戦かどうか
    static public bool IsOnline { get; set;}  //{外部から取得できます; 外部からセットできます;}


    // //タイルのプレハブ
    // public GameObject[] prefabTile;
    // //カーソルのプレハブ
    // public GameObject prefabCursor;


    // //内部データ
    // GameObject[,] tiles;
    // UnitController[,] units;

    // //ユニットのプレハブ（色ごと）
    // public List<GameObject> prefabWhiteUnits;
    // public List<GameObject> prefabBlackUnits;

    // Start is called before the first frame update
    void Start()
    {
        //内部データ
        // tiles = new GameObject[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z];
        // units = new UnitController[GameSceneDirector.TILE_X, GameSceneDirector.TILE_Z];

        // for(int i = 0; i < GameSceneDirector.TILE_X; i++)
        // {
        //     for(int j = 0; j < GameSceneDirector.TILE_Z; j++)
        //     {
        //         float x = i - GameSceneDirector.TILE_X / 2; //真ん中に来るように引いて調節
        //         float y = j - GameSceneDirector.TILE_Z / 2; //上に同じ

        //         Vector3 pos = new Vector3(x, 0, y);

        //         //作成
        //         int idx = (i + j) % 2;
        //         GameObject tile = Instantiate(prefabTile[idx], pos, Quaternion.identity);

        //         tiles[i, j] = tile;

        //         //ユニットの作成
        //         int type   = GameFSceneDirector.unitType[i, j] % 10;
        //         int player = GameFSceneDirector.unitType[i, j] / 10;

        //         GameObject prefab = getPrefubUnit(player, type);
        //         GameObject unit = null;
        //         UnitController ctrl = null;

        //         if(null == prefab) continue;  //prefabが０つまりnullになることもあるのでnullチェックしてこれ以上先に行かないようにする

        //         pos.y += 1.5f;
        //         unit = Instantiate(prefab);

        //         //初期設定
        //         ctrl = unit.GetComponent<UnitController>();
        //         ctrl.SetUnit(player, (UnitController.TYPE)type, tile);

        //         //内部データをセット
        //         units[i,j] = ctrl;
        //     }
        // }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    // //ユニットのプレハブを返す
    // GameObject getPrefubUnit(int player, int type)
    // {
    //     int idx = type - 1;

    //     if(0 > idx) return null;

    //     GameObject prefab = prefabWhiteUnits[idx];  //デフォルトでwhiteを返す
    //     if(1 == player) prefab = prefabBlackUnits[idx];  //もしプレヤー番号が１ならblackのprefabを返す

    //     return prefab;
    // }


    
    public void PvP()
    {
        PlayerCount = 2;
        titleSceneCamera = 4;
        IsOnline = false;
        SceneManager.LoadScene("GameFirstScene");
    }

    public void PvE()
    {
        PlayerCount = 1;
        titleSceneCamera = 4;
        IsOnline = false;
        SceneManager.LoadScene("GameFirstScene");
    }

    public void EvE()
    {
        PlayerCount = 0;
        titleSceneCamera = 4;
        IsOnline = false;
        SceneManager.LoadScene("GameFirstScene");
    }

    public void Online()
    {
        PlayerCount = 2;
        titleSceneCamera = 4;
        IsOnline = true;
        SceneManager.LoadScene("Online");
    }
}
