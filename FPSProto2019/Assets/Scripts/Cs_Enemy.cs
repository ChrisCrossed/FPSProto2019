using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Enemy : MonoBehaviour
{
    [SerializeField] GameObject TargetDummyUIObject;
    Vector3 UIPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ApplyDamage( int damage_ )
    {
        UIPos = TargetDummyUIObject.transform.position;

        GameObject.Find("HUD").GetComponent<cs_DamageUI>().ShowDamageInWorld( UIPos, damage_ );
    }
}
