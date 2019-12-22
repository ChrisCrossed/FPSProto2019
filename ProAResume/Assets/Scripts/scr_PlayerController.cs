using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Requirements
[RequireComponent(typeof(Rigidbody))]

public class scr_PlayerController : scr_PlayerInput
{
    GameObject this_Player;
    Rigidbody this_RigidBody;

    // Start is called before the first frame update
    void Start()
    {
        this_Player = gameObject;
        this_RigidBody = this_Player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
