using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRot : MonoBehaviour
{
    [SerializeField]
    float RotationSpeed = 15f;

    float Timer;

    // Start is called before the first frame update
    void Start()
    {
        Timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime * RotationSpeed;

        Timer %= 360f;

        Vector3 v3Rotation = gameObject.transform.eulerAngles;
        v3Rotation.y = Timer;

        gameObject.transform.eulerAngles = v3Rotation;
    }
}
