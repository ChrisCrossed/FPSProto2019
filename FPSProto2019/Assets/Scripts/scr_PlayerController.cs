using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponState
{
    Normal,
    ADS,
    Ability,
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
    enum GroundTouchState
    { 
        OnGround,
        Crouch,
        Airborne
    }

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
    [SerializeField] static float MAX_MOVE_SPEED = 350.0f;
    CapsuleCollider this_Collider;
    [SerializeField] PhysicMaterial[] physMatList;
    
    private Vector3 groundPointVelocity;
    private Rigidbody groundRigidbody;

    GroundTouchState GroundState;
    static float JUMP_VELOCITY = 4f;
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
    [SerializeField] float MAX_WEAPON_FIRE_RATE = 0.25f;
    float f_WeaponFireRateTimer = 0f;
    #endregion

    #region UI Objects
    GameObject[] UI_Ability_1_Objects;
    GameObject[] UI_Ability_2_Objects;
    GameObject[] UI_Ability_3_Objects;
    GameObject[] UI_Ability_Ult_Objects;

    GameObject[] GO_SnapbackMarker;
    GameObject GO_SnapbackMarker_Timer_UI;
    #endregion

    // Mouse Camera Rotation Information
    float f_CameraVertRotation;

    enum UI_AbilityObjNumber
    {
        UI_AbilityIcon = 0,
        UI_AbilityPip_Main = 1,
        UI_Ability_Letter = 2,
        UI_AbilityPip_Alt = 3
    }

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
        GroundState = new GroundTouchState();
        #endregion
        #region Weapon Objects
        go_HUD_WeaponModel = this_Camera_Object.transform.Find("WeaponMdl").gameObject;
        go_HUD_WeaponPos_Normal = this_Camera_Object.transform.Find("WeapPnt_Normal").gameObject;
        go_HUD_WeaponPos_ADS = this_Camera_Object.transform.Find("WeapPnt_ADS").gameObject;
        #endregion
        #region Ability Objects
        UI_Ability_1_Objects = new GameObject[4];
        UI_Ability_2_Objects = new GameObject[4];
        UI_Ability_3_Objects = new GameObject[4];
        UI_Ability_Ult_Objects = new GameObject[4];

        GO_SnapbackMarker = new GameObject[3];
        GO_SnapbackMarker_Timer_UI = GameObject.Find("UI_Ability_3_Timer");

        Init_UI_AbilityObjects();
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

        playerInput.OverrideControlsWithOfficialControls();

        // Set Update-based (Client-side) input vector defaults
        v3_ClientSideUpdateVelocity = new Vector3();
    }

    void Init_UI_AbilityObjects()
    {
        UI_Ability_3_Objects[(int)UI_AbilityObjNumber.UI_AbilityIcon] = GameObject.Find("UI_Ability_3").gameObject;
        UI_Ability_3_Objects[(int)UI_AbilityObjNumber.UI_AbilityPip_Main] = GameObject.Find("UI_Ability_3_Pip_1").gameObject;
        UI_Ability_3_Objects[(int)UI_AbilityObjNumber.UI_AbilityPip_Alt] = GameObject.Find("UI_Ability_3_Pip_2").gameObject;
        UI_Ability_3_Objects[(int)UI_AbilityObjNumber.UI_Ability_Letter] = GameObject.Find("UI_Text_Ability_3").gameObject;

        GO_SnapbackMarker[0] = GameObject.Find("Marker_Snapback").gameObject;
        GO_SnapbackMarker[1] = GO_SnapbackMarker[0].transform.Find("mdl_snapback_1").gameObject;
        GO_SnapbackMarker[2] = GO_SnapbackMarker[0].transform.Find("mdl_snapback_2").gameObject;

        SetSnapbackMarkerState(false);
        GO_SnapbackMarker_Timer_UI.GetComponent<Image>().fillAmount = 0f;
        SetAbilityPipState(UI_Ability_3_Objects, 1);
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

    Vector3 v3_ClientSideUpdateVelocity;
    // Intending on treating 'Update' as 'Client Side' updates
    private void Update()
    {
        // Get player input values
        base.UpdatePlayerInput();

        // Check if an ability was selected
        if(playerInput.KM_Ability_1 || playerInput.KM_Ability_2 || playerInput.KM_Ability_3 || playerInput.KM_Ability_Ult)
        {
            AbilityManager();
        }
        else if(playerInput.KM_Button_Weapon_Primary || playerInput.KM_Button_Weapon_Secondary || playerInput.KM_Button_Weapon_Melee)
        {
            WeaponManager();
        }

        AbilityTimerUpdate();

        // Mouse Input first
        MouseMoveUpdate();

        #region Add to Vector3 that gets passed into Fixed Update
        Vector3 tempVel = new Vector3();

        if (playerInput.KM_Forward)
            tempVel.z = 1.0f;
        else if (playerInput.KM_Backward)
            tempVel.z = -1.0f;

        if (playerInput.KM_Strafe_Left)
            tempVel.x = -1.0f;
        else if (playerInput.KM_Strafe_Right)
            tempVel.x = 1.0f;

        // Normalize movement Vector
        tempVel.Normalize();

        // Adjust movement vector in-line with player rotation
        v3_ClientSideUpdateVelocity += (this_RigidBody.transform.rotation * tempVel) * Time.deltaTime;
        #endregion

        // State Ability/Weapon Selection
        if(WeaponState != WeaponState.Ability)
        {
            // Determine if gun is switching positions
            bool weaponMoving = ADSCheck();

            if (!weaponMoving) FireGunCheck();
        }
        
    }

    bool PlayerCanJump;
    // Intending on treating 'Fixed Update' as 'Physics Updates' and 'Server Side'-style updates (Tick-rate?)
    private void FixedUpdate()
    {
        // Determine if on ground
        RaycastHit _hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out _hit, PLAYER_STAND_HEIGHT + 5f))
        {
            // print(_hit.distance);
            v3_ClientSideUpdateVelocity = Vector3.ProjectOnPlane(v3_ClientSideUpdateVelocity, -_hit.normal);
        }

        // Get potential Jump request
        // Test if player is crouching
        bool isCrouching = CrouchCheck();

        // Store current gravity velocity
        float currVertVelocity = this_RigidBody.velocity.y;


        v3_ClientSideUpdateVelocity *= MAX_MOVE_SPEED;

        // GroundSnap Timer after user jumps.
        bool snappedToGround = SnapToGround(_hit);

        if (snappedToGround)
        {
            if (playerInput.KM_Button_Jump)
            {
                currVertVelocity = ApplyJumpVelocity();
            }
            else
            {
                if (_hit.distance < JumpRayCastDistance + gameObject.transform.localScale.y)
                {
                    if (_hit.distance > gameObject.transform.localScale.y)
                    {
                        Vector3 newPos = gameObject.transform.position;
                        newPos.y -= Time.deltaTime;

                        if (newPos.y < _hit.point.y + gameObject.transform.localScale.y)
                            newPos.y = _hit.point.y + gameObject.transform.localScale.y;

                        gameObject.transform.position = newPos;
                    }
                }
            }
        }

        v3_ClientSideUpdateVelocity.y = currVertVelocity;

        // Assign new velocity to player
        this_RigidBody.velocity = v3_ClientSideUpdateVelocity;

        v3_ClientSideUpdateVelocity = new Vector3();
    }

    // ************************************************************
    #region Abilities

    // Character Name: Nok
    void AbilityManager()
    {
        WeaponState = WeaponState.Ability;

        ADSCheck(true);

        if (playerInput.KM_Ability_1)
            ActivateAbility_AreaSelectionDash();
        else if (playerInput.KM_Ability_2)
            ActivateAbility_WallPlacement();
        else if (playerInput.KM_Ability_3)
            ActivateAbility_MarkerSnapback();
        else if (playerInput.KM_Ability_Ult)
            ActivateAbility_AreaCleanse();
    }

    void AbilityTimerUpdate()
    {
        if(Timer_UI_Snapback > 0f)
        {
            Timer_UI_Snapback -= Time.deltaTime;
            if(Timer_UI_Snapback < 0f)
            {
                Timer_UI_Snapback = 0f;

                // Kick player back to marked position
                Vector3 snapbackPosition = GO_SnapbackMarker[0].transform.position;
                snapbackPosition.y += this_Player.gameObject.GetComponent<CapsuleCollider>().height / 2f;
                this_Player.transform.position = snapbackPosition;

                // Turn off snapback marker
                SetSnapbackMarkerState(false);
            }

            float perc = Timer_UI_Snapback / TIMER_UI_SNAPBACK_MAX;
            float greenAmt = 1.0f;
            float redAmt = 1.0f;

            if(perc > 0.5f)
            {
                redAmt = (1.0f - perc) * 2f;
            }
            else
            {
                greenAmt = perc * 2f;
            }

            // Set UI color based on perc.
            Color newColor = new Color(redAmt, greenAmt, 0f);
            GO_SnapbackMarker_Timer_UI.GetComponent<Image>().color = newColor;
            GO_SnapbackMarker_Timer_UI.GetComponent<Image>().fillAmount = perc;
        }
    }

    // 'Left' (Q) Ability
    // --- Smoke/Wall Dash
    void ActivateAbility_AreaSelectionDash()
    {
        
    }


    // 'Right' (E) Ability
    // --- 3 Wall Placement
    void ActivateAbility_WallPlacement()
    {

    }


    // 'Free' (C) Ability
    //  --- Marker Snapback
    float Timer_UI_Snapback = 0f;
    static float TIMER_UI_SNAPBACK_MAX = 5.0f;
    int AbilityNumUses_Snapback = 1;
    static int ABILITY_NUM_USES_SNAPBACK_MAX = 2;
    void ActivateAbility_MarkerSnapback()
    {
        if(AbilityNumUses_Snapback > 0)
        {
            AbilityNumUses_Snapback--;

            #region Manage the UI object within AbilityTimerUpdate

            Timer_UI_Snapback = TIMER_UI_SNAPBACK_MAX;
            SetAbilityPipState(UI_Ability_3_Objects, AbilityNumUses_Snapback);
            
            #endregion

            #region Manage the in-game marker for knowledge
            // Get player position and raycast to ground
            RaycastHit _hit;
            LayerMask layerMask = LayerMask.GetMask("Default");
            if (Physics.Raycast(gameObject.transform.position + new Vector3(0f, 10f, 0f), Vector3.down, out _hit, 15f, layerMask))
            {
                GO_SnapbackMarker[0].gameObject.transform.position = _hit.point;
                SetSnapbackMarkerState(true);
            }
            #endregion
        }


        // GO_SnapbackMarker[0].gameObject.transform.position

        // UI_Ability_1_Objects[(int)UI_AbilityObjNumber.UI_AbilityPip_Main].GetComponent<Image>().color = activeColor;
    }

    void SetSnapbackMarkerState(bool state)
    {
        foreach (GameObject go in GO_SnapbackMarker)
        {
            if (go.GetComponent<MeshRenderer>())
            {
                go.GetComponent<MeshRenderer>().enabled = state;
            }
        }
    }

    void SetAbilityPipState(GameObject[] UI_MarkerArray, int NumCharges)
    {
        Color activeColor = Color.cyan;

        switch (NumCharges)
        {
            case 2:
                UI_MarkerArray[(int)UI_AbilityObjNumber.UI_AbilityPip_Main].GetComponent<Image>().color = activeColor;
                UI_MarkerArray[(int)UI_AbilityObjNumber.UI_AbilityPip_Alt].GetComponent<Image>().color = activeColor;
                break;

            case 1:
                UI_MarkerArray[(int)UI_AbilityObjNumber.UI_AbilityPip_Main].GetComponent<Image>().color = activeColor;
                UI_MarkerArray[(int)UI_AbilityObjNumber.UI_AbilityPip_Alt].GetComponent<Image>().color = Color.white;
                break;

            case 0:
                UI_MarkerArray[(int)UI_AbilityObjNumber.UI_AbilityPip_Main].GetComponent<Image>().color = Color.white;
                UI_MarkerArray[(int)UI_AbilityObjNumber.UI_AbilityPip_Alt].GetComponent<Image>().color = Color.white;
                break;

            default:
                break;
        }
    }

    // Ultimate (X) Ability
    // --- Area Boardwipe
    void ActivateAbility_AreaCleanse()
    {

    }

    #endregion
    // ************************************************************


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

    float ApplyJumpVelocity()
    {
        JumpResetTimer = 0f;

        return JUMP_VELOCITY;
    }

    float JumpResetTimer = 0f;
    const float JUMP_TIMER_MAX = 0.5f;
    bool SnapToGround(RaycastHit _rayHit)
    {
        bool canJump = false;

        // If not enough time has passed, don't allow a jump
        if(JumpResetTimer < JUMP_TIMER_MAX)
        {
            canJump = false;

            JumpResetTimer += Time.fixedDeltaTime;
            if(JumpResetTimer > JUMP_TIMER_MAX)
            {
                JumpResetTimer = JUMP_TIMER_MAX;
            }
        }

        // Meant to continue on as the function runs each frame
        float playerTotalRayDistance = gameObject.transform.localScale.y + JumpRayCastDistance;
        if(_rayHit.distance < playerTotalRayDistance && JumpResetTimer == JUMP_TIMER_MAX)
        {
            canJump = true;
        }

        return canJump;
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

    void WeaponManager()
    {
        if (WeaponState == WeaponState.Ability)
            WeaponState = WeaponState.Normal;
    }

    float f_LerpPerc_Old;
    bool ADSCheck(bool usingAbility = false)
    {
        bool GunMoving = false;

        // Fire/ADS Input
        if (playerInput.InputType == ControllerType.KeyMouse)
        {
            #region Increase/Decrease & Cap weapon lerp timer
            // If the button is held down, determine if weapon switches positions
            if (playerInput.KM_Mouse_Right && !usingAbility)
            {
                // If we're less than 100%, move the weapon
                if (f_WeaponLerpTime < WEAPON_LERP_PERC_MAX)
                {
                    GunMoving = true;

                    f_WeaponLerpTime += Time.deltaTime;

                    if (f_WeaponLerpTime > WEAPON_LERP_PERC_MAX) 
                        f_WeaponLerpTime = WEAPON_LERP_PERC_MAX;
                }
            }
            else
            {
                if (f_WeaponLerpTime > 0f)
                {
                    GunMoving = true;

                    f_WeaponLerpTime -= Time.deltaTime;

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

            // Ensure weapon is visible if not using an ability
            go_MDL_WeaponModel.GetComponent<MeshRenderer>().enabled = true;
            if (!usingAbility)
            {
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
                go_MDL_WeaponModel.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            // Saving for potential future Controller support
        }

        return GunMoving;
    }

    bool mouseLeft_Old;
    void FireGunCheck()
    {
        f_WeaponFireRateTimer -= Time.deltaTime;
        if (f_WeaponFireRateTimer < 0f) f_WeaponFireRateTimer = 0f;

        // If the player fired this frame, but not a previous frame
        if(playerInput.KM_Mouse_Left && (playerInput.KM_Mouse_Left != mouseLeft_Old))
        {
            // As long as enough time between shots has passed, continue
            if (f_WeaponFireRateTimer == 0f)
            {
                // Ensure weapon object exists
                if (GunWeapon)
                {
                    // Fire weapon
                    GunWeapon.FireGun(WeaponState);

                    // Reset weapon fire rate
                    f_WeaponFireRateTimer = MAX_WEAPON_FIRE_RATE;
                }
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

    public float JumpRayCastDistance = 0.17f;
    

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


    #region Unused

    /*
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

    static float JUMP_VELOCITY = 7f;
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

        // TEST - Determine slope here
        print(rayHit_.distance);

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
        if(PlayerCanJump)
        {
            v3_NewVelocity.y = JUMP_VELOCITY;
        }

        // If player not giving input && slope of ground is greater than certain degree, change PhysMat
        bool hasInput = (tempVel != Vector3.zero);
        bool hasSlope = (SlopeAngle > 0f);
        ChangePhysMatStats((!hasInput && hasSlope), OnMovingPlatform);

        // Assign new velocity to player
        this_RigidBody.velocity = v3_NewVelocity;
    }

    
    float frictionAmount = 0f;
    void ChangePhysMatStats( bool increaseFriction_, bool onMovingPlatform_ )
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
            // Slightly increase friction amount if player is on a moving platform
            float tempFrictionAmount = 0f;
            if (OnMovingPlatform) tempFrictionAmount = 0.15f;

            if( frictionAmount != tempFrictionAmount)
            {
                frictionAmount -= Time.deltaTime * 10f;
                if (frictionAmount < tempFrictionAmount) frictionAmount = tempFrictionAmount;
            }

            this_Collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        this_Collider.material.dynamicFriction = frictionAmount;
        this_Collider.material.staticFriction = frictionAmount;
    }

    */

    #endregion
}
