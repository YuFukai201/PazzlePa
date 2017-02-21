using UnityEngine;
using System.Collections;

public class Magnet_Script : MonoBehaviour {

    [SerializeField]
    float Rotation_Speed = 2f;

    //誘爆のときにこいつを無理やりtrueにする？
    public bool OnClick_flag = false;

    Score_Manager _scoreManager;

    GameObject mainObj;

    bool GameOver;

    float time_count = 0;

    [SerializeField]
    float time_up = 3f;
    [SerializeField]
    GameObject obj_trigger;


    obj_float p_objFloat;

    bool _touch = false;


    // Use this for initialization
    void Start()
    {

        mainObj = GameObject.Find("balloon_main_obj");

        _scoreManager = mainObj.GetComponent<Score_Manager>();

        obj_trigger.GetComponent<CircleCollider2D>().enabled = false;

        p_objFloat = transform.parent.gameObject.GetComponent<obj_float>();

    }

    // Update is called once per frame
    void Update()
    {
        //ポーズなら停止
        if (common.mode_pose) return;

        gameObject.transform.Rotate(new Vector3(0, 0, Rotation_Speed + Time.deltaTime));


        /*//このオブジェクトが押されたら
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
                    obj_trigger.GetComponent<CircleCollider2D>().enabled = true;
                   
                    //回転、位置ともに固定化
                    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }
        }*/




        //押されたら
        if (p_objFloat.Touch_Flag)
        {
            if (!_touch)
            {
                //共通
                Rotation_Speed = 0;
                transform.parent = null;
                obj_trigger.GetComponent<CircleCollider2D>().enabled = true;

                //回転、位置ともに固定化
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                _touch = true;
            }
            else
            {

                time_count += 1 * Time.deltaTime;

                //自壊処理
                if (time_count >= time_up)
                {
                    //common.all_ball--;

                    _scoreManager.Balloon_Plus(gameObject.name);

                    Destroy(gameObject);
                }
            }
        }
        //シャボンの中
        else
        {
            //親に追従
            transform.position = transform.parent.gameObject.transform.position;
        }
       
       
      

    }

}
