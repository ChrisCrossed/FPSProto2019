using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_GunFire : MonoBehaviour
{
    [SerializeField] AudioClip aud_GunFire_Rifle_ADS;
    [SerializeField] AudioClip aud_GunFire_Rifle_Normal;

    GameObject go_AudioGunfirePos;
    AudioSource aud_GunfirePos;

    // Start is called before the first frame update
    void Start()
    {
        go_AudioGunfirePos = gameObject.transform.Find("RayPnt_Gun_Front").gameObject;
        aud_GunfirePos = go_AudioGunfirePos.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    { 
        
    }

    public void FireGun( WeaponState weaponState_ )
    {
        switch (weaponState_)
        {
            case WeaponState.Normal:
                aud_GunfirePos.clip = aud_GunFire_Rifle_Normal;
                break;
            case WeaponState.ADS:
                aud_GunfirePos.clip = aud_GunFire_Rifle_ADS;
                break;
            default:
                break;
        }

        aud_GunfirePos.Play();
    }
}
