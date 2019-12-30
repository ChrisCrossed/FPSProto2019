using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerPackage : MonoBehaviour
{
    void Awake()
    {
        if (GameObject.Find("Main Camera")) Destroy(GameObject.Find("Main Camera"));

        gameObject.transform.Find("Player").gameObject.transform.parent = null;
        gameObject.transform.Find("HUD").gameObject.transform.SetParent(null);
        gameObject.transform.Find("WeaponCam").gameObject.transform.parent = null;

        Destroy(gameObject);
    }
}
