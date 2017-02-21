using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Time_Stop_Script : MonoBehaviour {


    //時間停止ボタン
    [SerializeField]
    Button Tstop_Button;
    
    //image
    [SerializeField]
    Image Tstop_Image;
   
    //停止時間
    [SerializeField]
    float Tstop_Value = 3f;

    float Tstop_Gauge = 0;

    [SerializeField]
    SpriteRenderer BackGround_img;
    [SerializeField]
    GameObject clock_effect;
    bool _flag = false;

    //時間停止を開始した時間
    float last_stopTime;


    // Use this for initialization
    void Start () {

        Tstop_Image.fillAmount = 0f;

	}
	
	// Update is called once per frame
	void Update () {

        
       
        //時間停止処理
        if (common.mode_Tstop)
        {
            //時止め中にposeした時
            if (common.mode_pose)
            {
                //last_stopTime = 
            }
            //ボタン無効
            Tstop_Button.interactable = false;
            //停止
            Time.timeScale = 0;


            //時止め終了
            if (last_stopTime + Tstop_Value  < Time.realtimeSinceStartup )
            {
                _flag = false;
                if (!common.mode_pose)
                {
                    //再開
                    Time.timeScale = 1;
                }
                //Tstop_Value = 3f;
                //ゲージ初期化
                Tstop_Gauge = 0f;
                //ボタンの無効化
                Tstop_Button.interactable = false;
                //背景色変更
                BackGround_img.color = new Color(1f, 1f, 1f);

                common.mode_Tstop = false;
            }
        }
        //時間停止中はメインタイムを減算しない
        else
        { 
            //時止めゲージの加算
            Tstop_Gauge += 3.6f * Time.deltaTime;
            Tstop_Image.fillAmount = Tstop_Gauge / 100f;
            //ゲージMAX
            if (Tstop_Gauge >= 100 && !_flag)
            {
                _flag = true;
                var ce =Instantiate(clock_effect, new Vector3(-28f, 0.25f, 0f), Quaternion.identity) as GameObject;
                Destroy(ce, 1.3f);
                //ボタンの有効化
                Tstop_Button.interactable = true;
            }

        }


      

        //ポーズ中だったら無効
        if (common.mode_pose)Tstop_Button.interactable = false;

    }


    void Charge_Guage()
    {
        //ゲージを貯める処理
    }



    //時間停止ボタン
    public void Time_Stop()
    {
        var _flag = GetComponent<GameMain>().GameStop_Flag;
        //ゲームオーバ時にこの処理は通さない
        if (_flag) return;

        common.mode_Tstop = true;
        //背景色変更
        BackGround_img.color = new Color(0.45f, 0.45f, 0.45f);
        //止めた時間を代入
        last_stopTime = Time.realtimeSinceStartup;
        Debug.Log("Stop!");

    }
}
