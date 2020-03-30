using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cs_test : MonoBehaviour
{
    GameObject TargetDummyUIObject;
    Vector3 UIPos;

    // Start is called before the first frame update
    void Start()
    {
        TargetDummyUIObject = transform.parent.transform.Find("UIDamagePos").gameObject;

        timer = 0f;
    }

    // Update is called once per frame
    float timer;
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer < 0f)
        {
            UIPos = TargetDummyUIObject.transform.position;

            GameObject.Find("HUD").GetComponent<cs_DamageUI>().ShowDamageInWorld(UIPos, 25);

            timer = 3.0f;
        }
    }
}
