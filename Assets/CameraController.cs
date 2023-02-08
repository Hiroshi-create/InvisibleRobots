using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject subCamera;

    //ゲーム設定
    public const int TILE_X = 9;
    public const int TILE_Z = 9;

    public bool AutoRotate = false;
    //ここを中心にカメラを回す
    Vector3 lookatPosition = new Vector3(0, 0, 0);
    Vector2 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerNum();
        mainCamera = GameObject.Find("MainCamera");
        subCamera = GameObject.Find("SubCamera");

        if(playerNum() == 0 || TitleSceneDirector.titleSceneCamera == 3)
        {
            mainCamera.SetActive(true);
            subCamera.SetActive(false);
        }

        if(playerNum() == 1)
        {
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(AutoRotate)
        {
            transform.RotateAround(lookatPosition, new Vector3(0, 1, 0), 0.1f);
            return;
        }

        //プレイヤーの操作
        if(Input.GetMouseButtonDown(0))  //マウスが押されていたらカメラを回転させる
        {
            prevPosition = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))  //ドラックされているとき
        {
            float val = prevPosition.x - Input.mousePosition.x;
            val *= -0.1f;  //移動量を調節

            //場所、回転軸、移動量を指定
            transform.RotateAround(lookatPosition, new Vector3(0, 1, 0), val);
            prevPosition = Input.mousePosition;
        }
    }

    int playerNum()
    {
        for(int i = 0; i < TILE_X; i++)
        {
            for(int j = 0; j < TILE_Z; j++)
            {
                return GameFSceneDirector.unitType[i, j] / 10;
            }
        }

        return 1;
    }
}
