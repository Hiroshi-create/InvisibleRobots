using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
// using Photon.Pun;
// using Photon.Realtime;

public class DiceDuplication : MonoBehaviour
{
    //UI関連
    GameObject txtMovePatternInfo;
    // // //たまたまサイコロが立った時もう一回振る
    // private bool returnDiceMode;

    private int? temporaryStorage;

    // Start is called before the first frame update
    void Start()
    {
        //UIオブジェクトの取得
        txtMovePatternInfo = GameObject.Find("TextMovePattern");

        this.gameObject.transform.parent = null;
        //サイコロの目を一旦リセット
        temporaryStorage = null;

        //呼び出された時のRotationのxyz座標をランダムにする
        var xRotate = Random.Range(0.0f, 360.0f);
        var yRotate = Random.Range(0.0f, 360.0f);
        var zRotate = Random.Range(0.0f, 360.0f);
        transform.Rotate(xRotate, yRotate, zRotate);

        StartCoroutine("start");



        var torque = new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f));
        this.GetComponent<Rigidbody>().AddForce(torque, ForceMode.Impulse);
    }

    IEnumerator start()
    {
        //4秒間処理を待たせる
        yield return new WaitForSeconds(1f);

        //たまたまサイコロが立った時もう一回振る
        int time = 0;
        // Debug.Log(x + "   " + z);

        while(temporaryStorage == null)
        {
            //サイコロの出た目を読み取る
            Vector3 eulerAngles = transform.eulerAngles;
            int x = Mathf.RoundToInt(eulerAngles.x);
            int z = Mathf.RoundToInt(eulerAngles.z);

            if(x == 270)
            {
                //1
                temporaryStorage = 1;
                txtMovePatternInfo.GetComponent<Text>().text = "Movement pattern\n1";
                // Debug.Log("Dice1");
            }
            if(x == 90)
            {
                //6
                temporaryStorage = 6;
                txtMovePatternInfo.GetComponent<Text>().text = "Movement pattern\n6";
                // Debug.Log("Dice6");
            }
            if(x == 0)
            {
                if(z == 0)
                {
                    //2
                    temporaryStorage = 2;
                    txtMovePatternInfo.GetComponent<Text>().text = "Movement pattern\n2";
                    // Debug.Log("Dice2");
                }
                if(z == 270)
                {
                    //3
                    if(GameSceneDirector.nowPlayer == GameFSceneDirector.myPlayerNumber)
                    {
                        temporaryStorage = 3;
                    }
                    //敵のターンの時
                    else
                    {
                        temporaryStorage = 5;
                    }
                    
                    txtMovePatternInfo.GetComponent<Text>().text = "Movement pattern\n3";
                    // Debug.Log("Dice3");
                }
                if(z == 90)
                {
                    //4
                    temporaryStorage = 4;
                    txtMovePatternInfo.GetComponent<Text>().text = "Movement pattern\n4";
                    // Debug.Log("Dice4");
                }
                if(z == 180)
                {
                    //5
                    if(GameSceneDirector.nowPlayer == GameFSceneDirector.myPlayerNumber)
                    {
                        temporaryStorage = 5;
                    }
                    //敵のターンの時
                    else
                    {
                        temporaryStorage = 3;
                    }
                    
                    txtMovePatternInfo.GetComponent<Text>().text = "Movement pattern\n5";
                    // Debug.Log("Dice5");
                }
            }

            //サイコロが立った時用に数秒後にもう一度サイコロを振る処理
            if(time == 30)
            {
                //サイコロをもう一回振る
                // returnDiceMode = true;
                GameSceneDirector.diceModeOneLoop = false;
                break;
            }
            if(temporaryStorage != null) break;
            yield return new WaitForSeconds(0.1f);
            time++;
        }

        if(GameSceneDirector.diceModeOneLoop == true)
        {
            yield return new WaitForSeconds(1f);
            //出た目に対応するタイプを代入
            GameSceneDirector.selectedBehaviour = (UnitController.TYPE)Enum.ToObject(typeof(UnitController.TYPE), temporaryStorage);
            GameSceneDirector.diceModeStop = true;
        }

        //サイコロのオブジェクトを削除
        Destroy(this.gameObject);
    }
}
