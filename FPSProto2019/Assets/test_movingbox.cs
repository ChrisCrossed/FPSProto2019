using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_movingbox : MonoBehaviour
{
    // Start is called before the first frame update
    float startingYPos;
    void Start()
    {
        startingYPos = gameObject.transform.position.y;
    }

    // Update is called once per frame
    float f_Timer;
    void FixedUpdate()
    {
        f_Timer += Time.fixedDeltaTime;

        float newY = Mathf.Sin(f_Timer) * 5f;
        newY += startingYPos;

        float newX = Mathf.Sin(f_Timer) * 5f;
        newX += startingYPos;

        Vector3 newPos = gameObject.transform.position;
        newPos.y = newY;
        newPos.x = newX;
        gameObject.GetComponent<Rigidbody>().MovePosition(newPos);
        gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(Vector3.up * Mathf.Sin(f_Timer) * 90));
    }
}
