using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public struct PlayerInput
{
    // Game Settings
    private float _f_zDir; // Forward or Backward
    private float _f_xDir; // Strafe Left or Right
    private float _f_LookHoriz;
    private float _f_LookVert;
    private bool _b_JumpPressed;
    private bool _b_JumpHeld;
    private bool _b_DPad_LeftPressed;
    private bool _b_DPad_RightPressed;
    private bool _b_DPad_UpPressed;
    private bool _b_DPad_DownPressed;
    private Vector2 _v2_DPad;
    private ButtonState _d_Button_A;
    private ButtonState _d_Button_B;
    private ButtonState _d_Button_X;
    private ButtonState _d_Button_Y;
    private ButtonState _Start;
    private ButtonState _Select;

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
    public bool DPad_Pressed_Left
    {
        set { _b_DPad_LeftPressed = value; }
        get { return _b_DPad_LeftPressed; }
    }
    public bool DPad_Pressed_Right
    {
        set { _b_DPad_RightPressed = value; }
        get { return _b_DPad_RightPressed; }
    }
    public bool DPad_Pressed_Up
    {
        set { _b_DPad_UpPressed = value; }
        get { return _b_DPad_UpPressed; }
    }
    public bool DPad_Pressed_Down
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
}

public class scr_PlayerInput : MonoBehaviour
{
    // Controller scripts
    [SerializeField] internal PlayerIndex player = PlayerIndex.One;
    GamePadState player_State;
    GamePadState player_PrevState;

    // PlayerInput struct
    internal PlayerInput playerInput;

    // Start is called before the first frame update

    float f_InputMinimum = 0.01f;
    Vector2 v2_DPad;
    Vector2 v2_DPad_Old;

    void Start()
    {
        playerInput = new PlayerInput();
        player_PrevState = new GamePadState();

        // Controller input
        v2_DPad = new Vector2();
        v2_DPad_Old = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
         player_State = GamePad.GetState(player);

        if( player_State.DPad.Down == ButtonState.Pressed )
        {
            print("Connected - Forcing New Branch");
        }
    }
}
