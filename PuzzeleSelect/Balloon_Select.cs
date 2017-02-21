using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Balloon_Select : MonoBehaviour
{

    [SerializeField]
    GameObject[] Select_Image;

    List<GameObject> m_selectedObj = new List<GameObject>();

    GameObject nowSelect_obj = null;
    [SerializeField]
    Text Balloon_name;
    [SerializeField]
    Text Explanatory_text;

    // Use this for initialization
    void Start()
    {

        foreach (var _obj in Select_Image)
        {
            _obj.GetComponent<Image>().sprite = null;
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //マウスの位置取得
            RaycastHit2D mouse_hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            //何かをタッチしていたら
            if (mouse_hit.collider != null)
            {
                               
                GameObject hit_obj = mouse_hit.collider.gameObject;

                if (hit_obj.GetComponent<SpriteRenderer>() != null)
                {

                    //説明文表示
                    if (nowSelect_obj != hit_obj)
                    {
                        GameObject.Find("SOUNDetc/SE_setumei").GetComponent<AudioSource>().Play();

                        //風船の説明文
                        switch (hit_obj.name)
                        {
                            case "Puffer_Fish":
                                //名前
                                Balloon_name.text = "ふぐ";
                                //説明文
                                Explanatory_text.text = "小範囲の爆発効果";
                                break;

                            case "Bomb":
                                //名前
                                Balloon_name.text = "爆弾";
                                //説明文
                                Explanatory_text.text = "タッチして3秒後に\n中範囲の爆発効果";
                                break;

                            case "Rocket":
                                //名前
                                Balloon_name.text = "ロケット";
                                //説明文
                                Explanatory_text.text = "向いている方向へ発射\n触れた風船を消す";
                                break;

                            case "Battery":
                                //名前
                                Balloon_name.text = "電池";
                                //説明文
                                Explanatory_text.text = "ゲーム内制限時間を\n3秒追加";
                                break;

                            case "Magnet":
                                //名前
                                Balloon_name.text = "磁石";
                                //説明文
                                Explanatory_text.text = "周囲の風船を3秒集める";
                                break;

                            case "Anvil":
                                //名前
                                Balloon_name.text = "ダンベル";
                                //説明文
                                Explanatory_text.text = "タッチしたら落下する\n触れた風船を消す";
                                break;


                            default:
                                break;
                        }
                        Debug.Log("説明文表示");
                        nowSelect_obj = hit_obj;

                    }
                    //二回目のタッチならオブジェクトをイメージに入れる
                    else if (hit_obj == nowSelect_obj)
                    {
                        //タッチしたオブジェクトのspriteをcanvasのイメージに入れる
                        foreach (var _obj in Select_Image)
                        {
                            if (_obj.GetComponent<Image>().sprite == null)
                            {
                                GameObject.Find("SOUNDetc/SE_OK").GetComponent<AudioSource>().Play();

                                _obj.GetComponent<Image>().sprite = hit_obj.GetComponent<SpriteRenderer>().sprite;
                                _obj.name = hit_obj.name;

                                //選択されたオブジェクトをもう一度選択できないようにに
                                //hit_obj.GetComponent<SpriteRenderer>().enabled = false;
                                hit_obj.SetActive(false);
                                m_selectedObj.Add(hit_obj);

                                break;
                            }
                        }

                        nowSelect_obj = null;
                    }
                    
                }
                //キャンセル処理
                else if (hit_obj.GetComponent<Image>().sprite != null)
                {
                    foreach (var _obj in Select_Image)
                    {
                        if (_obj == hit_obj)
                        {
                            foreach (var l_obj in m_selectedObj)
                            {
                                if (l_obj.GetComponent<SpriteRenderer>().sprite == hit_obj.GetComponent<Image>().sprite)
                                {
                                   // l_obj.GetComponent<SpriteRenderer>().enabled = true;
                                    l_obj.SetActive(true);

                                    GameObject.Find("SOUNDetc/SE_change").GetComponent<AudioSource>().Play();
                                }
                            }
                            hit_obj.GetComponent<Image>().sprite = null;

                        }
                    }
                }
            }

        }
    }



    public void Move_Scene_Back()
    {
        var num = 0;
        foreach (var _obj in Select_Image)
        {
            //イメージすべてが選ばれたら
            if (_obj.GetComponent<Image>().sprite != null)
            {
                //選択された特殊風船の名前を保存
                PlayerPrefs.SetString("SpecialBalloon_" + num, _obj.name);
                num++;
            }
        }

        if (num >= 2)
        {
            GameObject.Find("SOUNDetc/SE_home").GetComponent<AudioSource>().Play();
            SceneManager.LoadScene("MenuScene");
        }
    }

    public void Move_Scene_Option()
    {
        var num = 0;
        foreach (var _obj in Select_Image)
        {
            //イメージすべてが選ばれたら
            if (_obj.GetComponent<Image>().sprite != null)
            {
                //選択された特殊風船の名前を保存
                PlayerPrefs.SetString("SpecialBalloon_" + num, _obj.name);
                num++;
            }
        }

        if (num >= 2)
        {
            GameObject.Find("SOUNDetc/SE_home").GetComponent<AudioSource>().Play();
            SceneManager.LoadScene("OptionScene");
        }
    }

}