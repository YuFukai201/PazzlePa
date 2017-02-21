using UnityEngine;
using System.Collections;

public class Anvil_Script : MonoBehaviour {


    //誘爆のときにこいつを無理やりtrueにする？
    public bool OnClick_flag = false;

    Score_Manager _scoreManager;

    GameObject mainObj;

    [SerializeField]
    float Rotation_Speed = 2f;

    bool GameOver;

    obj_float p_objFloat;

    bool _touch = false;

    // Use this for initialization
    void Start()
    {

        mainObj = GameObject.Find("balloon_main_obj");

        _scoreManager = mainObj.GetComponent<Score_Manager>();

        p_objFloat = transform.parent.gameObject.GetComponent<obj_float>();

    }
        // Update is called once per frame
        void Update () {

        //ポーズなら停止
        if (common.mode_pose) return;

        gameObject.transform.Rotate(new Vector3(0, 0, Rotation_Speed + Time.deltaTime));
      

        /* //このオブジェクトが押されたら
         if (Input.GetMouseButtonDown(0))
         {
             GameOver = mainObj.GetComponent<tmtm_main>().GameStop_Flag;

             //マウスの位置取得
             RaycastHit2D mouse_hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
             if (mouse_hit.collider != null && !GameOver)
             {
                 //最初に触ったオブジェクトの取得
                 GameObject hit_obj = mouse_hit.collider.gameObject;
                 if (hit_obj == gameObject && !OnClick_flag)
                 {
                     OnClick_flag = true;
                     Rotation_Speed = 0;
                     GetComponent<Rigidbody2D>().gravityScale = 5f;

                 }
             }
         }
         */
        //押されたら
        if (p_objFloat.Touch_Flag)
        {
            if (_touch) return;

            Rotation_Speed = 0;
            GetComponent<Rigidbody2D>().gravityScale = 5f;
            transform.parent = null;
            _touch = true;
        }
        //シャボンの中
        else
        {
            //親に追従
            transform.position = transform.parent.gameObject.transform.position;
        }

    }

    //バグ回避

    private void OnCollisionStay2D(Collision2D col)
    {
        if (p_objFloat.Touch_Flag)
        {
            if (col.gameObject == null) return;

            if (col.gameObject.tag == "balloon" || col.gameObject.tag == "Special")
            {

                //引数に特殊風船の倍率を
                col.gameObject.GetComponent<obj_float>()._Destroy(2.0f);
            }
            else if (gameObject.transform.position.y <= -9f)
            {
                OnClick_flag = false;

                //common.all_ball--;

                //特殊風船の単スコア追加
                _scoreManager.Balloon_Plus(this.name);

                Destroy(this.gameObject, 0.1f);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (p_objFloat.Touch_Flag)
        {
            if (col.gameObject == null) return;

            if (col.gameObject.tag == "balloon")
            {
                //引数に特殊風船の倍率を
                col.gameObject.GetComponent<obj_float>()._Destroy(2.0f);
            }
            else if (col.gameObject.tag == "Special")
            {
                if(col.transform.position.x < transform.position.x)
                //オブジェクトが引きずり込まれるので応急措置
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10, 0), ForceMode2D.Impulse);
                if (col.transform.position.x > transform.position.x)
                //オブジェクトが引きずり込まれるので応急措置
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(10, 0), ForceMode2D.Impulse);

            }
            else if (gameObject.transform.position.y <= -9f)
            {
                OnClick_flag = false;

                //common.all_ball--;

                //特殊風船の単スコア追加
                _scoreManager.Balloon_Plus(this.name);

                Destroy(this.gameObject, 0.1f);
            }
        }

    }






}
