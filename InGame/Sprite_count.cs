using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Sprite_count : MonoBehaviour
{


    [SerializeField]
    Sprite[] Score_image;

    [SerializeField]
    Sprite[] Time_image;

  

    //表示用
    [SerializeField]
    Image spr_score;
    [SerializeField]
    Image spr_time;


    //親
    public Image Score_Parent;
    public Image Time_Parents;


    List<int> Sprite_num_Score = new List<int>();

    List<int> Sprite_num_Time = new List<int>();

    void Start()
    {
        //スコアの初期イメージ
        RectTransform s_image = (RectTransform)Instantiate(spr_score).transform;
        s_image.SetParent(this.transform, false);
        s_image.localPosition = new Vector2(
             //中央揃え
             s_image.localPosition.x,
            -453);
        s_image.GetComponent<Image>().sprite = Score_image[0];



    }
    /// <summary>
    /// スコア表示用関数
    /// </summary>
    /// <param name="value">スコア</param>
    public void _SprScore(int value)
    {
        if (value <= 0) return;

        var objs = GameObject.FindGameObjectsWithTag("Score_number");
        foreach (var obj in objs)
        {
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                Destroy(obj);
            }
        }

        var digit = value;
        Sprite_num_Score = new List<int>();
        while (digit != 0)
        {
            value = digit % 10;
            digit = digit / 10;
            Sprite_num_Score.Add(value);
        }


        for (int i = 0; i < Sprite_num_Score.Count; ++i)
        {
            //複製
            RectTransform s_image = (RectTransform)Instantiate(spr_score).transform;
            s_image.SetParent(Score_Parent.transform, false);
            s_image.localPosition = new Vector2(
                 //中央揃え
                 s_image.localPosition.x - s_image.sizeDelta.x * (i - (Sprite_num_Score.Count / 2)),
                s_image.localPosition.y + 15f);
            s_image.GetComponent<Image>().sprite = Score_image[Sprite_num_Score[i]];

        }

    }
    /// <summary>
    /// 時間表示用関数
    /// </summary>
    /// <param name="value">時間</param>
    public void _SprTime(int value)
    {
        if (value >= 999)
        {
            Debug.Log("999秒以上です");
            return;
        }

        var objs = GameObject.FindGameObjectsWithTag("Time_number");
        foreach (var obj in objs)
        {
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                Destroy(obj);
            }
        }

        var digit = value;
        Sprite_num_Time = new List<int>();
        while (digit != 0)
        {
            value = digit % 10;
            digit = digit / 10;
            Sprite_num_Time.Add(value);
        }
        var division = 0f;
        //センタリングのため桁数で位置を判断
        if (Sprite_num_Time.Count % 2 == 0) division = 3.6f;
        else if (Sprite_num_Time.Count == 1) division = 100f;
        else if(Sprite_num_Time.Count % 2 != 0)division = 2.85f;

        for (int i = 0; i < Sprite_num_Time.Count; ++i)
        {
            //複製
            RectTransform s_image = (RectTransform)Instantiate(spr_time).transform;
            s_image.SetParent(Time_Parents.transform, false);
            s_image.localPosition = new Vector2(
                 //中央揃え
                 s_image.localPosition.x - (s_image.sizeDelta.x - 13.5f) * (i - Sprite_num_Time.Count/division ),
                s_image.localPosition.y +10f);
            s_image.localScale = new Vector3(0.7f, 0.7f, 0f);
            s_image.GetComponent<Image>().sprite = Time_image[Sprite_num_Time[i]];

        }
    }

}


