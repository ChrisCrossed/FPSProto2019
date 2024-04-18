using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NokWallState
{
    Active,
    Shattered,
    Disabled
}

public class NokWallLogic : MonoBehaviour
{
    float Timer;

    [SerializeField]
    static float TIMER_MAX = 30f;

    GameObject[] Walls;
    NokWallState wallState;

    // Start is called before the first frame update
    void Start()
    {
        Timer = TIMER_MAX;

        wallState = NokWallState.Active;

        Walls = new GameObject[4];
        Walls[0] = gameObject.transform.Find("Cube_0").gameObject;
        Walls[1] = gameObject.transform.Find("Cube_1").gameObject;
        Walls[2] = gameObject.transform.Find("Cube_2").gameObject;
        Walls[3] = gameObject.transform.Find("Cube_3").gameObject;
    }

    public void SetWallState(NokWallState _wallState)
    {
        switch (_wallState)
        {
            case NokWallState.Disabled:
                foreach(GameObject wall in Walls)
                {
                    wall.GetComponent<MeshRenderer>().enabled = false;
                }
                SetWallState(NokWallState.Shattered);
                break;

            case NokWallState.Shattered:
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                break;
            
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
        if(collision.gameObject.tag == "Player")
        {
            SetWallState(NokWallState.Shattered);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        return;

        if(Timer > 0f)
        {
            Timer -= Time.deltaTime;
            print(Timer);

            if(Timer < 0f)
            {
                SetWallState(NokWallState.Disabled);
            }
        }
    }
}
