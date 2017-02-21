using UnityEngine;
using System.Collections;

public class Battery_Script : MonoBehaviour {


    [SerializeField]
    float Rotation_Speed = 2f;

    //誘爆のときにこいつを無理やりtrueにする？
    public bool OnClick_flag = false;

    Score_Manager _scoreManager;

    GameObject mainObj;

    bool GameOver;

    const int p_time = 3;

    obj_float p_objFloat;

    bool _touch = false;

    [SerializeField]
    GameObject Plus_Effect;


    // Use this for initialization
    void Start()
    {

        mainObj = GameObject.Find("balloon_main_obj");

        _scoreManager = mainObj.GetComponent<Score_Manager>();

        p_objFloat = transform.parent.gameObject.GetComponent<obj_float>();
        Plus_Effect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズなら停止
        if (common.mode_pose) return;

        gameObject.transform.Rotate(new Vector3(0, 0, Rotation_Speed + Time.deltaTime));

        /*
          //このオブジェクトが押されたら
          if (Input.GetMouseButtonDown(0))
          {
              GameOver = mainObj.GetComponent<tmtm_main>().GameStop_Flag;

              //マウスの位置取得
              RaycastHit2D mouse_hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
              //ゲームオーバでなければ
              if (mouse_hit.collider != null && !GameOver)
              {
                  //最初に触ったオブジェクトの取得
                  GameObject hit_obj = mouse_hit.collider.gameObject;
                  if (hit_obj == gameObject && !OnClick_flag)
                  {
                      OnClick_flag = true;

                      Time_plus();

                  }
              }
          }
          */
        //押されたら
        if (p_objFloat.Touch_Flag)
        {
            if (_touch) return;

            Rotation_Speed = 0;
           //エフェクト
            Plus_Effect.transform.parent = null;
            Plus_Effect.SetActive(true);
            Destroy(Plus_Effect, 1.5f);

            transform.parent = null;
            Time_plus();
            _touch = true;
        }
        //シャボンの中
        else
        {
            //親に追従
            transform.position = transform.parent.gameObject.transform.position;
        }

    }



    void Time_plus()
    {
       

        //時間＋3のエフェクト生成

        //時間追加
        mainObj.GetComponent<GameMain>()._TimePlus(p_time);

        //common.all_ball--;

        //特殊風船の単スコア追加
        _scoreManager.Balloon_Plus(this.name);

        Destroy(gameObject, 0.05f);

    }



}
