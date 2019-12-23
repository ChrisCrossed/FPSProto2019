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
    GameObject this_Camera_Object;
    Camera this_Camera;

    [SerializeField]
    ControllerType currentInputType = ControllerType.Controller;

    // Mouse Camera Rotation Information
    float f_CameraVertRotation;

    // Start is called before the first frame update
    void Start()
    {
        this_Player = gameObject;
        this_RigidBody = this_Player.GetComponent<Rigidbody>();
        this_Camera_Object = this_Player.transform.GetChild(0).gameObject;
        this_Camera = this_Camera_Object.GetComponent<Camera>();

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

        // Store current gravity velocity
        float currVertVelocity = this_RigidBody.velocity.y;

        if (playerInput.InputType == ControllerType.Controller)
        {
            if (playerInput.DPad_Pressed_Left == Pressed && playerInput_OLD.DPad_Pressed_Right == Released)
            {
                print("Pressed");
            }
        }
        else // Keyboard/Mouse Input
        {
            // Mouse Input first
            MouseMoveUpdate();

            // Create velocity information
            Vector3 tempVel = new Vector3();

            #region Create initial movement vector
            if (playerInput.KM_Forward)
                tempVel.z = 1.0f;
            else if(playerInput.KM_Backward)
                tempVel.z = -1.0f;

            if (playerInput.KM_Strafe_Left)
                tempVel.x = -1.0f;
            else if (playerInput.KM_Strafe_Right)
                tempVel.x = 1.0f;
            #endregion

            // Normalize Vector
            tempVel.Normalize();

            Vector3 v3_PlayerVelocity = this_RigidBody.transform.rotation * tempVel;

            // Multiply by 'Speed' (Temp)
            v3_PlayerVelocity *= 5.0f;

            // Replace gravity
            v3_PlayerVelocity.y = currVertVelocity;

            // Assign new velocity to player
            this_RigidBody.velocity = v3_PlayerVelocity;
        }
    }

    void MouseMoveUpdate()
    {
        // Capture current player euler rotation
        Vector3 playerRotation = this_RigidBody.transform.eulerAngles;

        // Capture mouse X/Y input
        Vector2 tempMouseMove = playerInput.KM_Mouse_Movement;

        // Rotate character based on X
        playerRotation.y += tempMouseMove.x;

        // Rotate camera X & cap
        f_CameraVertRotation += playerInput.KM_Mouse_Movement.y;
        f_CameraVertRotation = Mathf.Clamp(f_CameraVertRotation, -85f, 85f);

        Vector3 cameraEuler = this_Camera_Object.transform.eulerAngles;
        cameraEuler.x = f_CameraVertRotation;

        // Camera Y matches player Y
        cameraEuler.y = playerRotation.y;

        // Apply final rotation
        this_RigidBody.transform.eulerAngles = playerRotation;
        this_Camera_Object.transform.eulerAngles = cameraEuler;
    }
}