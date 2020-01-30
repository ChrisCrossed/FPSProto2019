using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState
{
    Normal,
    ADS,
    Moving
}

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
    CapsuleCollider this_Collider;
    [SerializeField] PhysicMaterial[] physMatList;
    #endregion

    #region Weapon Position Lerp
    GameObject go_HUD_WeaponModel;
    GameObject go_HUD_WeaponPos_Normal;
    GameObject go_HUD_WeaponPos_ADS;
    float f_WeaponLerpTime = 0f;
    static float WEAPON_LERP_PERC_MAX = 0.15f;
    public AnimationCurve aCurve_WeaponLerp;
    #endregion

    #region Weapon Camera Lerp
    GameObject go_WeaponCam;
    Camera WeaponCam;
    GameObject go_MDL_WeaponModel;
    GameObject go_MDL_WeaponPos_Normal;
    GameObject go_MDL_WeaponPos_ADS;
    #endregion

    #region Raycast Objects
    GameObject[] rayObj_Jump = new GameObject[5];
    GameObject[] rayObj_Crouch = new GameObject[5];
    #endregion

    #region Gun Object
    scr_GunFire GunWeapon;
    #endregion

    // Mouse Camera Rotation Information
    float f_CameraVertRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Turn off Mouse icon
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Set defaults
        #region Player Defaults
        this_Player = gameObject;
        this_RigidBody = this_Player.GetComponent<Rigidbody>();
        this_Camera_Object = this_Player.transform.Find("PlayerCamera").gameObject;
        this_Camera = this_Camera_Object.GetComponent<Camera>();
        #endregion
        #region Movement Standards
        this_Collider = this_Player.GetComponent<CapsuleCollider>();
        #endregion
        #region Weapon Objects
        go_HUD_WeaponModel = this_Camera_Object.transform.Find("WeaponMdl").gameObject;
        go_HUD_WeaponPos_Normal = this_Camera_Object.transform.Find("WeapPnt_Normal").gameObject;
        go_HUD_WeaponPos_ADS = this_Camera_Object.transform.Find("WeapPnt_ADS").gameObject;
        #endregion
        #region Weapon Cam Objects
        go_WeaponCam = GameObject.Find("WeaponCam").gameObject;
        WeaponCam = go_WeaponCam.GetComponent<Camera>();
        go_MDL_WeaponModel = go_WeaponCam.transform.Find("WeaponMdl").gameObject;
        go_MDL_WeaponPos_Normal = go_WeaponCam.transform.Find("WeapPnt_Normal").gameObject;
        go_MDL_WeaponPos_ADS = go_WeaponCam.transform.Find("WeapPnt_ADS").gameObject;
        #endregion
        #region GunWeapon
        GunWeapon = GameObject.Find("Player").transform.Find("PlayerCamera").transform.Find("WeaponMdl").GetComponent<scr_GunFire>();
        #endregion
        #region Raycast Objects
        rayObj_Jump[0] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Jump").gameObject;
        rayObj_Jump[1] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Jump_FL").gameObject;
        rayObj_Jump[2] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Jump_FR").gameObject;
        rayObj_Jump[3] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Jump_BL").gameObject;
        rayObj_Jump[4] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Jump_BR").gameObject;
        

        rayObj_Crouch[0] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Crouch").gameObject;
        rayObj_Crouch[1] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Crouch_FL").gameObject;
        rayObj_Crouch[2] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Crouch_FR").gameObject;
        rayObj_Crouch[3] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Crouch_BL").gameObject;
        rayObj_Crouch[4] = this_Player.transform.Find("RaycastObjects").transform.Find("Ray_Crouch_BR").gameObject;
        #endregion

        // Set initial input as a controller. Should only be performed this once.
        SetControllerType = currentInputType;

        // Temporary Input Settings Override
        playerSettings.MouseSensitivity = playerSettings_MouseSensitivity;
        playerSettings.FieldOfView = playerSettings_FieldOfView;
        this_Camera.fieldOfView = playerSettings.FieldOfView;
    }

    void AssignGravity( bool b_IsOnGround_ )
    {
        if(b_IsOnGround_)
            this_RigidBody.AddForce(Physics.gravity, ForceMode.Acceleration);
        else
            this_RigidBody.AddForce(Physics.gravity * 2f, ForceMode.Acceleration);
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

        // Test if player is crouching
        bool isCrouching = CrouchCheck();

        // Determine if on ground
        RaycastHit rayHit = GroundRaycastCheck();

        // Test if the player tries to jump
        if(!isCrouching)
        {
            PlayerPressedJump = TryJump(rayHit);
        }

        // Apply input-based velocity
        ApplyControllerBasedVelocity(isCrouching, rayHit);

        // Determine if gun is switching positions
        bool weaponMoving = ADSCheck();

        // If gun isn't switching positions, determine if player is firing the gun
        if( !weaponMoving ) FireGunCheck();
    }

    void ApplyControllerBasedVelocity(bool _isCrouching, RaycastHit rayHit_)
    {
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
            CalculateMovementVelocity(currVertVelocity, _isCrouching, rayHit_);
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

    bool PlayerPressedJump;
    static float GROUND_TOUCH_TIMER_MAX = 0.025f;
    bool JumpButtonState;
    bool TryJump( RaycastHit rayHit_ )
    {
        bool playerPressedJump = false;

        if ( rayHit_.distance <= JumpRayCastDistance && GroundTouchTimer >= GROUND_TOUCH_TIMER_MAX)
        {
            if (playerInput.KM_Button_Jump)
            {
                if(!JumpButtonState)
                {
                    // State that player pressed to jump
                    playerPressedJump = true;
                    JumpButtonState = true;

                    // Setting to 'extreme' negative value to ensure player can't immediately 2x jump
                    GroundTouchTimer = -0.05f;
                }
            }
            else
            {
                // Old jump state for input comparison
                JumpButtonState = false;
            }
        }

        return playerPressedJump;
    }

    static float PLAYER_CROUCH_HEIGHT = 0.5f;
    static float PLAYER_STAND_HEIGHT = 1.0f;
    static float PLAYER_CROUCH_SCALE = 4f;
    bool CrouchCheck()
    {
        bool isCrouching = false;

        Vector3 v3_PlayerScale = this_Player.transform.localScale;

        if(playerInput.KM_Button_Crouch)
        {
            isCrouching = true;

            if(v3_PlayerScale.y > PLAYER_CROUCH_HEIGHT)
            {
                v3_PlayerScale.y -= Time.fixedDeltaTime * PLAYER_CROUCH_SCALE;

                if ( v3_PlayerScale.y < PLAYER_CROUCH_HEIGHT ) v3_PlayerScale.y = PLAYER_CROUCH_HEIGHT;
            }
        }
        else
        {
            isCrouching = false;

            if( v3_PlayerScale.y < PLAYER_STAND_HEIGHT)
            {
                // Check if there's no ceiling above the player (room to stand)
                RaycastHit hit = CrouchRaycastCheck(v3_PlayerScale.y);

                // If there's nothing above the player, stand
                if (hit.collider == null)
                {
                    v3_PlayerScale.y += Time.fixedDeltaTime * PLAYER_CROUCH_SCALE;

                    if (v3_PlayerScale.y > PLAYER_STAND_HEIGHT) v3_PlayerScale.y = PLAYER_STAND_HEIGHT;
                }
            }
        }

        this_Player.transform.localScale = v3_PlayerScale;

        return isCrouching;
    }

    static float JUMP_VELOCITY = 8f;
    static float WALK_PENALTY_PERC = 0.5f;
    static float CROUCH_PENALTY_PERC = 0.4f;
    static float ADS_PENALTY_PERC = 0.8f;
    void CalculateMovementVelocity( float f_CurrVertVelocity_, bool _isCrouching, RaycastHit rayHit_ )
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
        float f_LerpRate = 10f * Time.fixedDeltaTime;

        // Determine potential max move speed based on stand/walk/crouch
        float f_TempMaxSpeed = MAX_MOVE_SPEED;

        // Can only alter ground velocity if the player is touching the ground
        if (rayHit_.collider != null)
        {
            if (playerInput.KM_Button_Walk && !_isCrouching) f_TempMaxSpeed *= WALK_PENALTY_PERC;
            if(_isCrouching) f_TempMaxSpeed *= CROUCH_PENALTY_PERC;
        }

        // If player is not holding the weapon at waist level, take a minor move speed penalty
        if (WeaponState != WeaponState.Normal)
            f_TempMaxSpeed *= ADS_PENALTY_PERC;

        // Smoothly transition from old velocity into new velocity
        Vector3 v3_NewVelocity = Vector3.Lerp(v3_OldVelocity, v3_PlayerVelocity * f_TempMaxSpeed, f_LerpRate);

        // If nearly max speed, just cap at max speed
        if (v3_NewVelocity.magnitude / f_TempMaxSpeed > 0.96f)
            v3_NewVelocity = v3_NewVelocity.normalized * f_TempMaxSpeed;
        
        // If nearly zero, just set at zero.
        else if (v3_NewVelocity.magnitude <= 0.01f)
            v3_NewVelocity = Vector3.zero;
        #endregion

        // Replace gravity
        v3_NewVelocity.y = f_CurrVertVelocity_;
        if(PlayerPressedJump)
        {
            v3_NewVelocity.y = JUMP_VELOCITY;
        }

        // If player not giving input && slope of ground is greater than certain degree, change PhysMat
        bool hasInput = (tempVel != Vector3.zero);
        bool hasSlope = (SlopeAngle > 0f);
        ChangePhysMatStats(!hasInput && hasSlope);

        // Assign new velocity to player
        this_RigidBody.velocity = v3_NewVelocity;
    }

    float f_LerpPerc_Old;
    bool ADSCheck()
    {
        bool GunMoving = false;

        // Fire/ADS Input
        if (playerInput.InputType == ControllerType.KeyMouse)
        {
            #region Increase/Decrease & Cap weapon lerp timer
            // If the button is held down, determine if weapon switches positions
            if (playerInput.KM_Mouse_Right)
            {
                // If we're less than 100%, move the weapon
                if (f_WeaponLerpTime < WEAPON_LERP_PERC_MAX)
                {
                    GunMoving = true;

                    f_WeaponLerpTime += Time.fixedDeltaTime;

                    if (f_WeaponLerpTime > WEAPON_LERP_PERC_MAX) 
                        f_WeaponLerpTime = WEAPON_LERP_PERC_MAX;
                }
            }
            else
            {
                if (f_WeaponLerpTime > 0f)
                {
                    GunMoving = true;

                    f_WeaponLerpTime -= Time.fixedDeltaTime;

                    if (f_WeaponLerpTime < 0f) f_WeaponLerpTime = 0f;
                }
            }
            #endregion

            #region Lerp weapon position
            float f_LerpPerc = f_WeaponLerpTime / WEAPON_LERP_PERC_MAX;
            if( f_LerpPerc != f_LerpPerc_Old)
            {
                #region Weapon Model Position
                // Vector3 v3_WeaponHUDPos = Vector3.Lerp(go_HUD_WeaponPos_Normal.transform.position, go_HUD_WeaponPos_ADS.transform.position, f_LerpPerc);
                Vector3 v3_WeaponHUDPos = Vector3.Lerp(go_HUD_WeaponPos_Normal.transform.position, go_HUD_WeaponPos_ADS.transform.position, aCurve_WeaponLerp.Evaluate(f_LerpPerc));

                go_HUD_WeaponModel.transform.position = v3_WeaponHUDPos;
                #endregion
                #region Weapon Cam Position
                Vector3 v3_WeaponCamPos = Vector3.Lerp(go_MDL_WeaponPos_Normal.transform.position, go_MDL_WeaponPos_ADS.transform.position, aCurve_WeaponLerp.Evaluate(f_LerpPerc));
                go_MDL_WeaponModel.transform.position = v3_WeaponCamPos;
                #endregion

                f_LerpPerc_Old = f_LerpPerc;
            }
            #endregion

            // WeaponState Information
            if (f_LerpPerc == 0.0f)
                WeaponState = WeaponState.Normal;
            else if (f_LerpPerc == 1.0f)
                WeaponState = WeaponState.ADS;
            else
                WeaponState = WeaponState.Moving;
        }
        else
        {

        }

        return GunMoving;
    }

    bool mouseLeft_Old;
    void FireGunCheck()
    {
        // If the player fired this frame, but not a previous frame
        if(playerInput.KM_Mouse_Left && (playerInput.KM_Mouse_Left != mouseLeft_Old))
        {
            if (GunWeapon)
            {
                GunWeapon.FireGun(WeaponState);
            }
        }

        // Store last known position
        mouseLeft_Old = playerInput.KM_Mouse_Left;
    }

    private WeaponState this_WeaponState;
    public WeaponState WeaponState
    {
        get
        {
            return this_WeaponState;
        }
        private set
        {
            this_WeaponState = value;
        }
    }

    float JumpRayCastDistance = 0.17f;
    float GroundTouchTimer = 0f;
    float SlopeAngle;
    RaycastHit GroundRaycastCheck()
    {
        RaycastHit hit = new RaycastHit();
        RaycastHit hitTemp = new RaycastHit();
        float f_ShortestDistance = 5f;
        SlopeAngle = 0f;

        // Vector3's for slope checks
        Vector3 centerVectorHitPos = new Vector3();
        Vector3 highestVectorHitPos = new Vector3();

        int tempInfo = 10;

        // Run through all Raycast objects to see if one finds the ground
        for (int i = 0; i < rayObj_Jump.Length; ++i)
        {
            if (Physics.Raycast(rayObj_Jump[i].transform.position, Vector3.down, out hitTemp, JumpRayCastDistance))
            {
                // Temporarily store the center ground hit position
                if( i == 0 )
                    centerVectorHitPos = hitTemp.point;

                if(hitTemp.distance < f_ShortestDistance)
                {
                    tempInfo = i;
                    print(hitTemp.collider.tag);

                    f_ShortestDistance = hitTemp.distance;
                    hit = hitTemp;

                    // Also store information if a closer position is found than the center point for slope checking
                    if( i > 0 ) // Only runs on non-center raycast points since 0 == center point
                    {
                        highestVectorHitPos = hitTemp.point;
                    }
                }
            }
        }

        print("Shortest: " + tempInfo);

        // If highestVectorHitPos is filled, we're on an angle
        if( highestVectorHitPos != Vector3.zero )
        {
            float heightDiff = Mathf.Abs(highestVectorHitPos.y - centerVectorHitPos.y);
            if (heightDiff != 0f) SlopeAngle = 5f;

            /*
            float angle = Vector3.SignedAngle(highestVectorHitPos, centerVectorHitPos, Vector3.up);
            if (angle < 0) angle *= -1;
            SlopeAngle = angle;
            */
        }

        if(hit.collider != null)
        {
            // One found the ground (presumably [0]), so re-assign normal gravity
            AssignGravity(true);

            // Begin increasing ground touch timer
            if (GroundTouchTimer < GROUND_TOUCH_TIMER_MAX)
            {
                GroundTouchTimer += Time.fixedDeltaTime;
                if (GroundTouchTimer >= GROUND_TOUCH_TIMER_MAX) GroundTouchTimer = GROUND_TOUCH_TIMER_MAX;
            }
        }
        // If the RaycastHit doesn't detect an object, reset the timer
        else
        {
            GroundTouchTimer = 0f;

            AssignGravity(false);
        }

        return hit;
    }

    RaycastHit CrouchRaycastCheck( float playerHeight_ )
    {
        RaycastHit hit = new RaycastHit();

        float CrouchRayCastDistance = 0.1f; // buffer (0.95 yPos + 0.05)
        CrouchRayCastDistance += (PLAYER_STAND_HEIGHT - playerHeight_);

        // Run through all Raycast objects to see if one finds the ground
        for (int i = 0; i < rayObj_Crouch.Length; ++i)
        {
            if (Physics.Raycast(rayObj_Crouch[i].transform.position, Vector3.up, out hit, CrouchRayCastDistance))
            {
                break;
            }
        }

        return hit;
    }

    float frictionAmount = 0f;
    void ChangePhysMatStats( bool increaseFriction_)
    {
        if (increaseFriction_)
        {
            if( frictionAmount < 1f)
            {
                frictionAmount += Time.deltaTime * 10f;
                if (frictionAmount > 1f) frictionAmount = 1f;
            }

            this_Collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
        }
        else
        {
            if( frictionAmount > 0f)
            {
                frictionAmount -= Time.deltaTime * 10f;
                if (frictionAmount < 0f) frictionAmount = 0f;
            }

            this_Collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        this_Collider.material.dynamicFriction = frictionAmount;
        this_Collider.material.staticFriction = frictionAmount;
    }
}
