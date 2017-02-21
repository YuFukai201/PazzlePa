using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bomb_Script : MonoBehaviour {

    //誘爆のときにこいつを無理やりtrueにする？
    bool OnClick_flag = false;

    Score_Manager _scoreManager;

    GameObject mainObj;

    bool GameOver;

    bool m_isClick;
    float m_count = 0;

    //trigger内のオブジェクト
    List<GameObject> trigger_obj = new List<GameObject>();

    CircleCollider2D[] bomb_col;

    obj_float p_objFloat;

    bool _touch = false;

    [SerializeField]
    Sprite[] bom_anim;

    // Use this for initialization
    void Start()
    {

        mainObj = GameObject.Find("balloon_main_obj");

        GetComponents<CircleCollider2D>()[1].enabled = false;

        _scoreManager = mainObj.GetComponent<Score_Manager>();

        // Balloon_pr = GetComponent<Balloon_Parents>();

        p_objFloat = transform.parent.gameObject.GetComponent<obj_float>();
       
    }

    // Update is called once per frame
    void Update()
    {

        //ポーズなら停止
        if (common.mode_pose) return;



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
                 if (hit_obj == gameObject)
                 {

                     m_isClick = true;

                     //リスト初期化
                     trigger_obj.Clear();

                     //アニメーション

                 }
             }
         }
         */


        //押されたら
        if (p_objFloat.Touch_Flag)
        {
            if (!_touch)
            {
                m_isClick = true;
                //リスト初期化
                trigger_obj.Clear();
                transform.parent = null;
                //アニメーション
                StartCoroutine(bomb_animation());
              
                _touch = true;
            }
            else
            {
                //3秒後に爆破
                m_count += 1 * Time.deltaTime;

                if (m_count >= 3f)
                {
                    //爆破
                    OnClick_flag = true;
                    GetComponents<CircleCollider2D>()[1].enabled = true;
                    //エフェクト
                    GameObject remove_effect = Instantiate(Resources.Load("anim/exp"), transform.position, Quaternion.identity) as GameObject;
                    Destroy(remove_effect, 1f);
                }
            }
        }
        //シャボンの中
        else
        {
            //親に追従
            transform.position = transform.parent.gameObject.transform.position;
        }

      /*  //クリックされたら
        if (m_isClick)
        {
            //3秒後に爆破
            m_count += 1 * Time.deltaTime;
            if (m_count >= 3f)
            {
                //爆破
                OnClick_flag = true;
                GetComponents<CircleCollider2D>()[1].enabled = true;
                //エフェクト
                GameObject remove_effect = Instantiate(Resources.Load("anim/exp"), transform.position, Quaternion.identity) as GameObject;
                Destroy(remove_effect, 1f);
            }

        }*/

    }

    IEnumerator bomb_animation()
    {
        var count = 0;
        while (true)
        {

            gameObject.GetComponent<SpriteRenderer>().sprite = bom_anim[count];
            count++;
            yield return new WaitForSeconds(0.1f);
            if (count >= 2) count = 0;
        }
    }




    private void OnTriggerStay2D(Collider2D col)
    {


        //クリックされたら
        if (p_objFloat.Touch_Flag)
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
                    foreach (var _obj in trigger_obj)
                    {
                        if (_obj != null)
                        {
                            
                            //引数に特殊風船の倍率を
                            _obj.GetComponent<obj_float>()._Destroy(3f);
                        }
                    }
                    //初期化
                    trigger_obj.Clear();
                    OnClick_flag = false;

                    //common.all_ball--;

                    //特殊風船の単スコア追加
                    _scoreManager.Balloon_Plus(this.name);

                   Destroy(this.gameObject, 0.1f);
                
                }
            }
        }
    }

  
}
