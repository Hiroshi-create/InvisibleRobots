using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    //このユニットのプレイヤー番号
    public int Player;
    //ユニットの種類
    public TYPE Type;
    //置いてからの経過パターン
    public int ProgressTurnCount;
    //置いている場所
    public Vector2Int Pos, OldPos;
    //移動状態
    public List<STATUS> Status;

    // static public Vector2Int idx;
    

    // 1 = Type1 2 = Type2 3 = Type3 4 = Type4 5 = Type5 6 = Type6
    // public enum TYPE
    // {
    //     NONE = -1,
    //     TYPE1 = 1,  //PAWN としてOK
    //     TYPE2,  //ROOK としてOK
    //     TYPE3,  //KNIGHT としてOK
    //     TYPE4,  //BISHOP としてOK
    //     TYPE5,  //QUEEN としてOK
    //     TYPE6,  //KING としてOK
    // }
    //// 1 = ポーン 2 = ルーク 3 = ナイト 4 = ビショップ 5 = クイーン 6 = キング
    public enum TYPE
    {
        NONE = -1,
        PAWN = 1,
        ROOK,
        KNIGHT,
        BISHOP,
        QUEEN,
        KING,
        ATTACK,
    }

    ////移動状態  複数の状態を設定可能
    public enum STATUS
    {
        NONE = -1,
        QSIDE_CASTLING = 1,
        KSIDE_CASTLING,
        EN_PASSANT,
        CHECK,
    }

    // Start is called before the first frame update
    void Start()
    {
        // ProgressTurnCount = -1; //アンパッサン用 前回置いてからの経過ターン
        Status = new List<STATUS>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //初期設定
    public void SetUnit(int player, TYPE type, GameObject tile)
    {
        Player = player;
        Type = type;
        MoveUnit(tile);
        ProgressTurnCount = -1; //初動に戻す
    }

    //行動可能範囲の取得
    public List<Vector2Int> GetMovableTiles(UnitController[,] units, bool checkking = true)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        //----------------------------------------------------------------
        // //クイーン
        // if(TYPE.QUEEN == Type)
        // {
        //     //ルークとビショップの動きを合成
        //     ret = getMovableTiles(units, TYPE.ROOK);

        //     foreach(var v in getMovableTiles(units, TYPE.BISHOP))
        //     {
        //         if(!ret.Contains(v)) ret.Add(v);
        //     }
        // }
        
        //キング特有の処理をクリアするか
        if(TYPE.KING == Type)
        {
            ret = getMovableTiles(units, Type);

            //相手の行動範囲を考慮しない
            if(!checkking) return ret;

            //削除対象のタイル
            List<Vector2Int> removetiles = new List<Vector2Int>();

            // //敵の移動可能範囲と被っているかチェック
            // foreach(var v in ret)
            // {
            //     //移動した状態を作る
            //     UnitController[,] copyunits2 = new UnitController[units.GetLength(0), units.GetLength(1)];
            //     Array.Copy(units, copyunits2, units.Length);
            //     //移動してみる
            //     copyunits2[Pos.x, Pos.y] = null;
            //     copyunits2[v.x, v.y] = this;// Error: Index out of bounds

            //     int checkcount = GameSceneDirector.GetCheckUnits(copyunits2, Player, false).Count;

            //     //被っていたら削除対象
            //     if(0 < checkcount) removetiles.Add(v);

            // }
            // //上で取得した敵の移動範囲と被っているタイルを削除
            // foreach(var v in removetiles)
            // {
            //     ret.Remove(v);

            //     //キャスリングできる時だけ真横のタイルも削除
            //     if( -1 != ProgressTurnCount || Pos.y != v.y) continue;  //Pos.yが自分の場所、v.yが移動可能タイル

            //     //方向
            //     int add = -1;
            //     int cnt = units.GetLength(0);
            //     if(0 > Pos.x - v.x) add = 1;

            //     for(int i = v.x; 0 < cnt; i += add)
            //     {
            //         Vector2Int del = new Vector2Int(i, v.y);

            //         ret.Remove(del);
            //         cnt--;
            //     }
            // }
        }
        else
        {
            ret = getMovableTiles(units, Type);
        }

        return ret;
    }

    //指定されたタイプの移動可能範囲を返す
    List<Vector2Int> getMovableTiles(UnitController[,] units, TYPE type)
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        //ポーン  TYPE1
        if(TYPE.PAWN == type)
        {
            int dir = 1;
            if(1 == Player) dir = -1;

            //前方２マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                //dirで反対側の前方に直す
                new Vector2Int(0, 1 * dir),
                new Vector2Int(0, 2 * dir),
            };

            // //２回目以降は１マスしか進めない
            // if(-1 < ProgressTurnCount) vec.RemoveAt(vec.Count - 1);

            foreach(var v in vec)
            {
                Vector2Int checkpos = Pos + v;
                if(!isCheckable(units,checkpos)) continue;

                // 他のコマがあったら進めない
                if(null != units[checkpos.x, checkpos.y]) break;
                ret.Add(checkpos);  //コマを取るため
            }


            // //正面２マスの範囲で相手のコマを取れる
            // vec = new List<Vector2Int>()
            // {
            //     new Vector2Int(0, 1 * dir),
            //     new Vector2Int(0, 2 * dir),
            // };

            // foreach(var v in vec)
            // {
            //     Vector2Int checkpos = Pos + v;
            //     if(!isCheckable(units,checkpos)) continue;

            //     //なにもない時
            //     if(null == units[checkpos.x, checkpos.y]) continue;

            //     //自軍のユニットは無視
            //     if(Player == units[checkpos.x, checkpos.y].Player) continue;

            //     //ここまで来たら追加
            //     ret.Add(checkpos);
            // }
        }

        //ルーク  TYPE2
        else if(TYPE.ROOK == type)
        {
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int( 0, 1),
                new Vector2Int( 0,-1),
                new Vector2Int( 1, 0),
                new Vector2Int(-1, 0),
            };

            foreach(var v in vec)
            {
                int i = 1;  //iは３マス動かすために定義
                Vector2Int checkpos = Pos + v;
                while(isCheckable(units, checkpos) && i <= 3)  //配列オーバーしていない時にループ
                {
                    //誰かいたら終了（敵のタイルだったら追加して終了）
                    if(null != units[checkpos.x, checkpos.y])
                    {
                        // if(Player != units[checkpos.x, checkpos.y].Player)
                        // {
                        //     ret.Add(checkpos);
                        // }
                        break;
                    }
                    //誰もいない時
                    ret.Add(checkpos);
                    checkpos += v;
                    i++;
                }
            }
        }

        //ナイト  //TYPE3
        else if(TYPE.KNIGHT == type)
        {
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1, 2),
                new Vector2Int( 1, 2),
                new Vector2Int(-2,-1),
                new Vector2Int( 2,-1),
            };

            foreach(var v in vec)
            {
                Vector2Int checkpos = Pos + v;
                if(!isCheckable(units, checkpos)) continue;

                //同じプレイヤーのところは移動できない
                if(null != units[checkpos.x, checkpos.y]/* && Player == units[checkpos.x, checkpos.y].Player*/)
                {
                    continue;
                }
                ret.Add(checkpos);
            }
        }

        //ビショップ  //TYPE4
        else if(TYPE.BISHOP == type)
        {
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int( 1, 1),
                new Vector2Int( 1,-1),
                new Vector2Int(-1, 1),
                new Vector2Int(-1,-1),
            };

            foreach(var v in vec)
            {
                int i = 1;  //iは３マス動かすために定義
                Vector2Int checkpos = Pos + v;
                while(isCheckable(units, checkpos) && i <= 3)  //配列オーバーしていない時にループ
                {
                    //誰かいたら終了（敵のタイルだったら追加して終了）
                    if(null != units[checkpos.x, checkpos.y])
                    {
                        // if(Player != units[checkpos.x, checkpos.y].Player)
                        // {
                        //     ret.Add(checkpos);
                        // }
                        break;
                    }
                    //誰もいない時
                    ret.Add(checkpos);
                    checkpos += v;
                    i++;
                }
            }
        }

        //クイーン  //TYPE5
        else if(TYPE.QUEEN == type)
        {
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-2, 1),
                new Vector2Int( 2, 1),
                new Vector2Int(-1,-2),
                new Vector2Int( 1,-2),
            };

            foreach(var v in vec)
            {
                Vector2Int checkpos = Pos + v;
                if(!isCheckable(units, checkpos)) continue;

                //同じプレイヤーのところは移動できない
                if(null != units[checkpos.x, checkpos.y]/* && Player == units[checkpos.x, checkpos.y].Player*/)
                {
                    continue;
                }
                ret.Add(checkpos);
            }
        }

        //キング  //TYPE6
        else if(TYPE.KING == type)
        {
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1, 1),
                new Vector2Int( 0, 1),
                new Vector2Int( 1, 1),
                new Vector2Int(-1, 0),
                new Vector2Int( 1, 0),
                new Vector2Int(-1,-1),
                new Vector2Int( 0,-1),
                new Vector2Int( 1,-1),
            };

            foreach(var v in vec)
            {
                Vector2Int checkpos = Pos + v;
                if(!isCheckable(units, checkpos)) continue;

                //同じプレイヤーのところへは移動できない
                if(null != units[checkpos.x, checkpos.y]/* && Player == units[checkpos.x, checkpos.y].Player*/)
                {
                    continue;
                }
                ret.Add(checkpos);
            }
        }

        return ret;
    }

    //行動可能範囲の取得
    public List<Vector2Int> UnitsCursorsTiles(UnitController[,] units, bool checkking = true)
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        List<Vector2Int> vec = new List<Vector2Int>()
        {
            new Vector2Int( 1, 1),
            new Vector2Int( 1, 0),
            new Vector2Int( 1,-1),
            new Vector2Int( 0,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int( 0, 1),
            new Vector2Int( 0, 0),
        };

        foreach(var v in vec)
        {
            Vector2Int checkpos = Pos + v;
            if(!isCheckable(units, checkpos)) continue;

            // //同じプレイヤーのところへは移動できない
            // if(null != units[checkpos.x, checkpos.y]/* && Player == units[checkpos.x, checkpos.y].Player*/)
            // {
            //     continue;
            // }
            ret.Add(checkpos);
        }
        return ret;
    }

    // public List<Vector2Int> AttackRange(UnitController[,] units, bool checkking = true)
    // {
    //     if(TYPE.ATTACK == type)
    //     {
    //         List<Vector2Int> vec = new List<Vector2Int>()
    //         {
    //             new Vector2Int(-1, 1),
    //             new Vector2Int( 0, 1),
    //             new Vector2Int( 1, 1),
    //             new Vector2Int(-1, 0),
    //             new Vector2Int( 1, 0),
    //             new Vector2Int(-1,-1),
    //             new Vector2Int( 0,-1),
    //             new Vector2Int( 1,-1),
    //         };
    //         foreach(var v in vec)
    //         {
    //             int i = 1;  //iは３マス動かすために定義
    //             Vector2Int checkpos = Pos + v;
    //             while(isCheckable(units, checkpos) && i <= 3)  //配列オーバーしていない時にループ
    //             {
    //                 //誰かいたら終了（敵のタイルだったら追加して終了）
    //                 if(null != units[checkpos.x, checkpos.y])
    //                 {
    //                     if(Player != units[checkpos.x, checkpos.y].Player)
    //                     {
    //                         // ret.Add(checkpos);
    //                         inAttackRange = true;
    //                     }
    //                     // break;
    //                 }
    //                 //誰もいない時
    //                 // ret.Add(checkpos);
    //                 checkpos += v;
    //                 i++;
    //             }
    //         }
    //     }
    // }

    // bool isMovableTo(Vector2Int pos) {
    //     return pos[0] >= 0 && pos[0] < 8 && pos[1] >= 0 && pos[1] < 8;
    // }
    
    //配列オーバーのチェック
    static bool isCheckable(UnitController[,] ary, Vector2Int idx)
    {
        if( idx.x < 0 || ary.GetLength(0) <= idx.x || idx.y < 0 || ary.GetLength(1) <= idx.y)
        {
            return false;
        }
        return true;
    }

    //バトル開始時のポジション
    public void StartPos()
    {
        Vector3 pos = transform.position;
        //選択したユニットを少し上げる
        pos.y += 10f/*2*/;
        // GetComponent<Rigidbody>().isKinematic = true;  //Kinematicは無重力状態にする

        // //選択解除
        // if(!select)
        // {
        //     pos.y = 1.1f/*1.35f*/;
        //     GetComponent<Rigidbody>().isKinematic = false;
        // }
        transform.position = pos;
    }

    //選択された時の処理
    public void SelectUnit(bool select = true)
    {
        Vector3 pos = transform.position;
        //選択したユニットを少し上げる
        pos.y += 0.5f/*2*/;
        GetComponent<Rigidbody>().isKinematic = true;  //Kinematicは無重力状態にする

        //選択解除
        if(!select)
        {
            pos.y = 1.1f/*1.35f*/;
            GetComponent<Rigidbody>().isKinematic = false;
        }
        transform.position = pos;
    }

    //移動処理
    public void MoveUnit(GameObject tile)
    {
        //コマを移動する時は非選択状態にする
        SelectUnit(false);

        //タイルのポジションから配列の番号に戻す
        Vector2Int idx = new Vector2Int(
            (int)tile.transform.position.x + GameSceneDirector.TILE_X / 2,
            (int)tile.transform.position.z + GameSceneDirector.TILE_Z / 2);

        //新しい場所へ
        Vector3 pos = tile.transform.position;
        pos.y = 0.85f/*1.35f*/;
        transform.position = pos;

        // float distance = Mathf.Sqrt(Mathf.Round(Mathf.Abs(tile.transform.position.x-GameSceneDirector.TILE_X / 2 - transform.position.x)) * 
        //                             Mathf.Round(Mathf.Abs(tile.transform.position.x - GameSceneDirector.TILE_X / 2 - transform.position.x)) + 
        //                             Mathf.Round(Mathf.Abs(tile.transform.position.z-GameSceneDirector.TILE_X / 2 - transform.position.z)) * 
        //                             Mathf.Round(Mathf.Abs(tile.transform.position.z-GameSceneDirector.TILE_X / 2 - transform.position.z)));
        // float speed = distance / 100f;
        // Debug.Log(distance);
        // // Vector3 targetPos = new Vector3(tile.transform.position.x - GameSceneDirector.TILE_X / 2, 0.85f, ttile.transform.position.z - GameSceneDirector.TILE_X / 2);
        // // target.transform.position = targetPos;
        // Vector3 custody = transform.position;
        // while(transform.position != pos)
        // {
        //     transform.position = Vector3.MoveTowards(custody, pos, speed);
        //     await Task.Delay(10);
        //     // Debug.Log(unitpos.transform.position + "   " + target.position);
        //     // if(transform.position == pos)
        //     // {
        //     //     Debug.Log("OK");
        //     //     GameSceneDirector.moveCompleted = true;
        //     //     break;
        //     // }
        // }



        //移動状態をリセット
        Status.Clear();

        //インデックスの更新
        OldPos = Pos;
        Pos = idx;

        //置いてからの経過ターンをリセット
        ProgressTurnCount = 0;
    }

    // public async void MoveGradually(GameObject tile)
    // {
    //     //コマを移動する時は非選択状態にする
    //     SelectUnit(false);

    //     //タイルのポジションから配列の番号に戻す
    //     Vector2Int idx = new Vector2Int(
    //         (int)tile.transform.position.x + GameSceneDirector.TILE_X / 2,
    //         (int)tile.transform.position.z + GameSceneDirector.TILE_Z / 2);

    //     //新しい場所へ
    //     Vector3 pos = tile.transform.position;
    //     pos.y = 0.85f/*1.35f*/;
    //     // transform.position = pos;

    //     float distance = Mathf.Sqrt(Mathf.Round(Mathf.Abs(tile.transform.position.x-GameSceneDirector.TILE_X / 2 - transform.position.x)) * 
    //                                 Mathf.Round(Mathf.Abs(tile.transform.position.x-GameSceneDirector.TILE_X / 2 - transform.position.x)) + 
    //                                 Mathf.Round(Mathf.Abs(tile.transform.position.z-GameSceneDirector.TILE_X / 2 - transform.position.z)) * 
    //                                 Mathf.Round(Mathf.Abs(tile.transform.position.z-GameSceneDirector.TILE_X / 2 - transform.position.z)));
    //     float speed = distance / 100f;
    //     Debug.Log(tile.transform.position.x + "   " + tile.transform.position.z + "      " + transform.position.x + "   " + transform.position.z);
    //     Debug.Log(distance);
    //     // Vector3 targetPos = new Vector3(tile.transform.position.x - GameSceneDirector.TILE_X / 2, 0.85f, ttile.transform.position.z - GameSceneDirector.TILE_X / 2);
    //     // target.transform.position = targetPos;
    //     Vector3 custody = transform.position;
    //     while(transform.position != pos)
    //     {
    //         transform.position = Vector3.MoveTowards(custody, pos, speed);
    //         await Task.Delay(10);
    //         Debug.Log(transform.position + "   " + pos);
            
    //     }



    //     //移動状態をリセット
    //     Status.Clear();
    // }

    //前回移動してからのターンをカウントをする　　　とりあえずポーンがあるアンパッサン用に作成
    public void ProgressTurn()
    {
        //初動は無視
        if(0 > ProgressTurnCount) return;

        ProgressTurnCount++;

        //アンパッサンフラグチェック
        if(Type == TYPE.PAWN)
        {
            if(1 < ProgressTurnCount)
            {
                Status.Remove(STATUS.EN_PASSANT);
            }
        }
    }
}
