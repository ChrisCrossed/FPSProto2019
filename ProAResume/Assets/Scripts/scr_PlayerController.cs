using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct PlayerSettings
{
    private float _mouseSensitivity;
    private int _fieldOfView;

    public void ResetInputSettings()
    {
        _mouseSensitivity = 1.0f;
    }

    public float MouseSensitivity
    {
        set { _mouseSensitivity = value; }
        get { return _mouseSensitivity; }
    }

    public int FieldOfView
    {
        set { _fieldOfView = value; }
        get { return _fieldOfView; }
    }
}

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

    #region Player Settings
    PlayerSettings playerSettings;
    [SerializeField] [Range(0.01f, 10f)] float playerSettings_MouseSensitivity = 1.0f;
    [SerializeField] [Range(60, 90)] int playerSettings_FieldOfView = 85;
    #endregion

    #region Movement Standards
    [SerializeField] static float MAX_MOVE_SPEED = 5.0f;
    #endregion

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

        // Temporary Input Settings Override
        playerSettings.MouseSensitivity = playerSettings_MouseSensitivity;
        playerSettings.FieldOfView = playerSettings_FieldOfView;
        this_Camera.fieldOfView = playerSettings.FieldOfView;
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

            // Move player based on WASD input
            CalculateMovementVelocity(currVertVelocity);
        }
    }

    void MouseMoveUpdate()
    {
        // Capture current player euler rotation
        Vector3 playerRotation = this_RigidBody.transform.eulerAngles;

        // Capture mouse X/Y input
        Vector2 tempMouseMove = playerInput.KM_Mouse_Movement * playerSettings.MouseSensitivity;

        // Rotate character based on X
        playerRotation.y += tempMouseMove.x;

        // Rotate camera X & cap
        f_CameraVertRotation += playerInput.KM_Mouse_Movement.y * playerSettings.MouseSensitivity;
        f_CameraVertRotation = Mathf.Clamp(f_CameraVertRotation, -85f, 85f);

        Vector3 cameraEuler = this_Camera_Object.transform.eulerAngles;
        cameraEuler.x = f_CameraVertRotation;

        // Camera Y matches player Y
        cameraEuler.y = playerRotation.y;

        // Apply final rotation
        this_RigidBody.transform.eulerAngles = playerRotation;
        this_Camera_Object.transform.eulerAngles = cameraEuler;
    }

    void CalculateMovementVelocity( float f_CurrVertVelocity_ )
    {
        // Create velocity information
        Vector3 tempVel = new Vector3();

        #region Create initial movement vector
        if (playerInput.KM_Forward)
            tempVel.z = 1.0f;
        else if (playerInput.KM_Backward)
            tempVel.z = -1.0f;

        if (playerInput.KM_Strafe_Left)
            tempVel.x = -1.0f;
        else if (playerInput.KM_Strafe_Right)
            tempVel.x = 1.0f;
        #endregion

        // Normalize movement Vector
        tempVel.Normalize();

        // Adjust movement vector in-line with player rotation
        Vector3 v3_PlayerVelocity = this_RigidBody.transform.rotation * tempVel;

        #region Lerp current velocity into desired velocity
        // Player velocity last frame
        Vector3 v3_OldVelocity = this_RigidBody.velocity;
        
        // Find new velocity, set to a high percentage of the two combined
        Vector3 v3_NewVelocity = Vector3.Lerp(v3_OldVelocity, v3_PlayerVelocity * MAX_MOVE_SPEED, 0.25f);

        // Compare against itself
        float f_testVel_InputVel = Vector3.Magnitude(v3_PlayerVelocity * MAX_MOVE_SPEED);
        float f_testVel_NewVel = Vector3.Magnitude(v3_NewVelocity);

        if(f_testVel_InputVel != 0f)
        {
            if (f_testVel_NewVel / f_testVel_InputVel >= 0.94f)
                v3_NewVelocity = tempVel * MAX_MOVE_SPEED;
        }
        #endregion

        print(v3_NewVelocity.magnitude);

        // Replace gravity
        v3_NewVelocity.y = f_CurrVertVelocity_;

        // Assign new velocity to player
        this_RigidBody.velocity = v3_NewVelocity;
    }
}
