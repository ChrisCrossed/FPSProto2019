﻿using System.Collections;
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

    [SerializeField]
    ControllerType currentInputType = ControllerType.Controller;

    // Start is called before the first frame update
    void Start()
    {
        this_Player = gameObject;
        this_RigidBody = this_Player.GetComponent<Rigidbody>();

        // Set initial input as a controller. Should only be performed this once.
        SetControllerType = currentInputType;
    }

    public ControllerType SetControllerType
    {
        set
        {
            // Sets locally
            currentInputType = value;

            // Passes information to PlayerInput controller
            playerInput.InputType = currentInputType;
        }
    }

    private void FixedUpdate()
    {
        // Capture player input first
        base.UpdatePlayerInput();

        // Store current velocity
        Vector3 currVel = this_RigidBody.velocity;

        // Store current gravity velocity
        float currVertVelocity = currVel.y;

        if (playerInput.InputType == ControllerType.Controller)
        {
            if (playerInput.DPad_Pressed_Left == Pressed && playerInput_OLD.DPad_Pressed_Right == Released)
            {
                print("Pressed");
            }
        }
        else // Keyboard/Mouse Input
        {
            // Create velocity information
            Vector2 tempVel = new Vector2();

            #region Create initial movement vector
            if (playerInput.KM_Forward)
                tempVel.y = 1.0f;
            else if(playerInput.KM_Backward)
                tempVel.y = -1.0f;

            if (playerInput.KM_Strafe_Left)
                tempVel.x = -1.0f;
            else if (playerInput.KM_Strafe_Right)
                tempVel.x = 1.0f;
            #endregion

            // Normalize Vector
            tempVel.Normalize();

            // Apply velocity to player
            currVel.x = tempVel.x * 5.0f; // Horizontal/Strafing
            currVel.y = currVertVelocity; // Vertical
            currVel.z = tempVel.y * 5.0f; // Forward/Backward

            this_RigidBody.velocity = currVel;
        }
    }
}
