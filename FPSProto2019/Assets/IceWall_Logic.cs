using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall_Logic : MonoBehaviour
{
    GameObject ChildWall;

    // Start is called before the first frame update
    void Start()
    {
        ChildWall = transform.Find("IceWallChild").gameObject;
    }

    public void DestroyAbility()
    {
        // Turn off Mesh Renderer and Box Collider for parent & child objects
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        ChildWall.GetComponent<MeshRenderer>().enabled = false;
    }
}
