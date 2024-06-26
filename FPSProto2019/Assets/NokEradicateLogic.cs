using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public class NokEradicateLogic : MonoBehaviour
{
    float Timer = 0f;
    float TIMER_MAX = 2f;
    float TIMER_BUFFER = 0.25f;
    float RADIUS_MAX = 30.0f;
    [SerializeField] AnimationCurve lerpCurve;

    bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        DisableEradicateObject();
    }

    void DisableEradicateObject()
    {
        gameObject.transform.position = Vector3.zero;

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.transform.Find("Sphere").gameObject.GetComponent<MeshRenderer>().enabled = false;

        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    List<GameObject> AbilityObjectsTouched;
    public void UseAbility_NokEradicate(Vector3 _position)
    {
        

        // Enable
        isActive = true;

        // Move to player's location
        gameObject.transform.position = _position;

        // Enable Mesh Renderer
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        // Determine if entity is 'Utility'
        if(other.CompareTag("AbilityObject"))
        {
            print(other.gameObject.name);

            // Add to list of objects to cycle through
            if (other.gameObject.GetComponent<IceWall_Logic>())
            {
                other.gameObject.GetComponent<IceWall_Logic>().DestroyAbility();
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Smoke"))
            {
                other.gameObject.transform.parent.GetComponent<SmokeLogic>().SetSmokeState(SmokeState.Disabled);
                // other.gameObject.GetComponent<SmokeLogic>().SetSmokeState(SmokeState.Disabled);
            }
            if( other.gameObject.GetComponent<NokWallLogic>() )
            {
                other.gameObject.GetComponent<NokWallLogic>().SetWallState(NokWallState.Disabled);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Timer += Time.deltaTime;
            if(Timer > (TIMER_MAX + TIMER_BUFFER))
            {
                Timer = 0f;

                isActive = false;

                DisableEradicateObject();
            }
            else
            {
                // Expand to maximum size over time
                float perc = Mathf.Lerp(0f, TIMER_MAX, Timer);

                float tempPerc = lerpCurve.Evaluate(perc);

                // Convert Material opacity based on AnimCurve
                Material objMat = gameObject.GetComponent<MeshRenderer>().material;
                Color objMatColor =  objMat.color;

                // Converts from Full Alpha to SemiTransparent
                objMatColor.a = 1 - Mathf.Clamp(tempPerc, .25f, 0.75f);
                objMat.color = objMatColor;

                // Radius is set based on Max Time.
                float tempSize = tempPerc * RADIUS_MAX;
                gameObject.transform.localScale = new Vector3(tempSize, tempSize, tempSize);

                // Eradicate obj exists for MaxTime + Buffer

                // Each frame, any newly touched entities will be disabled
            }
        }
    }
}
