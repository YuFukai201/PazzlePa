using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class rabbit_anim : MonoBehaviour {

    public Sprite[] rabbits_img;
    public Image rabbit;

    float time = 0;
    float Interval = 0.15f;
    int anim_num = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        time += 1 * Time.deltaTime;
        if (time >= Interval)
        {
            rabbit.sprite = rabbits_img[anim_num];
            anim_num++;
            time = 0;
            if (anim_num >= rabbits_img.Length)
            {
                anim_num = 0;
            }
        }
        

		
	}
}
