using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cs_DamageUI : MonoBehaviour
{
    float UITimer;
    static float UI_TIMER_MAX = 2.0f;

    GameObject player;
    GameObject playerCamObj;
    Camera playerCam;

    GameObject damageImageObj;
    Image damageImage;

    Vector3 damageUIWorldPos;
    int damageDealt;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").gameObject;
        playerCamObj = player.transform.Find("PlayerCamera").gameObject;
        playerCam = playerCamObj.GetComponent<Camera>();

        damageImageObj = transform.Find("DamageInfo").gameObject;
        damageImage = damageImageObj.GetComponent<Image>();

        SetUITextVisibility(false);
    }

    public void ShowDamageInWorld( Vector3 targetDummyUIPos_, int damage_ )
    {
        // Reset UITimer
        UITimer = UI_TIMER_MAX;

        // Apply world position
        damageUIWorldPos = targetDummyUIPos_;

        // Assign damage dealt
        damageDealt = damage_;

        // Turn text on visually
        SetUITextVisibility(true);
    }

    void SetUITextVisibility( bool isVisible_ )
    {
        if( isVisible_ ) damageImageObj.GetComponent<Text>().enabled = true;
        else
        {
            damageImageObj.GetComponent<Text>().enabled = false;

            // Reposition the UI image (Hacky solution to prevent flickering)
            damageImageObj.transform.position = new Vector3(-5000, -5000, -10);
        }
    }

    bool GetUITextVisibility
    {
        get
        {
            if (damageImageObj.GetComponent<Text>().color.a != 0f)
            {
                return true;
            }

            return false;
        }
    }

    void SetUITextDamage( int damage_ )
    {
        damageImageObj.GetComponent<Text>().text = "" + damage_;
    }

    void UpdateDamageUI()
    {
        // Find screen pos
        Vector3 screenPos = playerCam.WorldToScreenPoint(damageUIWorldPos);

        // Reposition the UI image
        damageImageObj.transform.position = screenPos;

        // If Z is negative, don't show
        if (screenPos.z < 0f)
        {
            SetUITextVisibility(false);
            return;
        }
        else
            SetUITextVisibility(true);

        // Change UI scale based on distance
        float dist = Vector3.Distance(player.transform.position, damageUIWorldPos);
        float scale = (1f / dist) * 5f;

        if (scale > 1.0f) scale = 1.0f;
        if (scale < 0.45) scale = 0.45f;

        Vector2 newScale = new Vector2(scale, scale);
        damageImageObj.GetComponent<RectTransform>().localScale = newScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (UITimer > 0f)
        {
            UITimer -= Time.deltaTime;

            UpdateDamageUI();

            if (UITimer < 0f)
            {
                UITimer = 0f;

                SetUITextVisibility(false);
            }
        }
    }
}
