using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GarageScript : MonoBehaviour
{
    float YPos;
    bool GoUp;
    float GoUpTime = 4.0f;

    [SerializeField]
    AnimationCurve AnimCurve;

    // Start is called before the first frame update
    void Start()
    {
        YPos = gameObject.transform.position.y;
        GoUp = false;
    }

    float Timer = 0f;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GoUp = true;
        }

        if(GoUp)
        {
            Timer += Time.deltaTime / GoUpTime;

            float perc = AnimCurve.Evaluate(Timer);

            Vector3 newPos = gameObject.transform.position;
            newPos.y = (perc * 8f) + YPos;
            gameObject.transform.position = newPos;
        }

        print(Timer);

        if(Timer > 1.5f)
        {
            gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
