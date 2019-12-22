using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public class scr_PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
