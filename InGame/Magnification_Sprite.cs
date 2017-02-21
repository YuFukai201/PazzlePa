using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnification_Sprite : MonoBehaviour {

    [SerializeField]
    Sprite[] mag_Sprite;
    [SerializeField]
    GameObject mag_obj;
    [SerializeField]
    GameObject Parent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void magnification_disp(Vector3 pos, int combo)
    {
        combo -= 3;
        if (combo >= 6) combo = 5;
        //Debug.Log(combo);

        var _parent = Instantiate(Parent, pos, Quaternion.identity) as GameObject;
        var _obj = Instantiate(mag_obj) as GameObject;
        _obj.GetComponent<SpriteRenderer>().sprite = mag_Sprite[combo];
        _obj.transform.parent = _parent.transform;
        _obj.transform.position = Vector3.zero;
        
        Destroy(_obj, 2.0f);
        
    }

}
