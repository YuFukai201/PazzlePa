using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameMain : MonoBehaviour
{

    //壁
    public GameObject Right_Wall;
    public GameObject Left_Wall;

    //最初に触れたオブジェクト
    private GameObject Fst_ball = null;
    //最後に触れたオブジェクト
    private GameObject Lst_ball = null;

    //色の判定用
    private string Ball_name;

    //ドラッグ中に触れたオブジェクトリスト
    List<GameObject> remove_ball_list = new List<GameObject>();
    //時止め時のリスト保存用リスト
    List<List<GameObject>> m_removeBallList = new List<List<GameObject>>();
    int m_listNum = 0;

    //メインのボールの数
    [SerializeField]
    int default_ball_count;

    //スコアscript
    Score_Manager _scoreManager;
    //時間
    [SerializeField]
    float Main_Time;

    //線の太さ
    float r = 0.5f;
    //角度変数
    float theta;
    //canvas
    public Canvas canvas;
    Sprite_count _sprCount;

    //ゲームオーバーかどうか
    public bool GameStop_Flag = false;

    public GameObject Ready_Sprite;
    //ゲームオーバsprite
    public GameObject GameOver_Sprite;

    //特殊風船の生成フラグ
    bool Create_SpecialBalloon = false;

    //選択された要素を入れる
    int[] RandChoice_Balloon = new int[4];

    [SerializeField]
    Canvas Pose_Canvas;

    //倍率表示用オブジェクト
    [SerializeField]
    GameObject mag_obj;

    //ライン
    [SerializeField]
    LineRenderer m_line;

    //特殊風船選択の初期値
    string[] m_spBalloonChar = new string[2] { "Puffer_Fish", "Rocket" };

    // Use this for initialization
    void Start()
    {

        //風船の種類をランダムに4種類選択する
        GameObject[] Choice_Obj = (GameObject[])Resources.LoadAll<GameObject>("balloon");
        for (int i = 0; i < 4;)
        {
            //読み込んだファイルの中のasset数からランダムに数字を入れる
            RandChoice_Balloon[i] = Random.Range(0, Choice_Obj.Length);
            //もし取り出した要素だったら処理をもう一度
            if (Choice_Obj[RandChoice_Balloon[i]] == null) continue;
            //取り出した数字をNULLへ
            Choice_Obj[RandChoice_Balloon[i]] = null;
            i++;
        }


        //特殊風船の取得
        for (int i = 0; i < 2;)
        {
            //プレイヤープレハブの中身がnullなら処理を通さない
            if (PlayerPrefs.GetString("SpecialBalloon_" + i) == "") break;
            //選択された特殊風船のcharを入れる
            m_spBalloonChar[i] = PlayerPrefs.GetString("SpecialBalloon_" + i);
            Debug.Log(m_spBalloonChar[i]);
            i++;
        }


        //最初の生成
        StartCoroutine(ball_create(default_ball_count));

        //commonと連携
        //common.all_ball = default_ball_count;


        //スプライト変更のスクリプト
        _sprCount = canvas.GetComponent<Sprite_count>();
        //スコアのgetとsetするために読み込み
        _scoreManager = GetComponent<Score_Manager>();

        //pose画像初期表示
        Pose_Canvas.GetComponent<Canvas>().enabled = false;

        //初期化
        m_line.numPositions = 0;
        m_line.sortingLayerName = "Line";
        m_line.enabled = true;

        //Ready?
        StartCoroutine(Ready_UI());

    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(GameObject.FindGameObjectsWithTag("balloon").Length);

        //ポーズかゲームオーバかready中なら以下を処理させない
        if (common.mode_pose || GameStop_Flag) return;



        //制限時間を過ぎたら操作を受け付けないようにする
        if (Main_Time <= 0 && !GameStop_Flag)
        {
            GameStop_Flag = true;

            StartCoroutine(GameOvar_Space_Time());

            return;
        }

        //時間の経過  もし時止めだったらカウント中止
        if (!common.mode_Tstop) Main_Time -= 1 * Time.deltaTime;
        //時止め中じゃなく、m_removeBallListの中身があったなら
        else
        {
            for (int i = 0; i < m_removeBallList.Count; i++)
            {
                StartCoroutine(Remove_Dlay(m_removeBallList[i]));
            }
            m_removeBallList.Clear();
        }

        //時間とスコアのsprite更新
        _sprCount._SprTime((int)Main_Time);
        _sprCount._SprScore(_scoreManager.get_Score());



        //ドラッグ開始　クリック
        if (Input.GetMouseButtonDown(0) && Fst_ball == null)
        {
            //マウスの位置取得
            RaycastHit2D mouse_hit = Physics2D.CircleCast(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f, Vector2.zero);


            //バグ回避
            //消える予定のオブジェクトがないかチェック
            bool obj_in_list = false;
            foreach (var obj in remove_ball_list)
            {
                //もし消える予定のオブジェクトだったら
                if (obj == mouse_hit.collider.gameObject)
                {
                    obj_in_list = true;
                    Debug.Log("already remove object");
                }
            }


            //クリックしていたら,removelistの中にオブジェクトがなかったら
            if (mouse_hit.collider != null && !obj_in_list)
            {
                //最初に触ったオブジェクトの取得
                GameObject hit_obj = mouse_hit.collider.gameObject;
                //タグ取得
                Ball_name = hit_obj.name;
                if (hit_obj.tag == "balloon")
                {
                    //SE
                    GameObject.Find("FarstTouch_Sound").GetComponent<AudioSource>().Play();

                    //最初に触れたオブジェクト
                    Fst_ball = hit_obj;
                    Lst_ball = hit_obj;
                    //リストのリセット
                    remove_ball_list = new List<GameObject>();
                    //リストに追加
                    remove_ball_list.Add(hit_obj);
                    //死に顔
                    hit_obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("img/balloon/" + hit_obj.name + "_Exp");
                    //ポイント設置
                    hit_obj.GetComponent<obj_float>().LinePoint_Set();
                    //演出
                    StartCoroutine(Balloon_Scale(hit_obj));

                }
            }
        }

        //ドラッグ終了
        else if (Input.GetMouseButtonUp(0))
        {
            //リストの総配列
            int remove_count = remove_ball_list.Count;
            //8つ以上なら特殊パズル一個生成

            //三つ以上なら消す
            if (remove_count >= 3)
            {
                //風船生成条件
                if (remove_count >= 7)
                {
                    //特殊風船生成フラグ
                    Create_SpecialBalloon = true;
                }

                if (common.mode_Tstop)
                {

                    //リストに追加、ほかの処理でforeachで回すべし
                    m_removeBallList.Add(remove_ball_list);
                }

                else
                {
                    //削除処理開始
                    StartCoroutine(Remove_Dlay(remove_ball_list));
                }

            }

            else if (remove_count <= 2)
            {
                foreach (GameObject obj in remove_ball_list)
                {
                    objScaleInit(obj);
                    //死に顔から戻す
                    obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("img/balloon/" + obj.name);
                    //ポイント削除
                    obj.GetComponent<obj_float>().LinePoint_Delete();

                }
                //リストクリア
                remove_ball_list.Clear();
            }

            //初期化
            Fst_ball = null;
            Lst_ball = null;
            Ball_name = null;
            m_line.numPositions = 0;


        }

        //クリック中
        else if (Fst_ball != null)
        {
            //マウスの位置取得
            RaycastHit2D mouse_hit = Physics2D.CircleCast(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.25f, Vector2.zero);

            m_line.numPositions = remove_ball_list.Count;
            for (int i = 0; i < remove_ball_list.Count; i++)
            {
                m_line.SetPosition(i, remove_ball_list[i].transform.position);

            }

            if (mouse_hit.collider != null)
            {

                //ドラッグ中に当たったオブジェクト
                GameObject hit_obj = mouse_hit.collider.gameObject;
                
                //最初に触れたオブジェクトと同色かどうか、すでに触れたオブジェクトではないかどうか
                if (hit_obj.name == Ball_name && Lst_ball != hit_obj)
                {

                    //次触れられるオブジェクト距離の判定用
                    float dist = Vector2.Distance(hit_obj.transform.position, Lst_ball.transform.position);
                    if (dist < 2.3f)
                    {

                        //要素が一つの場合
                        if (remove_ball_list.Count == 1)
                        {
                            //同じ要素を選択していなければ追加
                            if (remove_ball_list[0] != hit_obj)
                            {
                                //リストに追加
                                remove_ball_list.Add(hit_obj);
                                Lst_ball = hit_obj;
                                //spriteを死に顔に変更
                                hit_obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("img/balloon/" + hit_obj.name + "_Exp");
                                //ポイント設置
                                hit_obj.GetComponent<obj_float>().LinePoint_Set();
                                //演出
                                StartCoroutine(Balloon_Scale(hit_obj));
                            }
                            return;
                        }
                        //最後のひとつ前の要素が新規要素と同じなら最後の要素を削除
                        if (remove_ball_list[remove_ball_list.Count - 2] == hit_obj)
                        {

                            //死に顔から戻す
                            remove_ball_list[remove_ball_list.Count - 1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("img/balloon/" + hit_obj.name);
                            //ポイント削除
                            remove_ball_list[remove_ball_list.Count - 1].GetComponent<obj_float>().LinePoint_Delete();
                            //スケールの初期化
                            objScaleInit(remove_ball_list[remove_ball_list.Count - 1]);
                            remove_ball_list.Remove(remove_ball_list[remove_ball_list.Count - 1]);

                            Lst_ball = hit_obj;
                        }
                        else
                        {
                            //要素の中に新規要素と同じものがあるならば追加しない
                            foreach (var i in Enumerable.Range(0, remove_ball_list.Count))
                            {
                                if (remove_ball_list[i] == hit_obj)
                                {
                                    return;
                                }
                            }
                            //リストに追加
                            remove_ball_list.Add(hit_obj);
                            //演出
                            StartCoroutine(Balloon_Scale(hit_obj));
                            Lst_ball = hit_obj;
                            //spriteを死に顔に変更
                            hit_obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("img/balloon/" + hit_obj.name + "_Exp");
                            //ポイント設置
                            hit_obj.GetComponent<obj_float>().LinePoint_Set();

                        }
                    }

                }

            }
        }

        //全体の風船が1個でも減っているなら1個追加
        if (GameObject.FindGameObjectsWithTag("balloon").Length < default_ball_count)
        {
            StartCoroutine(ball_create(1));
        }



    }
    //UPdate

    //  ボール作成
    IEnumerator ball_create(int ball_count)
    {
        if (GameObject.FindGameObjectsWithTag("balloon").Length >= default_ball_count) yield break;

        for (int i = 0; i < ball_count; i++)
        {
            //生成オブジェクト
            GameObject Ball_obj;
            //乱数生成
            int ram_Instan_num = Random.Range(0, 4 /*RandChoice_Balloon.Length*/);

            Vector2 Ball_pos = new Vector2(Random.Range(Left_Wall.transform.position.x + 3.0f, Right_Wall.transform.position.x - 3.0f), -11f);


            //特殊風船生成判定
            int Special_Instan_ram = Random.Range(0, 250);
            if (Special_Instan_ram < 1 || Create_SpecialBalloon)
            {

                //2つのうちどちらかを生成
                int Special_Create_num = Random.Range(0, 2);
                switch (Special_Create_num)
                {
                    //乱数で選択された名前を受け取って生成
                    case 0:
                        //爆弾を一つ以上生成しない
                        if (GameObject.Find("Bomb") != null && GameObject.Find("Bomb").name == m_spBalloonChar[0])break;
                        Ball_obj = Instantiate(Resources.Load("Test_balloon/" + m_spBalloonChar[0] + "_shabom"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = m_spBalloonChar[0];
                        //Debug.Log("Special!1");
                        break;

                    case 1:
                        if (GameObject.Find("Bomb") != null && GameObject.Find("Bomb").name == m_spBalloonChar[1]) break;
                        Ball_obj = Instantiate(Resources.Load("Test_balloon/" + m_spBalloonChar[1] + "_shabom"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = m_spBalloonChar[1];
                        //Debug.Log("Special!2");
                        break;

                    default:
                        Debug.LogError("not create Special object");
                        break;
                }

            }
            //ノーマル風船生成処理
            else
            {
                //生成　名前はclone抜きにしないと後々面倒なので後付け設定
                switch (ram_Instan_num/*RandChoice_Balloon[ram_Instan_num]*/)
                {
                    case 0:
                        Ball_obj = Instantiate(Resources.Load("balloon/Bear"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = "Bear";
                        break;

                    case 1:
                        Ball_obj = Instantiate(Resources.Load("balloon/Bard"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = "Bard";
                        break;

                    case 2:
                        Ball_obj = Instantiate(Resources.Load("balloon/Pig"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = "Pig";
                        break;
                    case 3:
                        Ball_obj = Instantiate(Resources.Load("balloon/Cow"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = "Cow";
                        break;
                    case 4:
                        Ball_obj = Instantiate(Resources.Load("balloon/Lion"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = "Lion";
                        break;
                    case 5:
                        Ball_obj = Instantiate(Resources.Load("balloon/Dog"), Ball_pos, Quaternion.identity) as GameObject;
                        Ball_obj.name = "Dog";
                        break;

                    default:
                        Debug.LogError("not create Nomal object");
                        break;
                }
            }

            //特殊風船フラグ
            Create_SpecialBalloon = false;
            //画面上の風船数
            //common.all_ball++;

        }
        yield return new WaitForSeconds(0.01f);

    }
    //風船が消える時ディレイを入れる処理
    IEnumerator Remove_Dlay(List<GameObject> _list)
    {

        //倍率表示処理
        GetComponent<Magnification_Sprite>().magnification_disp(_list[_list.Count - 1].transform.position, _list.Count);
        var wait = new WaitForSeconds(0.05f);

        //オブジェクト消去処理
        for (int i = 0; i < _list.Count; i++)
        {
            //ポイント削除
            _list[i].GetComponent<obj_float>().LinePoint_Delete();
            //SE
            _list[i].GetComponent<AudioSource>().Play();

            yield return wait;

            //エフェクト
            GameObject remove_effect = Instantiate(Resources.Load("anim/exp_Balloon"), _list[i].transform.position, Quaternion.identity) as GameObject;

            //風船の種類ごとカウントと倍率
            _scoreManager.set_Score(_list[i].name, _list.Count);

            //消去
            Destroy(_list[i]);
            Destroy(remove_effect, 1f);
            //common.all_ball--;
        }

        //初期化
        _list.Clear();
    }



    //風船の演出
    IEnumerator Balloon_Scale(GameObject _obj)
    {
        var wait = new WaitForSecondsRealtime(0.05f);
        //スケールの初期化
        //無限拡大させないように
        objScaleInit(_obj);

        var _Scale = _obj.transform.localScale;
        _obj.transform.localScale = _Scale * 1.3f;
        yield return wait;
        _obj.transform.localScale = _Scale * 1.1f;
        yield return wait;
        _obj.transform.localScale = _Scale * 1.2f;
        yield return wait;
        _obj.transform.localScale = _Scale * 1.1f;

        if (remove_ball_list.Count == 0)
            objScaleInit(_obj);

    }

    //ゲームオーバーの演出用
    IEnumerator GameOvar_Space_Time()
    {

        yield return new WaitForSecondsRealtime(0.1f);
        m_line.numPositions = 0;
        GameObject.Find("GameOver_Sound").GetComponent<AudioSource>().Play();
        //スコアをプレイヤープレハブへ登録処理 現在ゲーム上にある風船の名前をgetできたらもう少し効率的に書けるかも？
        // リザルト用加筆
        PlayerPrefs.SetInt("Score", (int)_scoreManager.get_Score());
        PlayerPrefs.SetInt("Pig_Total", _scoreManager.get_AnimalScore("Pig"));
        PlayerPrefs.SetInt("Bear_Total", _scoreManager.get_AnimalScore("Bear"));
        PlayerPrefs.SetInt("Bard_Total", _scoreManager.get_AnimalScore("Bard"));
        PlayerPrefs.SetInt("Cow_Total", _scoreManager.get_AnimalScore("Cow"));
        PlayerPrefs.SetInt("Lion_Total", _scoreManager.get_AnimalScore("Lion"));
        PlayerPrefs.SetInt("Dog_Total", _scoreManager.get_AnimalScore("Dog"));
        // 加筆箇所終わり

        //ゲームオーバー表示
        GameOver_Sprite.SetActive(true);

        yield return new WaitForSeconds(2f);
        //初期化処理
        _init();
        SceneManager.LoadScene("ResultScene");
    }

    //始まる前のレディーとゴー
    IEnumerator Ready_UI()
    {
        Ready_Sprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/UI/Old/ready");
        GameStop_Flag = true;
        Ready_Sprite.SetActive(true);
        yield return new WaitForSeconds(2f);
        Ready_Sprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/UI/old/go");
        Ready_Sprite.GetComponent<Image>().SetNativeSize();
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            Ready_Sprite.transform.localScale += Vector3.one * 0.05f;
            if (Ready_Sprite.transform.localScale.y >= 1.3f)
            {
                GameObject.Find("Ready_Sound").GetComponent<AudioSource>().Play();
                break;
            }
        }



        yield return new WaitForSeconds(0.4f);

        GameStop_Flag = false;
        Ready_Sprite.SetActive(false);
    }


    //バグ回避
    //ドラッグ中にdeleteObjに触れてオブジェクトが消えNULLになるので回避用関数
    public void Ball_Remove(GameObject _obj)
    {
        if (remove_ball_list.Count != 0)
        {
            foreach (var l in remove_ball_list)
            {
                if (l == _obj)
                {
                    remove_ball_list.Remove(l);
                }
            }
        }
    }

    public void _TimePlus(int t)
    {
        Main_Time += t;
    }
    /// <summary>
    /// 初期化関数
    /// </summary>
    void _init()
    {
        //初期化
        // common.all_ball = 0;
        Time.timeScale = 1.0f;
    }


    //ポーズ開始
    public void Pose_Button()
    {
        if (!common.mode_pose)
        {
            //SE
            GameObject.Find("Pose_sound").GetComponent<AudioSource>().Play();

            GameObject.Find("Main Camera").GetComponent<UnityStandardAssets.ImageEffects.Blur>().enabled = true;
            common.mode_pose = true;
            Time.timeScale = 0f;

            //ボタン表示
            Pose_Canvas.GetComponent<Canvas>().enabled = true;




        }
        else Debug.LogError("error!");

    }
    //ポーズ終了
    public void Continue_Button()
    {
        if (common.mode_pose)
        {
            GameObject.Find("Main Camera").GetComponent<UnityStandardAssets.ImageEffects.Blur>().enabled = false;
            common.mode_pose = false;
            Time.timeScale = 1.0f;
            Pose_Canvas.GetComponent<Canvas>().enabled = false;
        }
        else Debug.LogError("error!");
    }
    //ホームへ戻る
    public void MoveHome_Button()
    {
        if (common.mode_pose)
        {
            _init();
            common.mode_pose = false;
            SceneManager.LoadScene("MenuScene");

        }
        else Debug.LogError("error!");
    }

    //拡大率の初期化
    void objScaleInit(GameObject _obj)
    {
        _obj.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
    }

}
