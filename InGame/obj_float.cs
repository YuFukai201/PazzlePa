using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class obj_float : MonoBehaviour {

    [SerializeField]
    float force_pow = 0;

    float time_count = 0;

    GameMain _balloonMainSc;
    Score_Manager m_score;

    bool _pose = false;

    public bool Touch_Flag = false;

    [SerializeField]
    GameObject m_PointOrg;

    GameObject m_LinePoint;

    // Use this for initialization
    void Start () {

        _balloonMainSc = GameObject.Find("balloon_main_obj").GetComponent<GameMain>();
        m_score = GameObject.Find("balloon_main_obj").GetComponent<Score_Manager>();
        this.gameObject.GetComponent<Rigidbody2D>().AddForce( new Vector2(0,2f), ForceMode2D.Impulse);
       
        
    }
	
	// Update is called once per frame
	void Update ()
    {

        //一時停止
        if (common.mode_pose || common.mode_Tstop)
        {
            if (!_pose)
            {   //オブジェクトの運動と回転を保存
                gameObject.GetComponent<Rigidbody2D>().Pause(gameObject);
                //オブジェクト停止
                gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                _pose = true;
            }
           // return;
        }
        else if(!common.mode_pose && !common.mode_Tstop)
        {
            if (_pose)
            {
                gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                //保存している値を反映
                gameObject.GetComponent<Rigidbody2D>().Resume(gameObject);
                _pose = false;
            }
            //かける浮力的な
            time_count += Time.deltaTime;
            if (time_count >= 0.15f)
            {
                this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, force_pow), ForceMode2D.Force);
                time_count = 0;
            }
        }

         
       
        //時間停止時にここが通らない
        if (gameObject.tag == "Shabom")
        {
          
            //このオブジェクトが押されたら
            if (Input.GetMouseButtonDown(0))
            {
                var GameOver = _balloonMainSc.GameStop_Flag;
              
                //マウスの位置取得
                RaycastHit2D mouse_hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //ゲームオーバでなければ
                if (mouse_hit.collider != null && !GameOver)
                {
                   
                    //最初に触ったオブジェクトの取得
                    GameObject hit_obj = mouse_hit.collider.gameObject;
                    if (hit_obj == gameObject && !Touch_Flag)
                    {
                        Touch_Flag = true;
                        Destroy(gameObject, 0.1f);

                    }
                }
            }
        }

        

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (gameObject.tag == "Special") return;
        //天井に当たったら削除の処理
        if (col.gameObject.name == ("deleteObj"))
        {
            if (gameObject.tag != "Special")
                _balloonMainSc.Ball_Remove(gameObject);

            //common.all_ball--;
            Destroy(gameObject,0.1f);
            
        }
    }

    //自壊処理
    public void _Destroy(float mag)
    {
        if (gameObject.tag == "balloon")
        {

            //SE
            gameObject.GetComponent<AudioSource>().Play();
            //特殊計算処理
            m_score.set_SpBalloon_Score(gameObject.name, mag);

            //爆発エフェクト
            GameObject _Exprosion = GameObject.Instantiate(Resources.Load("anim/exp_Balloon"), transform.position, Quaternion.identity) as GameObject;
            Destroy(_Exprosion, 1f);

            _balloonMainSc.Ball_Remove(gameObject);

            //common.all_ball--;

           

            Destroy(gameObject);

        }
        //誘爆
        else if (gameObject.tag == "Special")
        {

        }
       
    }


    public void LinePoint_Set()
    {
        if (gameObject.tag == "balloon")
        {
            //生成
            m_LinePoint = Instantiate(m_PointOrg, gameObject.transform.position, Quaternion.identity) as GameObject;
            m_LinePoint.transform.parent = gameObject.transform;
               
           
        }
    }

    public void LinePoint_Delete()
    {
        if (gameObject.tag == "balloon")
        {
            Destroy(m_LinePoint);
            m_LinePoint = null;
        }
    }

}
