using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    //このユニットのプレイヤー番号
    static public int nowPlayer;

    static public bool inAttackRange;
    //trueの時に攻撃
    static public bool inAttackRange1;
    static public bool inAttackRange2;
    static public bool inAttackRange3;
    static public bool inAttackRange4;
    static public bool inAttackRange5;
    static public bool inAttackRange6;
    static public bool inAttackRange7;
    static public bool inAttackRange8;
    static public bool inAttackRange9;
    static public bool inAttackRange10;
    static public bool inAttackRange11;
    static public bool inAttackRange12;
    static public bool inAttackRange13;
    static public bool inAttackRange14;
    static public bool inAttackRange15;
    static public bool inAttackRange16;

    // static public bool attackOneLoop;

    private Camera mainCamera;
    [SerializeField]
    static public int attackPower = 500;

    // //Start is called before the first frame update
    // void Start()
    // {
    // }

    //Update is called once per frame
    void Update()
    {
        if(inAttackRange == true)
        {
            inAttackRange1 = true;
            inAttackRange2 = true;
            inAttackRange3 = true;
            inAttackRange4 = true;
            inAttackRange5 = true;
            inAttackRange6 = true;
            inAttackRange7 = true;
            inAttackRange8 = true;
            inAttackRange9 = true;
            inAttackRange10 = true;
            inAttackRange11 = true;
            inAttackRange12 = true;
            inAttackRange13 = true;
            inAttackRange14 = true;
            inAttackRange15 = true;
            inAttackRange16 = true;

            StartCoroutine("start");
            // oneLoop = true;
            inAttackRange = false;
        }
        
    }


    IEnumerator start()
    {
        yield return new WaitForSeconds(0f);

        // if(Input.GetMouseButtonDown(0))
        // if(Input.GetKey(KeyCode.Space))
        // if(inAttackRange = true)
        // {
        Vector3 origin = new Vector3(GameSceneDirector.moved_X-GameSceneDirector.TILE_X / 2, 1, GameSceneDirector.moved_Z-GameSceneDirector.TILE_Z / 2);

        if(nowPlayer == 0)
        {
            // if(Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Player")))
            // if(Physics.Raycast(origin, Vector3.right, out RaycastHit hit, 1f, LayerMask.GetMask("Player")))  //gameObject.transform.position
            Vector3 direction = new Vector3(0, 0, 1);
            Ray ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit, 1.2f, LayerMask.GetMask("InvalidWhite")) && inAttackRange1 == true)
            {
                hit.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange1 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);

            direction = new Vector3(0, 0, -1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit1, 1.2f, LayerMask.GetMask("InvalidWhite")) && inAttackRange2 == true)
            {
                hit1.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange2 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);

            direction = new Vector3(1, 0, 0);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit2, 1.2f, LayerMask.GetMask("InvalidWhite")) && inAttackRange3 == true)
            {
                hit2.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange3 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);

            direction = new Vector3(-1, 0, 0);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit3, 1.2f, LayerMask.GetMask("InvalidWhite")) && inAttackRange4 == true)
            {
                hit3.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange4 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);


            direction = new Vector3(1, 0, 1);
            ray = new Ray(origin, direction);
            //斜め方向のレーザー
            if(Physics.Raycast(ray, out RaycastHit hit4, 1.5f, LayerMask.GetMask("InvalidWhite")) && inAttackRange5 == true)
            {
                hit4.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange5 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);

            direction = new Vector3(1, 0, -1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit5, 1.5f, LayerMask.GetMask("InvalidWhite")) && inAttackRange6 == true)
            {
                hit5.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange6 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);

            direction = new Vector3(-1, 0, 1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit6, 1.5f, LayerMask.GetMask("InvalidWhite")) && inAttackRange7 == true)
            {
                hit6.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange7 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);

            direction = new Vector3(-1, 0, -1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit7, 1.5f, LayerMask.GetMask("InvalidWhite")) && inAttackRange8 == true)
            {
                hit7.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange8 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);
        }
        else if(nowPlayer == 1)
        {
            // if(Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Player")))
            // if(Physics.Raycast(origin, Vector3.right, out RaycastHit hit, 1f, LayerMask.GetMask("Player")))  //gameObject.transform.position
            Vector3 direction = new Vector3(0, 0, 1);
            Ray ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit, 1.2f, LayerMask.GetMask("Player1")) && inAttackRange9 == true)
            {
                hit.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange9 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);

            direction = new Vector3(0, 0, -1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit1, 1.2f, LayerMask.GetMask("Player1")) && inAttackRange10 == true)
            {
                hit1.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange10 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);

            direction = new Vector3(1, 0, 0);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit2, 1.2f, LayerMask.GetMask("Player1")) && inAttackRange11 == true)
            {
                hit2.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange11 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);

            direction = new Vector3(-1, 0, 0);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit3, 1.2f, LayerMask.GetMask("Player1")) && inAttackRange12 == true)
            {
                hit3.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange12 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.2f, Color.red, 5);


            direction = new Vector3(1, 0, 1);
            ray = new Ray(origin, direction);
            //斜め方向のレーザー
            if(Physics.Raycast(ray, out RaycastHit hit4, 1.5f, LayerMask.GetMask("Player1")) && inAttackRange13 == true)
            {
                hit4.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange13 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);

            direction = new Vector3(1, 0, -1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit5, 1.5f, LayerMask.GetMask("Player1")) && inAttackRange14 == true)
            {
                hit5.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange14 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);

            direction = new Vector3(-1, 0, 1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit6, 1.5f, LayerMask.GetMask("Player1")) && inAttackRange15 == true)
            {
                hit6.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange15 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);

            direction = new Vector3(-1, 0, -1);
            ray = new Ray(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit7, 1.5f, LayerMask.GetMask("Player1")) && inAttackRange16 == true)
            {
                hit7.transform.GetComponent<EnemyHPManager>().TakeDamage(attackPower);
                inAttackRange16 = false;
            }
            // Debug.DrawRay(ray.origin, ray.direction*1.5f, Color.red, 5);
        }

        yield return new WaitForSeconds(2f);
        
        //どれにもヒットしなかった時
        if(inAttackRange1 == true && inAttackRange2 == true && inAttackRange3 == true && inAttackRange4 == true && 
           inAttackRange5 == true && inAttackRange6 == true && inAttackRange7 == true && inAttackRange8 == true)
        {
            // GameSceneDirector.dieUnit = "doNotAttack";
            if(GameSceneDirector.dieUnit == null) GameSceneDirector.dieUnit = "doNotAttack";
            GameSceneDirector.hpProcessingCompleted = true;
        }
        if(inAttackRange9  == true && inAttackRange10 == true && inAttackRange11 == true && inAttackRange12 == true && 
           inAttackRange13 == true && inAttackRange14 == true && inAttackRange15 == true && inAttackRange16 == true)
        {
            // GameSceneDirector.dieUnit = "doNotAttack";
            if(GameSceneDirector.dieUnit == null) GameSceneDirector.dieUnit = "doNotAttack";
            GameSceneDirector.hpProcessingCompleted = true;
        }

        if(inAttackRange1  == true) inAttackRange1  = false;
        if(inAttackRange2  == true) inAttackRange2  = false; 
        if(inAttackRange3  == true) inAttackRange3  = false; 
        if(inAttackRange4  == true) inAttackRange4  = false; 
        if(inAttackRange5  == true) inAttackRange5  = false; 
        if(inAttackRange6  == true) inAttackRange6  = false; 
        if(inAttackRange7  == true) inAttackRange7  = false; 
        if(inAttackRange8  == true) inAttackRange8  = false; 
        
        if(inAttackRange9  == true) inAttackRange9  = false; 
        if(inAttackRange10 == true) inAttackRange10 = false; 
        if(inAttackRange11 == true) inAttackRange11 = false; 
        if(inAttackRange12 == true) inAttackRange12 = false; 
        if(inAttackRange13 == true) inAttackRange13 = false; 
        if(inAttackRange14 == true) inAttackRange14 = false; 
        if(inAttackRange15 == true) inAttackRange15 = false; 
        if(inAttackRange16 == true) inAttackRange16 = false; 

        inAttackRange = false;
    }
}