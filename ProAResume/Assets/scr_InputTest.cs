using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_InputTest : MonoBehaviour
{
    [SerializeField] GameObject[] wallObjects = new GameObject[4];

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject wall in wallObjects)
        {
            wall.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
