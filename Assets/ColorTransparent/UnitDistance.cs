using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDistance : MonoBehaviour
{
    //trueの時に見えるようになる
    static public bool changeTransparencyMT;
    static public bool changeTransparencyYT;

    public List<GameObject> WhiteTrans;
    public List<GameObject> WhiteEntity;
    public List<GameObject> BlackTrans;
    public List<GameObject> BlackEntity;


    // public enum UnitGroup {
    //     IS_MINE,
    //     IS_ENEMY,
    //     NONE
    // }

    // [SerializeField] UnitGroup group;

    //マテリアルを読み込む
    public Material[] ColorChenge = new Material[4];
    // public Material mat;///
    // public Material[] mats;///

    //ユニットの数
    private int numberOfUnits = 7;

    //見えるようになる距離
    private float distan = 1.5f;

    private float xpos;
    private float zpos;
    //選択、移動したユニットのX,Z座標
    private float movedXpos;
    private float movedZpos;

    //ループ数
    // private int roop;

    // Start is called before the first frame update
    void Start()
    {
        WhiteTrans = new List<GameObject>();  //初期化
        WhiteEntity = new List<GameObject>();  //初期化
        BlackTrans = new List<GameObject>();  //初期化
        BlackEntity = new List<GameObject>();  //初期化
        // // mats = GetComponent<SkinnedMeshRenderer>().materials;
        // mats[3] = mat;
        // GetComponent<SkinnedMeshRenderer>().materials = mats;
        // var mat = Resources.Load<Material>("Materials/mat");
        // var mats = this.GetComponent<MeshRenderer>().materials;
        // for(int i = 0; i < this.GetComponent<MeshRenderer>().materials.Length; i++)
        // {
        //     mats[i] = mat;
        // }
        // this.GetComponent<MeshRenderer>().materials = mats;



        //  TODO  自分の初期の色は見える
        TransChangeWcore.transChangeWcore = true;
        TransChangeW1.transChangeW1 = true;
        TransChangeW2.transChangeW2 = true;
        TransChangeW3.transChangeW3 = true;
        TransChangeW4.transChangeW4 = true;
        TransChangeW5.transChangeW5 = true;
        TransChangeW6.transChangeW6 = true;
        // GetComponent<MeshRenderer>().material = ColorChenge[0];
        // GetComponent<SkinnedMeshRenderer>().material = ColorChenge[0];
        // if(ClickAttackScript.nowPlayer == 0) GetComponent<MeshRenderer>().material = ColorChenge[3];

        //相手の初期の色は透明
        // Vector3 direction = new Vector3(0, 0, 1);
        // for(float i = -4.0f; i <= 4.0f; i+=1.0f)
        // {
            //始点
            // Vector3 origin = new Vector3(i, 1.0f, -4.5f);
            // Ray ray = new Ray(origin, direction);
            // // if(Physics.Raycast(ray, out RaycastHit hit, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player1")))
            // foreach(RaycastHit hitBlack in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player1")))
            // {
                //  TODO  見えない敵
                TransChangeBcore.transChangeBcore = false;
                TransChangeB1.transChangeB1 = false;
                TransChangeB2.transChangeB2 = false;
                TransChangeB3.transChangeB3 = false;
                TransChangeB4.transChangeB4 = false;
                TransChangeB5.transChangeB5 = false;
                TransChangeB6.transChangeB6 = false;
                
                // hitBlack.collider.gameObject.GetComponent<SkinnedMeshRenderer>().material = ColorChenge[2];
                // hitBlack.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[2];
            // }
            // Debug.DrawRay(ray.origin, ray.direction * (float)GameSceneDirector.TILE_Z, Color.red, 5);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        //選択したユニットのX,Z座標
        movedXpos = (float)GameSceneDirector.moved_X - GameSceneDirector.TILE_X / 2;
        movedZpos = (float)GameSceneDirector.moved_Z - GameSceneDirector.TILE_Z / 2;

        //自分のターンで表示するかしないか
        if(changeTransparencyMT == true)
        // if(Input.GetKey(KeyCode.Space))
        {
            //LayerがInvalidのユニットをPlayerに戻す
            CheckLayer();
            // for(roop = 0; roop <= numberOfUnits; roop++)
            // {
                //全てのユニットペアでチェックしてユニットとの距離に応じて表示するかしないか
                EachMyUnits("Player2", "InvalidWhite");
                //動かしたユニットの分も処理
                // MovedMyUnit(/*movedXpos, movedZpos, */"Player1", "InvalidBlack");  //これをしなくてもいいように
            // }
            // Debug.Log("changeTransparencyMT is TRUE");
        }
        // else{Debug.Log("changeTransparencyMT is FALSE");}

        //相手のターンで表示するかしないか
        if(changeTransparencyYT == true)
        // if(Input.GetKey(KeyCode.Space))
        {
            //LayerがInvalidのユニットをPlayerに戻す
            CheckLayer();
            // for(roop = 0; roop <= numberOfUnits; roop++)
            // {
                //全てのユニットペアでチェックしてユニットとの距離に応じて表示するかしないか
                EachMyUnits("Player2", "InvalidWhite");
                //動かしたユニットの分も処理
                // MovedEnemyUnit(movedXpos, movedZpos, "Player2", "InvalidWhite");
            // }
            // Debug.Log("changeTransparencyYT is TRUE");
        }
        // else{Debug.Log("changeTransparencyYT is FALSE");}
    }

    /**
     * 自身のユニットを透明化するかしないかを判定メソッド
     */
    // bool IsTransparency(float distanceBetweenUnits, float constantDistance = 3.0f) {
    //     return group != UnitGroup.IS_MINE && distanceBetweenUnits <= constantDistance;
    // }

    //LayerがInvalidのユニットをPlayerに戻す
    void CheckLayer()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        for(float i = -4.0f; i <= 4.0f; i+=1.0f)
        {
            //始点
            Vector3 origin = new Vector3(i, 1.0f, -4.5f);
            Ray ray = new Ray(origin, direction);

            //敵ユニットを戻す
            // if(Physics.Raycast(ray, out RaycastHit hitBlack, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidBlack")))
            foreach(RaycastHit hitBlack in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidBlack")))
            {
                //敵の全ユニットをPlayer1に戻す
                hitBlack.collider.gameObject.layer = LayerMask.NameToLayer("Player1");
                

                // hitBlack.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[3];
                // hitBlack.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[3];
            }

            //自分のユニットを戻す
            // if(Physics.Raycast(ray, out RaycastHit hitWhite, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidWhite")))
            foreach(RaycastHit hitWhite in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidWhite")))
            {
                //自分の全ユニットをPlayer2に戻す
                hitWhite.collider.gameObject.layer = LayerMask.NameToLayer("Player2");
            }
            Debug.DrawRay(ray.origin, ray.direction * (float)GameSceneDirector.TILE_Z, Color.green, 5);
        }
        //  TODO  敵の全ユニットを 見えない敵 に一度戻す
        TransChangeBcore.transChangeBcore = false;
        TransChangeB1.transChangeB1 = false;
        TransChangeB2.transChangeB2 = false;
        TransChangeB3.transChangeB3 = false;
        TransChangeB4.transChangeB4 = false;
        TransChangeB5.transChangeB5 = false;
        TransChangeB6.transChangeB6 = false;
    }

    //全てのユニットペアでチェック
    void EachMyUnits(string originLayer, string targetLayer)
    {
        //方向
        Vector3 direction = new Vector3(0, 0, 1);
        for(float i = -4.0f; i <= 4.0f; i+=1.0f)
        {
            //始点
            Vector3 origin = new Vector3(i, 1.0f, -4.5f);
            Ray ray = new Ray(origin, direction);
                
            // if(Physics.Raycast(ray, out RaycastHit hit, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player2")))
            foreach(RaycastHit hit in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask(originLayer/*"Player2"*/)))
            {
                //レーザーに当たったユニットのX,Z座標を受け取る
                float selectUnitX = hit.collider.gameObject.transform.position.x;
                float selectUnitZ = hit.collider.gameObject.transform.position.z;
                // Debug.Log(hit.point.x + "  " + hit.point.z);
                // Debug.Log(movedXpos + "  " + movedZpos + "   " + hit.point.x + "  " + hit.point.z);
                
                // if(changeTransparencyMT == true)
                // {
                    IfColorChenge(selectUnitX, selectUnitZ, originLayer/*"Player2"*/, targetLayer/*"InvalidWhite"*/);
                // }

                // if(changeTransparencyYT == true)
                // {
                //     IfColorChenge(selectUnitX, selectUnitZ, originLayer, targetLayer);
                //     // MovedEnemyUnit(selectUnitX, selectUnitZ, originLayer, targetLayer);
                // }

                //多段しないようにLayerを変更
                // if(roop == 7)
                // {
                    hit.collider.gameObject.layer = LayerMask.NameToLayer(targetLayer/*"InvalidWhite"*/);
                // }

                // unitsPos.Add (new Vector3(hit.point.x, hit.point.y, hit.point.z));
                // Debug.Log(unitsPos);
            }
            Debug.DrawRay(ray.origin, ray.direction * (float)GameSceneDirector.TILE_Z, Color.red, 5);
        }
    }

    //ユニットとの距離に応じて表示するかしないか
    void IfColorChenge(float xpos, float zpos, string originLayer, string targetLayer)
    {
        //Player2をPlayer1に、InvalidWhiteをInvalidBlackに
        string OriginLayer = originLayer == "Player1" ? "Player2": "Player1";
        string TargetLayer = targetLayer == "InvalidWhite" ? "InvalidBlack": "InvalidWhite";
        //方向
        Vector3 direction = new Vector3(0, 0, 1);
        for(float i = -4.0f; i <= 4.0f; i+=1.0f)
        {
            //始点
            Vector3 origin = new Vector3(i, 1.0f, -4.5f);
            Ray ray = new Ray(origin, direction);
            // if(Physics.Raycast(ray, out RaycastHit hit, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player1")))
            foreach(RaycastHit hit in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask(OriginLayer/*"Player1"*/)))
            {
                //レーザーに当たったユニットのX,Z座標を受け取る
                float selectUnitX = hit.collider.gameObject.transform.position.x;
                float selectUnitZ = hit.collider.gameObject.transform.position.z;

                //距離を求める
                float distance = Mathf.Sqrt(Mathf.Round(Mathf.Abs(xpos - selectUnitX)) * Mathf.Round(Mathf.Abs(xpos - selectUnitX)) + 
                                            Mathf.Round(Mathf.Abs(zpos - selectUnitZ)) * Mathf.Round(Mathf.Abs(zpos - selectUnitZ)));
                // Debug.Log(xpos + "  " + zpos + "    " + selectUnitX + "  " + selectUnitZ + " → " + distance);

                //どのくらい離れているユニットを出現させるか
                if(distance <= distan/*IsTransparency(distance, distan)*/)
                {
                    //  TODO  見える敵
                    // Debug.Log(hit.collider.gameObject.tag);
                    if(hit.collider.gameObject.tag == "NucleusBlack") { TransChangeBcore.transChangeBcore = true; }
                    else if(hit.collider.gameObject.tag == "PlayerB1") { TransChangeB1.transChangeB1 = true; }
                    else if(hit.collider.gameObject.tag == "PlayerB2") { TransChangeB2.transChangeB2 = true; }
                    else if(hit.collider.gameObject.tag == "PlayerB3") { TransChangeB3.transChangeB3 = true; }
                    else if(hit.collider.gameObject.tag == "PlayerB4") { TransChangeB4.transChangeB4 = true; }
                    else if(hit.collider.gameObject.tag == "PlayerB5") { TransChangeB5.transChangeB5 = true; }
                    else if(hit.collider.gameObject.tag == "PlayerB6") { TransChangeB6.transChangeB6 = true; }
                    // hit.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[2];
                }
                // else
                // {
                //     hit.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[3];
                // }
                //多段しないようにLayerを変更
                hit.collider.gameObject.layer = LayerMask.NameToLayer(TargetLayer/*"InvalidBlack"*/);

                // //多段しないようにLayerを変更
                // if(roopNum >= numberOfUnits)
                // {
                //     hit.collider.gameObject.layer = LayerMask.NameToLayer(TargetLayer/*"InvalidBlack"*/);
                // }
                
                
                // unitsPos.Add (new Vector3(hit.point.x, hit.point.y, hit.point.z));
                // Debug.Log(unitsPos);
                Debug.DrawRay(ray.origin, ray.direction * (float)GameSceneDirector.TILE_Z, Color.red, 5);
            }
        }
        EnemyReset();
        changeTransparencyMT = false;
        changeTransparencyYT = false;
    }

    // //動かしたユニットの分も処理
    // void MovedMyUnit(/*float xpos, float zpos, */string originLayer, string targetLayer)
    // {
    //     //方向
    //     Vector3 direction = new Vector3(0, 0, 1);
    //     for(float i = -4.0f; i <= 4.0f; i+=1.0f)
    //     {
    //         //始点
    //         Vector3 origin = new Vector3(i, 1.0f, -4.5f);
    //         Ray ray = new Ray(origin, direction);
    //         // if(Physics.Raycast(ray, out RaycastHit hit, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player1")))
    //         foreach(RaycastHit hit in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask(originLayer/*"Player1"*/)))
    //         {
    //             //レーザーに当たったユニットのX,Z座標を受け取る
    //             float selectUnitX = hit.collider.gameObject.transform.position.x;
    //             float selectUnitZ = hit.collider.gameObject.transform.position.z;

    //             //距離を求める
    //             float distance = Mathf.Sqrt(Mathf.Round(Mathf.Abs(movedXpos - selectUnitX)) * Mathf.Round(Mathf.Abs(movedXpos - selectUnitX)) + 
    //                                         Mathf.Round(Mathf.Abs(movedZpos - selectUnitZ)) * Mathf.Round(Mathf.Abs(movedZpos - selectUnitZ)));
    //             // Debug.Log(movedXpos + "  " + movedZpos + "    " + selectUnitX + "  " + selectUnitZ + " → " + distance);

    //             //どのくらい離れているユニットを出現させるか
    //             if(distance <= distan/*IsTransparency(distance, distan)*/)
    //             {
    //                 //  TODO  見える敵
    //                 // Debug.Log(hit.collider.gameObject.tag);
    //                 if(hit.collider.gameObject.tag == "NucleusBlack") { TransChangeBcore.transChangeBcore = true; }
    //                 else if(hit.collider.gameObject.tag == "PlayerB1") { TransChangeB1.transChangeB1 = true; }
    //                 else if(hit.collider.gameObject.tag == "PlayerB2") { TransChangeB2.transChangeB2 = true; }
    //                 else if(hit.collider.gameObject.tag == "PlayerB3") { TransChangeB3.transChangeB3 = true; }
    //                 else if(hit.collider.gameObject.tag == "PlayerB4") { TransChangeB4.transChangeB4 = true; }
    //                 else if(hit.collider.gameObject.tag == "PlayerB5") { TransChangeB5.transChangeB5 = true; }
    //                 else if(hit.collider.gameObject.tag == "PlayerB6") { TransChangeB6.transChangeB6 = true; }
    //                 // Debug.Log(v.transform.gameObject.tag);
    //                 // hit.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[2];
    //             }
    //             // else
    //             // {
    //             //     hit.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[3];
    //             // }

    //             //多段しないようにLayerを変更
    //             // if(roop == 7)
    //             // {
    //                 hit.collider.gameObject.layer = LayerMask.NameToLayer(targetLayer/*"InvalidBlack"*/);
    //             // }

    //             // unitsPos.Add (new Vector3(hit.point.x, hit.point.y, hit.point.z));
    //             // Debug.Log(unitsPos);
    //         }
    //         Debug.DrawRay(ray.origin, ray.direction * (float)GameSceneDirector.TILE_Z, Color.red, 5);
    //     }
    //     changeTransparencyMT = false;
    // }

    // //敵ユニットが動かした時
    // void MovedEnemyUnit(float xpos, float zpos, string originLayer, string targetLayer)
    // {
    //     //方向
    //     Vector3 direction = new Vector3(0, 0, 1);
    //     for(float i = -4.0f; i <= 4.0f; i+=1.0f)
    //     {
    //         //始点
    //         Vector3 origin = new Vector3(i, 1.0f, -4.5f);
    //         Ray ray = new Ray(origin, direction);
    //         // if(Physics.Raycast(ray, out RaycastHit hit, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidWhite")))
    //         foreach(RaycastHit hit in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask(originLayer/*"Player2"*/)))
    //         {
    //             //レーザーに当たったユニットのX,Z座標を受け取る
    //             float selectUnitX = hit.collider.gameObject.transform.position.x;
    //             float selectUnitZ = hit.collider.gameObject.transform.position.z;
    //             // float selectUnitX = hitWhite.point.x;
    //             // float selectUnitZ = hitWhite.point.z;

    //             //距離を求める
    //             float distance = Mathf.Sqrt(Mathf.Round(Mathf.Abs(xpos - selectUnitX)) * Mathf.Round(Mathf.Abs(xpos - selectUnitX)) + 
    //                                         Mathf.Round(Mathf.Abs(zpos - selectUnitZ)) * Mathf.Round(Mathf.Abs(zpos - selectUnitZ)));
    //             // if(roop == 7)
    //             // {
    //                 // Debug.Log(movedXpos + "  " + movedZpos + "  " + hit.point.x + "  " + hit.point.z);
    //             // }
    //             // Debug.Log(movedXpos + "  " + movedZpos);

    //             //どのくらい離れているユニットを出現させるか
    //             if(distance <= distan/*IsTransparency(distance, distan)*/)
    //             {
    //                 //  TODO  見える敵
    //                 // hit.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[2];
    //                 // this.GetComponent<SkinnedMeshRenderer>().material = ColorChenge[2];
    //                 // // GetComponent<MeshRenderer>().material = ColorChenge[2];
    //                 // hit.collider.gameObject.layer = LayerMask.NameToLayer("Player1");
    //             }
    //             else
    //             {
    //                 //透明の黒
    //                 // hit.collider.gameObject.GetComponent<MeshRenderer>().material = ColorChenge[3];
    //                 // // GetComponent<MeshRenderer>().material = ColorChenge[3];
    //                 // hit.collider.gameObject.layer = LayerMask.NameToLayer("Player1");
    //             }

    //             // //多段しないようにLayerを変更
    //             // if(roop == 7)
    //             // {
    //             //     hit.collider.gameObject.layer = LayerMask.NameToLayer("InvalidBlack");
    //             // }

    //             // unitsPos.Add (new Vector3(hit.point.x, hit.point.y, hit.point.z));
    //             // Debug.Log(unitsPos);
    //         }
    //         // if(Physics.Raycast(ray, out RaycastHit hitWhite, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player2")))
    //         // foreach(RaycastHit hitWhite in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("Player2")))
    //         // {
    //         //     //レーザーに当たったユニットのX,Z座標を受け取る
    //         //     float selectUnitX = hitWhite.point.x;
    //         //     float selectUnitZ = hitWhite.point.z;

    //         //     float distance = Mathf.Sqrt(Mathf.Ceil(Mathf.Abs(movedXpos - selectUnitX)) * Mathf.Ceil(Mathf.Abs(movedXpos - selectUnitX)) + 
    //         //                                 Mathf.Ceil(Mathf.Abs(movedZpos - selectUnitZ)) * Mathf.Ceil(Mathf.Abs(movedZpos - selectUnitZ)));
    //         //     // if(roop == 7)
    //         //     // {
    //         //         // Debug.Log(movedXpos + "  " + movedZpos + "  " + hitWhite.point.x + "  " + hitWhite.point.z);
    //         //     // }
    //         // }
    //         Debug.DrawRay(ray.origin, ray.direction * (float)GameSceneDirector.TILE_Z, Color.red, 5);
    //     }
    //     changeTransparencyYT = false;
    // }

    //敵ユニットをPlayer1に戻す
    void EnemyReset()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        for(float i = -4.0f; i <= 4.0f; i+=1.0f)
        {
            //始点
            Vector3 origin = new Vector3(i, 1.0f, -4.5f);
            Ray ray = new Ray(origin, direction);

            //敵ユニットを戻す
            // if(Physics.Raycast(ray, out RaycastHit hitBlack, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidBlack")))
            foreach(RaycastHit hitBlack in Physics.RaycastAll(ray, (float)GameSceneDirector.TILE_Z, LayerMask.GetMask("InvalidBlack")))
            {
                //敵の全ユニットをPlayer1に戻す
                hitBlack.collider.gameObject.layer = LayerMask.NameToLayer("Player1");
            }
        }
    }
}

