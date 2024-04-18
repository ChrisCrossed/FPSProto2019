using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum SmokeState
{
    Normal,
    Selected,
    Disabled
}

public class SmokeLogic : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject SmokeOutside;
    GameObject SmokeInside;

    [SerializeField]
    Material BlackSmoke;
    [SerializeField]
    Material BlueSmoke;
    [SerializeField]
    Material InsideSmoke;
    
    void Start()
    {
        SmokeOutside = gameObject.transform.Find("Smoke_Outside").gameObject;
        SmokeInside = gameObject.transform.Find("Smoke_Inside").gameObject;
    }

    float SmokeTimer;
    float SelectionTimer;

    private void Update()
    {
        if(SmokeTimer > 0f)
        {
            SmokeTimer -= Time.deltaTime;

            if(SmokeTimer < 0f)
            {
                SmokeTimer = 0f;
                SetSmokeState(SmokeState.Disabled);
            }
        }

        if(SelectionTimer > 0f)
        {
            SelectionTimer -= Time.deltaTime;
            if(SelectionTimer < 0f)
            {
                SelectionTimer = 0f;

                SetSmokeState(SmokeState.Normal);
            }
        }
    }

    public void SetSmokeState(SmokeState smokeState)
    {
        switch(smokeState)
        {
            case SmokeState.Selected:
                SmokeState_Selected();
                break;

            case SmokeState.Disabled:
                SmokeState_Disabled();
                break;

            case SmokeState.Normal:
            default:
                SmokeState_Normal();
                break;
        }
    }

    void SmokeState_Selected()
    {
        SmokeOutside.GetComponent<MeshRenderer>().material = BlueSmoke;
        SmokeOutside.transform.localScale = new Vector3(5.3f, 5.3f, 5.3f);

        SelectionTimer = 0.01f;
    }

    void SmokeState_Normal()
    {
        SmokeOutside.GetComponent<MeshRenderer>().material = BlackSmoke;
        SmokeOutside.transform.localScale = new Vector3(5.1f, 5.1f, 5.1f);
    }

    void SmokeState_Disabled()
    {
        SmokeOutside.GetComponent<MeshRenderer>().enabled = false;
        SmokeInside.GetComponent<MeshRenderer>().enabled = false;
    }
}
