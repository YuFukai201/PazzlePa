using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Score_Manager : MonoBehaviour {


    //風船の種類ごとのスコアと消した数
    struct Animals_Value
    {
        //dictionary
        Dictionary<string, float> animals_Dictionary;
        //ディクショナリーへ特殊スコアのデフォルト値を追加
        Dictionary<string, int> SpBalloon_Dictionary;

        //倍率
        List<float> Magnification;

        //それぞれの消した数に応じたスコア保存変数
        int Pig_Count;
        int Bear_Count;
        int Bard_Count;
        int Lion_Count;
        int Cow_Count;
        int Dog_Count;
        //int Special_Count1;
        //int Special_Count2;

        //それぞれ動物のスコア
        int Pig_Score;
        int Bear_Score;
        int Bard_Score;
        int Lion_Score;
        int Cow_Score;
        int Dog_Score;

        
        public float Total_Score;

        //コンストラクタ
        public Animals_Value(bool init) : this()
        {
            //初期化
            //風船個数
            Pig_Count = 0;
            Bear_Count = 0;
            Bard_Count = 0;
            Dog_Count = 0;
            Lion_Count = 0;
            Cow_Count = 0;



            //通常風船スコア
            Pig_Score = 95;
            Bear_Score = 95;
            Bard_Score = 95;
            Dog_Score = 95;
            Lion_Score = 95;
            Cow_Score = 95;

            //総合スコア
            Total_Score = 0;


            //ディクショナリーへ特殊スコアのデフォルト値を追加
            SpBalloon_Dictionary = new Dictionary<string, int>()
            {   
                //ハリセンボン
                {"Puffer_Fish_Score",1000},
                //ロケット
                {"Rocket_Score",1000},
                //重り？
                {"Anvil_Score",1200},
                //電池
                {"Battery_Score",2000},
                 //磁石
                {"Magnet_Score",2500},
                //爆弾
                {"Bomb_Score",1500},



            };


            //ディクショナリーへ追加 風船の種類分中身が必要
            animals_Dictionary = new Dictionary<string, float>()
            {
                //各風船カウント
                { "Bear_Count",Bear_Count },
                { "Pig_Count",Pig_Count },
                { "Bard_Count",Bard_Count },
                { "Lion_Count",Lion_Count },
                { "Cow_Count",Cow_Count },
                { "Dog_Count",Dog_Count },
                //スコア
                { "Bear_Score",Bear_Score },
                { "Pig_Score",Pig_Score },
                { "Bard_Score",Bard_Score },
                { "Lion_Score",Lion_Score },
                { "Cow_Score",Cow_Score },
                { "Dog_Score",Dog_Score }

            };
            //コンボ倍率
            Magnification = new List<float>()
            {
                //0
                1f,
                //1
                1f,
                //2
                1f,
                //3
                1f,
                //4
                1.1f,
                //5
                1.3f,
                //6
                1.5f,
                //7
                2.0f,
                //8
                2.5f
            };
        }

        /// <summary>
        /// スコア計算
        /// </summary>
        /// <param name="a_name">名前</param>
        /// <param name="mag">コンボ数</param>
        public void setAnimal_Value(string a_name, int mag)
        {

            if (mag >= Magnification.Count) mag = 8;
            //動物ごとのスコア
            animals_Dictionary[a_name + "_Count"] += (animals_Dictionary[a_name + "_Score"] * Magnification[mag]);
            //トータルスコア
            Total_Score += (animals_Dictionary[a_name + "_Score"] * Magnification[mag]);

            // Debug.Log((int)Total_Score);
        }

        //特殊風船計算
        public void SpBalloon_ScoreCalculation(string a_name,float mag)
        {
            animals_Dictionary[a_name + "_Count"] += (animals_Dictionary[a_name + "_Score"] * mag);
            //トータルスコア
            Total_Score += (animals_Dictionary[a_name + "_Score"] *mag);

        }
        
        //特殊風船単品のスコア計算
        public void SpBalloon_Plus(string sp_name)
        { 
            Total_Score += SpBalloon_Dictionary[sp_name + "_Score"];    
        }

        //カウントgetter  
        public float getAnimal_Count(string a_name)
        {
            float g_Value;
            g_Value = animals_Dictionary[a_name + "_Count"];
            return g_Value;
        }
        //動物ごとのスコアgetter
        public float getAnimal_Score(string a_name)
        {
            float g_Value;
            g_Value = animals_Dictionary[a_name + "_Score"];
            return g_Value;
        }

    };


    //スコアの構造体。引数の意味はない
    Animals_Value AnimalsValue = new Animals_Value(false);
   
    // private int _Totalscore = 0;

    /// <summary>
    /// 倍率orコンボ数とかスコアを触れているオブジェクトに表示する
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="destroy_obj"></param>
    public void effect_ValueText(Vector3 pos , bool destroy_obj, int combo_num)
    {
        if (destroy_obj == true)
        {
            //スコア描画
        }
        //コンボ描画or倍率描画

    }

    //通常風船計算
    public void set_Score(string name, int combo)
    {
        //風船の種類ごとカウントと倍率
        AnimalsValue.setAnimal_Value(name, combo);
        
       
    }
    
    //特殊風船計算
    public void set_SpBalloon_Score(string anim_name, float mag)
    {
        //Debug.Log(anim_name);
        //特殊風船のスコア
        AnimalsValue.SpBalloon_ScoreCalculation(anim_name, mag); 
       
    }
    //単品計算
    public void Balloon_Plus(string sp_name)
    {
       
        //単品計算
        AnimalsValue.SpBalloon_Plus(sp_name);
    }

    //トータルスコア
    public int get_Score()
    {
        return (int)AnimalsValue.Total_Score;
    }
    
    //動物ごとのスコア取得
    public int get_AnimalScore(string a_name)
    {
        return (int)AnimalsValue.getAnimal_Count(a_name);
    }  
}
