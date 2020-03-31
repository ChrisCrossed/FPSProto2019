using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class scr_GunFire : MonoBehaviour
{
    [SerializeField] AudioClip aud_GunFire_Rifle_ADS;
    [SerializeField] AudioClip aud_GunFire_Rifle_Normal;

    GameObject go_AudioGunfirePos;
    AudioSource aud_GunfirePos;

    GameObject CameraObj;
    GameObject RaycastPnt_Gun_Back;
    GameObject RaycastPnt_Gun_Front;

    [SerializeField] float WeaponDMGAssumption;

    // Start is called before the first frame update
    void Start()
    {
        go_AudioGunfirePos = gameObject.transform.Find("RayPnt_Gun_Front").gameObject;
        aud_GunfirePos = go_AudioGunfirePos.GetComponent<AudioSource>();

        CameraObj = gameObject.transform.parent.gameObject;
        RaycastPnt_Gun_Back = gameObject.transform.Find("RayPnt_Gun_Back").gameObject;
        RaycastPnt_Gun_Front = gameObject.transform.Find("RayPnt_Gun_Front").gameObject;
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

        // Raycast from Gun toward point aimed at
        RaycastToPoint();

        aud_GunfirePos.Play();
    }

    float ThisFunction(RaycastHit o)
    {
        return o.distance;
    }

    void RaycastToPoint()
    {
        int layerMask = LayerMask.GetMask("Default", "Enemy", "Bangable");
        RaycastHit[] hitArray;

        float damageReductionModifier = 0f;
        float wallBangReduction = 0.1f;
        float wallBangReductionIncrease = 0.05f;
        float wallBangReductionCap = 0.6f;

        hitArray = Physics.RaycastAll(CameraObj.transform.position, CameraObj.transform.forward, 250f, layerMask);

        // Rearranges array based on distance since hitArray is unsorted.
        hitArray = hitArray.OrderBy(o => o.distance).ToArray();
        // TODO: Learn about Linq & Lambda Functions
        // hitArray = hitArray.OrderBy(ThisFunction).ToArray();

        if (hitArray.Length > 0)
        {
            Debug.DrawLine(CameraObj.transform.position, hitArray[hitArray.Length - 1].point, Color.red, 0.5f);
        }

        for( int i = 0; i < hitArray.Length; ++i )
        {
            print(hitArray[i].collider.name);

            // Reduce damage based on wallbangable surfaces
            if ( hitArray[i].collider.gameObject.layer != LayerMask.NameToLayer("Bangable"))
            {
                if (hitArray[i].collider.gameObject.layer == LayerMask.NameToLayer("Default")) break;

                if (hitArray[i].collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    // Calculate current damage based on wall-bangable surfaces
                    int totalDamage = Mathf.FloorToInt(WeaponDMGAssumption * (1f - damageReductionModifier));

                    // Calculate current damage based on body part
                    if (hitArray[i].collider.name == "CenterMass") totalDamage = Mathf.FloorToInt(totalDamage * 0.43f);
                    if (hitArray[i].collider.name == "OuterMass") totalDamage = Mathf.FloorToInt(totalDamage * 0.35f);

                    hitArray[i].collider.gameObject.GetComponent<Cs_Enemy>().ApplyDamage( totalDamage );

                    break;
                }
            }
            else
            {
                // Increase damage reduction modifier (starts 0.1 -> 0.25 -> 0.4 -> 0.7 MAX (But overridden to 0.6))
                damageReductionModifier += wallBangReduction;
                
                // Increase wallBangReduction modifiers (starts 0.1 -> 0.15 -> 0.2 -> 0.25 MAX)
                wallBangReduction += wallBangReductionIncrease;
                
                // Cap
                if ( (1f - damageReductionModifier) < wallBangReductionCap)
                    damageReductionModifier = wallBangReductionCap;
            }
        }

        print("Total Damage Multiplier: " + (1f - damageReductionModifier));

        print("---");
    }
}
