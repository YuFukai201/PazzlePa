using UnityEngine;
using System.Collections;


public class Rocket_Script : MonoBehaviour {
    [SerializeField]
    float Rotation_Speed = 2f;

    //Balloon_Parents Balloon_pr;

    //誘爆のときにこいつを無理やりtrueにする？
    public bool OnClick_flag = false;

    Score_Manager _scoreManager;

    GameObject mainObj;

    bool GameOver;

    obj_float p_objFloat;

    bool _touch = false;

    // Use this for initialization
    void Start()
    {

        mainObj = GameObject.Find("balloon_main_obj");

        _scoreManager = mainObj.GetComponent<Score_Manager>();

        // Balloon_pr = GetComponent<Balloon_Parents>();

        p_objFloat = transform.parent.gameObject.GetComponent<obj_float>();
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        //ポーズなら停止
        if (common.mode_pose) return;

        gameObject.transform.Rotate(new Vector3(0, 0, Rotation_Speed + Time.deltaTime));

        if (p_objFloat.Touch_Flag && !common.mode_Tstop)
        {
            //発射
            GetComponent<Rigidbody2D>().AddForce(transform.up * 30f, ForceMode2D.Force);
        }

        /*
        //このオブジェクトが押されたら
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

                    Rotation_Speed = 0f;

                    gameObject.GetComponent<CircleCollider2D>().enabled = false;
                }
            }
        }*/

        //押されたら
        if (p_objFloat.Touch_Flag)
        {
            if (_touch) return;

            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<CapsuleCollider2D>().enabled = true;
            Rotation_Speed = 0;
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




    //バグ回避用
    private void OnCollisionStay2D(Collision2D col)
    {
        if (p_objFloat.Touch_Flag)
        {
            if (col.gameObject == null) return;

            if (col.gameObject.tag == "balloon" || col.gameObject.tag == "Special")
            {
                //ロケット処理
                //引数に特殊風船の倍率を
                col.gameObject.GetComponent<obj_float>()._Destroy(1.8f);
            }
            else
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

            if (col.gameObject.tag == "balloon" || col.gameObject.tag == "Special")
            {
                //ロケット処理
                //引数に特殊風船の倍率を
                col.gameObject.GetComponent<obj_float>()._Destroy(1.8f);
            }
            else
            {
                OnClick_flag = false;

               // common.all_ball--;

                //特殊風船の単スコア追加
                _scoreManager.Balloon_Plus(this.name);

                Destroy(this.gameObject, 0.1f);
            }
        }

    }
    

    /*
    private void OnTriggerStay2D(Collider2D col)
    {

        //クリックされたら
        if (OnClick_flag)
        {
            if (col.gameObject == null) return;

            if (col.gameObject.tag == "balloon" || col.gameObject.tag == "Special")
            {
                //trigger_objのlist内のオブジェクトではなかったらリストに追加
                if (!trigger_obj.Any(i => i == col.gameObject))
                {
                    trigger_obj.Add(col.gameObject);
                }
                //オブジェクトがあったら消去処理
                else
                {
                    if (trigger_obj.Count == 0) return;
                    foreach (var o in trigger_obj)
                    {
                        o.GetComponent<obj_float>()._Destroy();
                    }

                    //初期化
                    trigger_obj.Clear();
                    OnClick_flag = false;

                    common.all_ball--;

                    //特殊風船の単スコア追加
                    _scoreManager.Balloon_Plus(this.name);

                    Destroy(this.gameObject, 0.1f);
                }
            }
        }
    }
    */

}
