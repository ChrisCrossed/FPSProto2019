using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using System.IO;

public enum ControllerType
{
    Controller,
    KeyMouse
}

public struct PlayerInput
{
    private ControllerType controllerType;

    // Game Settings
    private float _f_zDir; // Forward or Backward
    private float _f_xDir; // Strafe Left or Right
    private float _f_LookHoriz;
    private float _f_LookVert;
    private bool _b_JumpPressed;
    private bool _b_JumpHeld;
    private ButtonState _b_DPad_LeftPressed;
    private ButtonState _b_DPad_RightPressed;
    private ButtonState _b_DPad_UpPressed;
    private ButtonState _b_DPad_DownPressed;
    private Vector2 _v2_DPad;
    private ButtonState _d_Button_A;
    private ButtonState _d_Button_B;
    private ButtonState _d_Button_X;
    private ButtonState _d_Button_Y;
    private ButtonState _Start;
    private ButtonState _Select;

    internal ControllerType InputType
    {
        set { controllerType = value; }
        get { return controllerType; }
    }

    #region Controller Input
    // Analog Sticks
    public float zDir
    {
        internal set
        {
            float f_Temp = value;

            // Cap the value between -1 and 1
            if (f_Temp < -1.0f) f_Temp = -1.0f;
            if (f_Temp > 1.0f) f_Temp = 1.0f;

            // If very close to 0f, set to 0f
            if (f_Temp < .05f && f_Temp > -0.05f) f_Temp = 0f;

            _f_zDir = f_Temp;
        }
        get
        {
            return _f_zDir;
        }
    }
    public float xDir
    {
        set
        {
            float f_Temp = value;

            // Cap the value between -1 and 1
            if (f_Temp < -1.0f) f_Temp = -1.0f;
            if (f_Temp > 1.0f) f_Temp = 1.0f;

            // If very close to 0f, set to 0f
            if (f_Temp < .05f && f_Temp > -0.05f) f_Temp = 0f;

            _f_xDir = f_Temp;
        }
        get
        {
            return _f_xDir;
        }
    }

    public float LookHoriz
    {
        set
        {
            _f_LookHoriz = value;
        }
        get { return _f_LookHoriz; }
    }
    public float LookVert
    {
        set
        {
            _f_LookVert = value;
        }
        get { return _f_LookVert; }
    }

    // DPad
    public ButtonState DPad_Pressed_Left
    {
        set { _b_DPad_LeftPressed = value; }
        get { return _b_DPad_LeftPressed; }
    }
    public ButtonState DPad_Pressed_Right
    {
        set { _b_DPad_RightPressed = value; }
        get { return _b_DPad_RightPressed; }
    }
    public ButtonState DPad_Pressed_Up
    {
        set { _b_DPad_UpPressed = value; }
        get { return _b_DPad_UpPressed; }
    }
    public ButtonState DPad_Pressed_Down
    {
        set { _b_DPad_DownPressed = value; }
        get { return _b_DPad_DownPressed; }
    }

    public Vector2 DPadVector
    {
        set { _v2_DPad = value; }
        get { return _v2_DPad; }
    }

    // A/B/X/Y
    public ButtonState Button_A
    {
        internal set { _d_Button_A = value; }
        get { return _d_Button_A; }
    }
    public ButtonState Button_B
    {
        internal set { _d_Button_B = value; }
        get { return _d_Button_B; }
    }
    public ButtonState Button_X
    {
        internal set { _d_Button_X = value; }
        get { return _d_Button_X; }
    }
    public ButtonState Button_Y
    {
        internal set { _d_Button_Y = value; }
        get { return _d_Button_Y; }
    }

    // Bumpers
    public ButtonState Bumper_Left
    {
        internal set;
        get;
    }
    public ButtonState Bumper_Right
    {
        internal set;
        get;
    }

    public ButtonState Trigger_Left
    {
        internal set;
        get;
    }
    public ButtonState Trigger_Right
    {
        internal set;
        get;
    }

    // Start & Select
    public ButtonState Button_Start
    {
        set { _Start = value; }
        internal get { return _Start; }
    }
    public ButtonState Button_Select
    {
        set { _Select = value; }
        get { return _Select; }
    }
    #endregion

    #region Keyboard Input
    private bool _Button_Forward;
    private bool _Button_Backward;
    private bool _Button_Strafe_Left;
    private bool _Button_Strafe_Right;

    private bool _Button_Jump;
    private bool _Button_Crouch;
    private bool _Button_Walk;

    private bool _Button_Ability_1;
    private bool _Button_Ability_2;
    private bool _Button_Ability_3;
    private bool _Button_Ability_Ult;

    private bool _Button_Weapon_Primary;
    private bool _Button_Weapon_Secondary;
    private bool _Button_Weapon_Melee;

    private Vector2 _v2_Mouse;
    private bool _Button_Mouse_Left;
    private bool _Button_Mouse_Right;

    public bool KM_Forward
    {
        set { _Button_Forward = value; }
        internal get { return _Button_Forward; }
    }
    public bool KM_Backward
    {
        set { _Button_Backward = value; }
        internal get { return _Button_Backward; }
    }
    public bool KM_Strafe_Left
    {
        set { _Button_Strafe_Left = value; }
        internal get { return _Button_Strafe_Left; }
    }
    public bool KM_Strafe_Right
    {
        set { _Button_Strafe_Right = value; }
        internal get { return _Button_Strafe_Right; }
    }

    #region Weapon selection
    public bool KM_Button_Weapon_Primary
    {
        set { _Button_Weapon_Primary = value; }
        internal get { return _Button_Weapon_Primary; }
    }
    public bool KM_Button_Weapon_Secondary
    {
        set { _Button_Weapon_Secondary = value; }
        internal get { return _Button_Weapon_Secondary; }
    }
    public bool KM_Button_Weapon_Melee
    {
        set { _Button_Weapon_Melee = value; }
        internal get { return _Button_Weapon_Melee; }
    }

    #endregion

    public bool KM_Button_Jump
    {
        set { _Button_Jump = value; }
        internal get { return _Button_Jump; }
    }
    public bool KM_Button_Crouch
    {
        set { _Button_Crouch = value; }
        internal get { return _Button_Crouch; }
    }
    public bool KM_Button_Walk
    {
        set { _Button_Walk = value; }
        internal get { return _Button_Walk; }
    }
    public Vector2 KM_Mouse_Movement
    {
        set { _v2_Mouse = value; }
        internal get { return _v2_Mouse; }
    }
    public bool KM_Mouse_Left
    {
        set { _Button_Mouse_Left = value; }
        internal get { return _Button_Mouse_Left; }
    }
    public bool KM_Mouse_Right
    {
        set { _Button_Mouse_Right = value; }
        internal get { return _Button_Mouse_Right; }
    }
    public bool KM_Ability_1
    {
        set { _Button_Ability_1 = value; }
        internal get { return _Button_Ability_1; }
    }
    public bool KM_Ability_2
    {
        set { _Button_Ability_2 = value; }
        internal get { return _Button_Ability_2; }
    }
    public bool KM_Ability_3
    {
        set { _Button_Ability_3 = value; }
        internal get { return _Button_Ability_3; }
    }
    public bool KM_Ability_Ult
    {
        set { _Button_Ability_Ult = value; }
        internal get { return _Button_Ability_Ult; }
    }
    #endregion

    public void OverrideControlsWithOfficialControls()
    {
        #region Part 1 - Check for BackupKeybinds.json file
        string output = "No file found";

        string localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

        localAppData += "\\VALORANT\\Saved\\Config";

        /* WORKS
        foreach (string file in Directory.GetFiles(localAppData, "BackupKeybinds.json", SearchOption.AllDirectories))
        {
            return file;
        }

        return output;
        */
        #endregion

        #region Part 2 - If file exists, overwrite default keys / mouse info
        // https://forum.unity.com/threads/how-to-read-json-file.401306/
        #endregion
    }
}

public class scr_PlayerInput : MonoBehaviour
{
    // Controller scripts
    [SerializeField] internal PlayerIndex player = PlayerIndex.One;
    GamePadState player_State;
    GamePadState player_PrevState;

    // PlayerInput struct
    internal PlayerInput playerInput;
    internal PlayerInput playerInput_OLD;

    // Start is called before the first frame update

    float f_InputMinimum = 0.01f;
    Vector2 v2_DPad;
    Vector2 v2_DPad_Old;

    void Start()
    {
        playerInput = new PlayerInput();
        playerInput_OLD = new PlayerInput();
        player_PrevState = new GamePadState();

        // Controller input
        v2_DPad = new Vector2();
        v2_DPad_Old = new Vector2();
    }

    // Update is called once per frame
    protected void UpdatePlayerInput()
    {
        if(playerInput.InputType == ControllerType.Controller)
        {
            #region Capture new input this frame
            player_State = GamePad.GetState(player);
            playerInput_OLD = playerInput; // Externally Referenced
            #endregion

            #region Reset DPad Bools and Vector2
            // Reset values
            playerInput.DPad_Pressed_Up = ButtonState.Released;
            playerInput.DPad_Pressed_Down = ButtonState.Released;
            playerInput.DPad_Pressed_Left = ButtonState.Released;
            playerInput.DPad_Pressed_Right = ButtonState.Released;
            v2_DPad = new Vector2();
            #endregion

            if (player_State.DPad.Up == ButtonState.Pressed && player_State.DPad.Down == ButtonState.Released)
            {
                playerInput.DPad_Pressed_Up = ButtonState.Pressed;
            }
            if (player_State.DPad.Down == ButtonState.Pressed && player_State.DPad.Up == ButtonState.Released)
            {
                playerInput.DPad_Pressed_Down = ButtonState.Pressed;
            }
            if (player_State.DPad.Left == ButtonState.Pressed && player_State.DPad.Right == ButtonState.Released)
            {
                playerInput.DPad_Pressed_Left = ButtonState.Pressed;
            }
            if (player_State.DPad.Right == ButtonState.Pressed && player_State.DPad.Left == ButtonState.Released)
            {
                playerInput.DPad_Pressed_Right = ButtonState.Pressed;
            }

            #region Replace old input from last frame
            player_PrevState = player_State;
            #endregion
        }
        else if(playerInput.InputType == ControllerType.KeyMouse)
        {
            #region WASD
            playerInput.KM_Forward = false;
            playerInput.KM_Backward = false;
            playerInput.KM_Strafe_Left = false;
            playerInput.KM_Strafe_Right = false;

            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                playerInput.KM_Forward = true;
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                playerInput.KM_Backward = true;

            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                playerInput.KM_Strafe_Left = true;
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                playerInput.KM_Strafe_Right = true;
            #endregion

            #region Weapon Selection
            playerInput.KM_Button_Weapon_Primary = false;
            playerInput.KM_Button_Weapon_Secondary = false;
            playerInput.KM_Button_Weapon_Melee = false;

            if (Input.GetKey(KeyCode.Alpha1))
                playerInput.KM_Button_Weapon_Primary = true;
            if (Input.GetKey(KeyCode.Alpha2))
                playerInput.KM_Button_Weapon_Secondary = true;
            if (Input.GetKey(KeyCode.Alpha3))
                playerInput.KM_Button_Weapon_Melee = true;
            #endregion

            #region Alternate Buttons
            playerInput.KM_Button_Jump = false;
            playerInput.KM_Button_Crouch = false;
            playerInput.KM_Button_Walk = false;

            if (Input.GetKey(KeyCode.Space))
                playerInput.KM_Button_Jump = true;

            if (Input.GetKey(KeyCode.LeftControl))
                playerInput.KM_Button_Crouch = true;
            
            if (Input.GetKey(KeyCode.LeftShift))
                playerInput.KM_Button_Walk = true;
            #endregion

            #region Player Abilities
            playerInput.KM_Ability_1 = false;
            playerInput.KM_Ability_2 = false;
            playerInput.KM_Ability_3 = false;
            playerInput.KM_Ability_Ult = false;

            if (Input.GetKey(KeyCode.Q))
                playerInput.KM_Ability_1 = true;
            if (Input.GetKey(KeyCode.E))
                playerInput.KM_Ability_2 = true;
            if (Input.GetKey(KeyCode.C))
                playerInput.KM_Ability_3 = true;
            if (Input.GetKey(KeyCode.X))
                playerInput.KM_Ability_Ult = true;
            #endregion

            #region Mouse Input
            // Reset Mouse Position
            Vector2 tempMouse = new Vector2();

            // Mouse Horizontal Input
            tempMouse.x = Input.GetAxis("Mouse X");

            // Mouse Vertical Input
            tempMouse.y = -Input.GetAxis("Mouse Y");

            // Replace Mouse Input
            playerInput.KM_Mouse_Movement = tempMouse;

            // Mouse Left/Right Buttons
            playerInput.KM_Mouse_Left = false;
            playerInput.KM_Mouse_Right = false;

            if (Input.GetAxis("Fire1") != 0f)
                playerInput.KM_Mouse_Left = true;

            if (Input.GetAxis("Fire2") != 0f)
                playerInput.KM_Mouse_Right = true;
            #endregion
        }
    }
}
