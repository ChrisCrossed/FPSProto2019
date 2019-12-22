using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Requirements
[RequireComponent(typeof(Rigidbody))]

public class scr_PlayerController : scr_PlayerInput
{
    // Pre-Defined
    XInputDotNetPure.ButtonState Pressed = XInputDotNetPure.ButtonState.Pressed;
    XInputDotNetPure.ButtonState Released = XInputDotNetPure.ButtonState.Released;

    GameObject this_Player;
    Rigidbody this_RigidBody;

    // Start is called before the first frame update
    void Start()
    {
        this_Player = gameObject;
        this_RigidBody = this_Player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Capture player input first
        base.UpdatePlayerInput();

        if (playerInput.DPad_Pressed_Left == Pressed && playerInput_OLD.DPad_Pressed_Right == Released)
        {
            print("Pressed");
        }
    }
}
